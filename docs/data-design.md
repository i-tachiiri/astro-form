
## 1 📂 コレクション概要

| Collection          | 主な役割           | Partition Key   | 代表的アクセス              | 備考                    |
| ------------------- | -------------- | --------------- | -------------------- | --------------------- |
| **Users**           | ログイン関係者のプロファイル | `/id`           | ID 参照・ログイン時取得        | Entra External ID を使用 |
| **Forms**           | フォーム定義 + 項目    | `/userId`       | ユーザー別一覧・ID 詳細取得      | items は埋め込み           |
| **FormSubmissions** | フォーム回答結果       | `/formId`       | formId で一覧           | answers に PII         |
| **ActivityLogs**    | 操作・イベント履歴      | `/partitionKey` | userId / formId / 期間 | ログ種別で PK 分岐           |

---

## 2 🗄️ スキーマ詳細

> **Notation** : JSON (camelCase)、ISO 8601 UTC。

### 2.1 Users

```json
{
  "id": "user-abcdef12345",
  "displayName": "山田 太郎",
  "email": "taro.yamada@example.com",
  "role": "FortuneTeller",
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-06-20T14:30:00Z"
}
```

### 2.2 Forms (embedded items)

```json
{
  "id": "form-guid-12345",
  "userId": "user-abcdef12345",
  "name": "星占いフォーム 2024年版",
  "description": "2024年のあなたの運勢を占います。",
  "navigationText": "Astro Form",
  "thankYouPageUrl": "https://astroform.com/thanks",
  "status": "Published",
  "createdAt": "2024-06-01T09:00:00Z",
  "updatedAt": "2024-06-20T10:15:00Z",
  "items": [ /* see Appendix A */ ]
}
```

### 2.3 FormSubmissions

```json
{
  "id": "submission-guid-67890",
  "formId": "form-guid-12345",
  "answers": "{\"item-guid-a1\":\"田中\", ...}",
  "submittedAt": "2024-06-20T15:00:00Z",
  "submitterInfo": "{\"ipAddress\":\"203.0.113.1\", ...}"
}
```

### 2.4 ActivityLogs

```json
{
  "id": "log-unique-id-1",
  "partitionKey": "user-abcdef12345",
  "timestamp": "2024-06-20T15:05:00Z",
  "userId": "user-abcdef12345",
  "formId": null,
  "actionType": "USER_LOGIN",
  "details": "{\"message\":\"ユーザー 'taro.yamada@example.com' がログインしました。\"}"
}
```

## 3 🛡️ セキュリティ & コンプライアンス

1. **暗号化** : `FormSubmissions.answers` は **AES‑256** で暗号化し、鍵は **Azure Key Vault** で管理。
2. **RBAC** : Managed Identity + Cosmos RBAC で *read/write* をロール分離。
3. **PII 削除** : GDPR/LAPPS 対応として `userId` / `formId` パーティション単位で TTL または手動削除 API。
4. **監査ログ** : ActivityLogs にアクセス監査を含め、保持期間 730 days。
5. **バックアップ** : 自動バックアップ (4h/7d) に加え、**週次で Azure Blob Storage にエクスポート**。Blob は RA‑GRS で冗長化。

---
