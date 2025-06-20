## ✅ 改訂版タスク一覧（全画面 UI 実装指示）

### 1. feat: 開発環境の整備




- docs/architectures/application-design.md を参照し、各プロジェクトとサンプル UnitTest を追加する
- Github Actions に `test-and-build.yml`（ビルド & テスト）を配置
-  `.editorconfig` を追加し `dotnet format` が通る設定を確定
- Serilog を導入し、`Console.WriteLine` を禁止する CA ルールを有効化
- docs/azure-setup.md を参照し、Bicep/ARM でサービスを定義
- docs/docker-setup.md を参照し、docker関連のセットアップを行う
- `docs/ui-design.md` に記載のある全画面を作成。コンポーネントは`docs/components.md` に基づく
- 

- `dotnet test`が通る事を確認
- nektos/actでテストが通るか確認
 - `dotnet format`で差分がない事を確認
- `dotnet build`が通る事を確認





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
