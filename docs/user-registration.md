# ユーザー登録フローとロール管理

Microsoft Entra External ID (旧 Azure AD B2C) を利用して占い師や運用担当のアカウントを管理します。

## 登録フロー
1. フロントエンドの **S01 Login** 画面から Entra External ID のサインアップ／サインイン画面へ遷移。
2. 認証完了後、取得した `objectId` 等の情報を用いて `/users/register` API を呼び出します。
3. API は `UserService` 経由で `Users` コレクションにプロフィールを保存します。
4. 初回登録時のロールは `FortuneTeller` とし、必要に応じて運用担当が変更します。

## ロール管理
- `Domain/Entities.cs` で `UserRole` 列挙体を定義しています。
- ロールは `/users/{id}/role` エンドポイントから更新できます。
- 運用担当は Entra External ID ポータルまたは本 API を通じてロールを変更します。
