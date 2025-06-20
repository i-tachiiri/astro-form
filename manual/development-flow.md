1. **feat:** Conventional Commit を守りつつ `dotnet new sln` → 3 層 (`Presentation/`, `Domain/`, `Infra/`) のプロジェクト雛形を生成し、サンプル UnitTest を追加せよ。加えて GitHub Actions に `test-and-build.yml`（ビルド & テスト）を配置すること。

2. **chore:** `.editorconfig` を追加し、`dotnet format` で自動整形が通る設定を確定せよ。また Serilog の最小構成を導入し、`Console.WriteLine` を禁止する静的解析ルール（CA1416 など）を有効化せよ。

3. **infra:** `infra/` 配下に Bicep もしくは ARM で **Static Web Apps, Azure Functions, Blob Storage, Key Vault, Cosmos DB Free Tier** を定義せよ。さらに `deploy-infra.yml` を追加し、リソース グループ名を変えるだけで Dry-Run／Deploy が実行できる状態にすること。

4. **dev:** `docker-compose.yml` を作成し、**Azurite + Cosmos Emulator + SWA CLI** を一括起動できるようにせよ。あわせて `README.md` に起動手順を追記し、S3 達成の前提となる `BackupService` の **Echo API**（`/api/echo` が常に `200 OK` + ペイロードを返す）を実装せよ。

5. **feat:** SWA テンプレートのフロントエンド（`Client/`）と Functions API（`Api/`）を生成し、Functions から Key Vault シークレットをマネージド ID 経由で取得して JSON 返却。クライアント側で fetch し画面に表示して S5 の最小要件を満たすこと。

6. **feat:** `BackupService --sync` コマンドを実装し、ローカル JSON を Cosmos Emulator へ書き込み、Azurite へファイルをアップロードできるようにせよ。xUnit + Verify でレスポンスのスナップショットテストを追加すること。

7. **ci:** `release-desktop.yml` を追加し、Avalonia デスクトップアプリを self-contained で発行、署名モック後に Blob `$web/updates` へアップロードするパイプラインを構築せよ。

8. **test:** `coverlet` を統合し、行カバレッジ 70 %以上を CI で必須とする。閾値に満たない場合はビルドを失敗させること。

9. **chore:** PR テンプレートを追加し、AGENTS.md §6 のチェックリスト（Build/Test/Infra Dry-Run/Spec Compliance など）が自動で付与されるようにせよ。

10. **docs:** `README.md` を更新し、**clone → build → desktop run → static apps run → sync** までを手作業ゼロで再現する手順を記載。CI/CD とデプロイ バッジも追加して全緑を確認せよ。
