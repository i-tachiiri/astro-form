# ☁ Azure リソース構築ガイド（azure-setup.md）

本プロジェクトでは、以下の Azure リソースを **Infrastructure as Code (IaC)** で管理します。`infra/` 配下の **Bicep / ARM テンプレート**をリポジトリにコミットし、GitHub Actions (`deploy-infra.yml`) から **環境別に自動デプロイ**できる構成を採用します。

---

## ✅ 対象リソース一覧

| リソース名           | 用途                            | プラン/構成例                |
| --------------- | ----------------------------- | ---------------------- |
| Static Web Apps | フロントエンド（Blazor WASM）のホスティング   | Free プラン               |
| Azure Functions | API 実行エンジン（バックエンド）            | Consumption Plan       |
| Blob Storage    | ファイル同期／PDF 格納                 | Standard LRS, Hot tier |
| Key Vault       | 機密情報（Cosmos 接続文字列等）のセキュア保管    | RBAC ベースで制御            |
| Cosmos DB       | NoSQL データベース（Form / Answer 等） | Free Tier (1,000 RU/s) |

---

## 🔀 環境分離戦略（dev / prod）

| 項目                 | 開発環境 (dev)                       | 本番環境 (prod)                  |
| ------------------ | -------------------------------- | ---------------------------- |
| **パラメータファイル**      | `infra/parameters/dev.json`      | `infra/parameters/prod.json` |
| **リソースグループ名**      | `rg-demo-app-dev`                | `rg-demo-app-prod`           |
| **リソース名の接尾辞**      | `-dev`                           | なし（または `-prod`）              |
| **CIトリガー**         | `develop` ブランチ / `feature/**` PR | `main` ブランチ / リリースタグ         |
| **GitHub Secrets** | `AZURE_CREDENTIALS_DEV`          | `AZURE_CREDENTIALS_PROD`     |

> **ポイント** : 開発環境と本番環境を完全に分離することで、検証デプロイが本番に影響しないようにします。

---

## 🧱 ディレクトリ構成

```text
infra/
├── main.bicep               # 主要テンプレート（module 呼び出し形式）
├── modules/
│   ├── swa.bicep           # Static Web Apps
│   ├── function.bicep      # Azure Functions
│   ├── blob.bicep          # Blob Storage
│   ├── keyvault.bicep      # Key Vault
│   └── cosmos.bicep        # Cosmos DB
└── parameters/
    ├── dev.json            # 開発環境用パラメータ
    └── prod.json           # 本番環境用パラメータ
```

---

## 🆓 Azure 無料枠で運用するためのポイント

- **Static Web Apps** : Free プランを選択。カスタムドメインが 1 つまで利用可能です。
- **Azure Functions** : Consumption プランは月 100 万リクエストまで無料。常時稼働はせず、必要に応じてウォームアップ関数を利用します。
- **Cosmos DB** : データベース作成時に Free Tier を有効化し、1,000 RU/s 以内で運用します。
- **Blob Storage / Key Vault** : 標準の LRS / Standard SKU を利用し、無料枠の範囲で収まるようデータ量を管理します。

これらの設定は `infra/` 配下の Bicep テンプレートに反映済みのため、そのままデプロイすれば無料枠で動作する構成になります。

無料枠設定が維持されているか確認するため、`scripts/check-free-tier.sh` を実行してテンプレートを検証できます。
また、月次の利用料金を確認する `scripts/monitor-costs.sh` も用意しており、定期的に実行することでコスト超過を早期に検知できます。

## 🚀 デプロイ手順（ローカル）

### 1. Azure CLI にログイン

```bash
az login
az account set --subscription <your-subscription-id>
```

### 2. リソースグループを作成（例：dev）

```bash
az group create --name rg-demo-app-dev --location japaneast
```

### 3. Dry‑Run（what‑if）で差分確認

```bash
az deployment group what-if \
  --resource-group rg-demo-app-dev \
  --template-file infra/main.bicep \
  --parameters @infra/parameters/dev.json
```

### 4. 実デプロイ

```bash
az deployment group create \
  --resource-group rg-demo-app-dev \
  --template-file infra/main.bicep \
  --parameters @infra/parameters/dev.json
```

> 本番環境の場合は `rg-demo-app-prod` と `prod.json` に読み替えてください。

---

## 🤖 GitHub Actions による自動デプロイ

`deploy-infra.yml` は **ブランチごとに環境を切り替えてデプロイ**します。

```yaml
name: Deploy Azure Infrastructure

on:
  push:
    branches: [ main, develop ]
  workflow_dispatch:
    inputs:
      env:
        type: choice
        description: "Target environment"
        options: [ dev, prod ]
        default: dev

jobs:
  deploy:
    runs-on: ubuntu-latest
    env:
      ENV_NAME: ${{ github.ref == 'refs/heads/main' && 'prod' || 'dev' }}
    steps:
      - uses: actions/checkout@v4
      - uses: azure/login@v1
        with:
          creds: ${{ secrets[format('AZURE_CREDENTIALS_{0}', env.ENV_NAME)::toUpper()] }}

      - name: Azure ARM deploy (what-if)
        uses: azure/arm-deploy@v1
        with:
          scope: resourcegroup
          resourceGroupName: rg-demo-app-${{ env.ENV_NAME }}
          template: infra/main.bicep
          parameters: infra/parameters/${{ env.ENV_NAME }}.json
          deploymentMode: Validate

      - name: Azure ARM deploy (create)
        uses: azure/arm-deploy@v1
        with:
          scope: resourcegroup
          resourceGroupName: rg-demo-app-${{ env.ENV_NAME }}
          template: infra/main.bicep
          parameters: infra/parameters/${{ env.ENV_NAME }}.json
          deploymentMode: Incremental
```

---



