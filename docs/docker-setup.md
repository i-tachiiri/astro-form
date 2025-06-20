# 🐳 ローカル開発環境セットアップガイド（local-dev-setup.md）

本プロジェクトでは、クラウドサービスと類似の環境をローカルで再現し、検証や開発を効率化するために Docker ベースのエミュレーション環境を用意しています。

---

## ✅ 起動するサービス一覧（docker-compose）

| サービス名           | 目的                                      |
| --------------- | --------------------------------------- |
| Azurite         | Azure Blob Storage のローカルエミュレーター         |
| Cosmos Emulator | Azure Cosmos DB のローカルエミュレーター            |
| SWA CLI         | Static Web Apps のローカルサーバー（Blazor WASM等） |

---

## 📁 ファイル構成例

```
/docker-compose.yml
/docker/
  ├── cosmos/         # Cosmos Emulator 設定
  └── azurite/        # Azurite 設定（必要に応じて）
```

---

## 🚀 起動手順

```bash
docker-compose up -d
```

> ブラウザから：
>
> * Cosmos Emulator: [http://localhost:8081/\_explorer/index.html](http://localhost:8081/_explorer/index.html)
> * Azurite Blob: [http://127.0.0.1:10000/devstoreaccount1](http://127.0.0.1:10000/devstoreaccount1)

---

## 🔁 BackupService の検証用エンドポイント

バックアップ機能の動作検証のため、API プロジェクト内に `/api/echo` エンドポイントを実装しています。

### 📌 `/api/echo` の仕様

* メソッド: `POST`
* 入力: 任意の JSON ペイロード
* 出力: 同じ JSON を `200 OK` で返す

### ✅ 使い方（curl 例）

```bash
curl -X POST http://localhost:7071/api/echo \
     -H "Content-Type: application/json" \
     -d '{"message":"ping"}'
```

### 🎯 使用目的

* フロントエンド〜Functions API 間の疎通確認
* CI でのレスポンス Snapshot テスト対象

---

## 🔧 補足

* Cosmos Emulator の接続には `AccountEndpoint=https://localhost:8081/;AccountKey=...` を使用
* Blazor WASM からの接続先を `http://localhost:7071/api/` に設定すると、SWA CLIと連携可能

---

## 🔒 Key Vault の扱い（ローカル環境）

Azure Key Vault には公式エミュレーターがありません。そのためローカル開発では次のいずれかの方法で **シークレットを疑似置換** します。

| アプローチ                                      | 使い方                                                                                | 適切なケース                       |
| ------------------------------------------ | ---------------------------------------------------------------------------------- | ---------------------------- |
| **.env / user‑secrets**                    | `Directory.Build.props` で `UserSecretsId` を設定し、`dotnet user-secrets set KEY VALUE` | C# コードが直接シークレットにアクセスする場合。    |
| **`local.settings.json`**                  | Functions のローカル設定（`Values` セクション）にシークレットを記載                                        | Functions だけがシークレットを必要とする場合。 |
| **Azure CLI / VS Code Signed‑in Identity** | `DefaultAzureCredential` を使用し、Azure CLI でログインしたユーザーから Key Vault を取得                | 実際の Key Vault で動作検証したい場合。    |

> CI 環境では GitHub Secrets に設定し、`azure/login` アクションの Managed Identity で Key Vault へアクセス。

## 📦 Blob Storage について

* **Azurite** が Blob Storage のローカルエミュレーターとして既にコンテナに含まれています。
* 開発用接続文字列は `UseDevelopmentStorage=true` または `DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=...` を利用してください。

---
