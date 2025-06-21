# 🚀 Astro Form テンプレート — リポジトリ概要

> **対象読者**: 本リポジトリを操作・拡張する AI／人間コントリビュータ
> **目的**: `docs/development-flow.md` に従って実装を完了しつつ、`docs/ai-review.md`へAIから人間の改善要望をあげる事

---

## 📂 ディレクトリ構成（抜粋）

| パス                   | 役割・内容                                          |
| -------------------- | ---------------------------------------------- |
| `/src/`              | アプリケーション本体（Functions / BFF など）。  |
| `/infra/`            | IaC スクリプト（Bicep / ARM / Terraform など想定）。       |
| `/docs/`             | 仕様・設計ドキュメント一式。詳細は下記参照。                         |
| `/tests/`            | Unit / Integration テスト群。                       |
| `.github/workflows/` | CI/CD（build / test / deploy / release）。        |

### `/docs` 配下

`development-flow.md` を参照すれば、どの実装の際にどのドキュメントを参照すべきか書いてあります。

| ファイル名                                    | 目的・AIが参照すべきポイント                                          |
| ---------------------------------------- | -------------------------------------------------------- |
| `requirements.md`                        | ビジネス要求・課題・目的の整理。                              |
| `use-cases.md`                           | ユースケース定義・ユーザーストーリーの整理。                                   |
| `ui-design.md`                           | 画面一覧・遷移図・レイアウト構成・レスポンシブ対応の指針。                            |
| `components.md`                          | UI・Functions 含む**再利用可能な構成部品**の定義と設計指針。                   |
| `entities.md`                            | ドメインモデルのエンティティ仕様。**C# クラス構成とのマッピング**に関わる内容。              |
| `data-design.md`                         | データモデル・ER図・Cosmos DB の**コレクション設計**とパーティション設計。            |
| `api-spec.md`                            | API のエンドポイント定義、リクエスト・レスポンス構造、OpenAPI 仕様。                 |
| `application-design.md`（architectures配下） | アプリケーション層全体の構成設計。DI・責務分離・各層のやり取りの定義。                     |
| `ci.md`                                  | GitHub Actions等による CI/CD パイプラインの設計。                      |
| `docker-setup.md`                        | Docker / Azurite / ローカル開発コンテナ構築手順。                       |
| `azure-setup.md`                         | Azure リソース（Cosmos DB, Blob, Functions, Key Vault等）の作成手順。 |
| `security.md`                            | 認証（EasyAuthなど）、認可、暗号化方式（例：AES256+Key Vault）、セキュリティ設計の全般。 |
| `user-registration.md`                   | Microsoft Entra External ID を用いたユーザー登録フローとロール管理の手順。 |
| `development-guideline.md`               | コーディング規約（.editorconfig）、ログ（Serilog）、構成指針。                |
| `development-flow.md`                    | **最初に読むべき**プロジェクト進行・レビュー・実装・テスト・デプロイなどの**開発フロー全体**。                 |
| `ai-review.md`                           | AIによるコードレビューや改善提案の記録・注釈（AIペアコーディング向け）。                   |


> 🔖 **ドキュメント更新の原則**

- ドキュメントは人間が記載します
- AIは`docs/ai-review.md` に改善点を箇条書きにして下さい

---

## 🛠️ AI 向け作業フロー

1. **Read → Plan → Modify** の 3 ステップを厳守。
2. 仕様矛盾を検出した場合は `docs/ai-review.md`へ追記
3. Infra/CI 変更はコスト増チェックを自動実行。🪙 +¥500/月 を超える場合 `cost:review` ラベル必須。

---

## ⚡ クイックスタート（人間開発者向け）

```bash
# 1. clone & dev container
gh repo clone <fork>
cd <repo>
# 推奨: Dev Containers で開いて必要ツールを自動インストール

# 2. ローカル実行
make run-functions    # Azure Functions + Azurite + Cosmos Emulator

# 3. Static Apps (front-end) 開発
swa start --run "dotnet watch run"
```

> 💡 **Tips**: `.env.sample` をコピーして `.env` を作成すると、ローカル開発で Key Vault 依存をバイパスできます。

---

## 📜 ライセンス

MIT License

---

