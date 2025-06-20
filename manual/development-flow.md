1. **feat:** Conventional Commit を守りつつ `dotnet new sln` → 3 層 (`Presentation/`, `Domain/`, `Infra/`) のプロジェクト雛形を生成し、サンプル UnitTest を追加せよ。加えて GitHub Actions に `test-and-build.yml`（ビルド & テスト）を配置すること。

2. **chore:** `.editorconfig` を追加し、`dotnet format` で自動整形が通る設定を確定せよ。また Serilog の最小構成を導入し、`Console.WriteLine` を禁止する静的解析ルールを有効化せよ。

3. **infra:** `infra/` 配下に Bicep もしくは ARM で **Static Web Apps, Azure Functions, Blob Storage, Key Vault, Cosmos DB (Free Tier)** を定義せよ。さらに `deploy-infra.yml` を追加し、リソース グループ名を変えるだけで Dry-Run／Deploy が実行できる状態にすること。

4. **dev:** `docker-compose.yml` を作成し、**Azurite + Cosmos Emulator + SWA CLI** を一括起動できるようにせよ。あわせて `README.md` に起動手順を追記し、S3 達成の前提となる `BackupService` の **Echo API**（`/api/echo` が常に `200 OK` + ペイロードを返す）を実装せよ。

5. **feat:** **UI スキャフォールド**  
   - `Presentation/Client/` に **Login (S01)**・**Dashboard (S10)**・**FormBuilder (S11)**・**FormViewer (Public)** の 4 画面を生成し、SWA ルーティングと共通サイドバー／ヘッダーを実装すること。  
   - TailwindCSS を導入し、アクセシビリティ AA 準拠のテーマを適用せよ。  

6. **feat:** **ドメイン & データ層**  
   - `Domain/Entities/` に **FormEntity, QuestionEntity, AnswerEntity, UserEntity** を定義し、値オブジェクト／ファクトリパターンを導入。  
   - `Infra/Cosmos/` に汎用 `ICosmosRepository<T>` + 4 具体リポジトリを実装し、DI 登録を行うこと。  

7. **feat:** **Functions API**  
   - `Api/` に CRUD エンドポイントを実装：`/forms`, `/forms/{id}/questions`, `/answers`, `/users`。  
   - Key Vault から取得した接続文字列で Cosmos DB にアクセスし、上記リポジトリ経由で読み書き。  
   - OpenAPI (Swashbuckle) を有効にし `/.well-known/openapi.json` を公開すること。  

8. **feat:** **BackupService --sync**  
   - ローカル JSON データを Cosmos Emulator へ書き込み、アップロード対象のファイルを Azurite へ同期。  
   - xUnit + Verify でレスポンスのスナップショットテストを追加し、CI で実行せよ。  

9. **ci:** `coverlet` を統合し、行カバレッジ 70 %以上を必須にする。閾値未満ならビルド失敗。  
   - PR テンプレートを追加し、AGENTS.md §6 のチェックリスト（Build/Test/Infra Dry-Run/Spec Compliance など）が自動付与されるようにせよ。

10. **docs:** 既存の `/docs` 以下の  
      - `ui-design.md`  
      - `data-design.md`  
      - `entities.md`  
      - `security.md`  
    の **内容を仕様と実装に照らしてレビューし、差分があれば追補** せよ。  
    併せて `README.md` を更新し、**clone → build → swa start → sync** までを手作業ゼロで再現する手順と CI/CD バッジを追加すること。
