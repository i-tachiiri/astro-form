## ✅ 改訂版タスク一覧（全画面 UI 実装指示）

### 1. feat: 開発環境の整備

- docs/architectures/application-design.md を参照し、各プロジェクトとサンプル UnitTest を追加する
- Github Actions に `test-and-build.yml`（ビルド & テスト）を配置する
-  `.editorconfig` を追加し `dotnet format` が通る設定を確定
- Serilog を導入し、`Console.WriteLine` を禁止する CA ルールを有効化

### 2. infra: Azure環境の整備

* `infra/` に Bicep/ARM で以下を定義：

  * Static Web Apps
  * Azure Functions
  * Blob Storage
  * Key Vault
  * Cosmos DB (Free Tier)
* `deploy-infra.yml` により Dry-Run／Deploy を自動化。

### 3. dev: Docker環境の整備

* `docker-compose.yml` を作成し、以下を一括起動：

  * Azurite
  * Cosmos Emulator
  * SWA CLI
* `README.md` に起動手順を追記。
* `BackupService` に `/api/echo` エンドポイントを実装（200 OK + ペイロードを返す）。

### 5. feat: UI スキャフォールド（全13画面）

`docs/ui-design.md` と `docs/components.md` に基づき、以下のルートを実装：

| 画面ID | ルート例                 | 主な機能                              |
| ---- | -------------------- | --------------------------------- |
| S01  | `/login`             | Microsoft Entra External ID サインイン |
| S02  | `/forms`             | フォーム一覧・検索・新規作成                    |
| S03  | `/forms/:id/edit`    | フォーム設定 + 質問設計 + 公開                |
| S04  | `/forms/:id/preview` | HTML 生成プレビュー                      |
| S05  | `/thank-you`         | サンクスページ（または外部 URL）                |
| S06  | `/logs`              | ログ閲覧・フィルタ                         |
| S07  | `/account`           | プロフィール・ポリシー表示                     |
| S08  | `/privacy`           | 静的プライバシーポリシー                      |
| S09  | `/account/delete`    | アカウント削除確認                         |
| S10  | `/good-bye`          | アカウント削除後                          |
| S11  | `/answers`           | 回答結果の確認・CSV 出力                    |
| S12  | `/contact`           | 問い合わせフォームの編集                      |
| S13  | `/p/:slug`           | 一般公開フォーム表示                        |

* TailwindCSS + A11y AA テーマ適用。
* 共通 UI コンポーネントは `docs/components.md` に従って構築。

### 6. feat: ドメイン & データ層設計

* `Domain/Entities/` に Form, Question, Answer, User, Log を定義。
* 値オブジェクト + ファクトリパターンを適用。
* `Infra/Cosmos/` に `ICosmosRepository<T>` + 各具体リポジトリを実装し DI 登録。

### 7. feat: Functions API

* CRUD エンドポイントを用意：

  * `/forms`
  * `/questions`
  * `/answers`
  * `/users`
  * `/logs`
* Key Vault から Cosmos 接続文字列取得。
* Swashbuckle により OpenAPI `/openapi.json` を公開。

### 8. feat: BackupService --sync

* ローカル JSON → Cosmos Emulator へ書き込み。
* Azurite にファイル同期。
* xUnit + Verify によるレスポンススナップショットテストを実装。

### 9. ci: カバレッジとPRルール

* `coverlet` により 70% 行カバレッジを必須化。
* PR テンプレートで `AGENTS.md §6` のチェックリストを自動適用。

### 10. docs: ドキュメント整合性確認

* `/docs` 配下の以下ドキュメントを実装に合わせて更新：

  * `ui-design.md`
  * `components.md`
  * `data-design.md`
  * `entities.md`
  * `security.md`
* `README.md` に clone → build → swa start → sync の手順と CI/CD バッジを追記。
