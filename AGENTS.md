# 🤖 AGENT マニフェスト

> **対象読者**: リポジトリを自動生成・保守するペアコーディング AI
> **関連ドキュメント**: まず *spec/template-spec.md* を読んでください。本ファイルは実行ルールです。
> **最終更新日**: 2025‑06‑20

---

## 1. ミッション

**Azure 静的アプリ + Functions** スタック（Static Web Apps, Blob Storage, Key Vault, Cosmos DB）で構成された プロジェクト群を作成し、機能／非機能要件をすべて満たしながら月額コストを抑える。

---

## 2. 入力ソース

1. **仕様書一式** – `/docs` 配下
2. **ユーザープロンプト** – ChatGPT / `ai:request` ラベル付き GitHub Issue
3. **既存コード** – `main` ブランチのリポジトリ内容

> 仕様と矛盾する指示は **仕様改訂が添付されない限り拒否** すること。

---

## 3. 成功基準

| #  | 基準                                                                                             |
| -- | ---------------------------------------------------------------------------------------------- |
| S1 | `dotnet build` & `dotnet test` が Windows / macOS / Linux（GitHub Actions）で成功する。                 |
| S2 | `deploy-infra.yml` で **Static Web Apps・Blob Storage・Key Vault・Cosmos DB** がダミー RG へデプロイ成功。     |
| S3 | `BackupService --sync` がローカル **Azurite + Cosmos Emulator** に JSON / Doc を POST し *200 OK* を取得。 |
| S4 | `release-desktop.yml` が署名済みインストーラを生成し、Blob `$web/updates` へアップロード。                             |
| S5 | Static Web Apps 環境でフロントエンドがビルドされ、**Functions 経由で Key Vault シークレット取得 & Cosmos DB 読み書き**が確認できる。  |

---

## 4. エージェント ワークフロー

1. **最新取得** – `main` を pull
2. **仕様解析** – 仕様とリポジトリ差分を比較
3. **計画** – ファイル追加 / 修正 / テスト更新 / ワークフロー調整
4. **実行** – §6 に従い小粒コミット
5. **プッシュ** – `ai/<ticket‑id>` ブランチ → 自動生成リリースノート付き PR
6. **レビュー対応** – コメントに 24 h 以内に応答

> ⏱ **CPU 上限**: アクションごと 30 min。外部通信は GitHub・Azure CLI のみ。

---

## 5. コーディング標準

* **言語** – C# 11+、Nullable 有効、`async`/`await` 徹底
* **スタイル** – `.editorconfig` + `dotnet format` 合格
* **構成** – 名前空間 = フォルダ (`App.Pages`, `App.Data`, `Infra.Cosmos` 等)
* **ロギング** – Serilog、`Console.WriteLine` 禁止
* **シークレット** – Key Vault からマネージド ID 経由で取得し、ハードコード禁止

---

## 6. コミット & PR 規約

* **ブランチ** – `ai/{issue-number}-{slug}`
* **コミット** – Conventional Commits (`feat:`, `fix:`, `docs:`, `ci:`, `infra:` 等)
* **PR タイトル** – `[AI] <scope>: summary`
* **PR 本文** – 自動生成チェックリスト

  ```markdown
  - [ ] Builds pass (`dotnet build`)
  - [ ] Tests pass (`dotnet test`)
  - [ ] Static Apps / Functions build green
  - [ ] Infra deploy dry‑run green
  - [ ] Spec compliance validated
  ```

---

## 7. ツール規則

| タスク                | コマンド                                                                             | 主要フラグ例                                      |
| ------------------ | -------------------------------------------------------------------------------- | ------------------------------------------- |
| ビルド                | `dotnet build`                                                                   | `-c Release`                                |
| テスト                | `dotnet test`                                                                    | –                                           |
| デスクトップ公開           | `dotnet publish`                                                                 | `-c Release -r win-x64 --self-contained`（例） |
| Static Apps 起動     | `swa start`                                                                      | `--run "dotnet watch run"`                  |
| インフラ展開             | `az deployment group create`                                                     | 手動 RG 指定。自動承認なし                             |
| Blob 同期            | `az storage blob upload-batch`                                                   | `--overwrite --auth-mode login`             |
| Cosmos Emulator CI | `docker run -p 8081:8081 mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator` | –                                           |

---

## 8. ガードレール

* 1 PR の新規依存ライブラリ追加は **3 件以下**
* Preview / Experimental NuGet 禁止（仕様改訂時のみ許可）
* Azure コストが **¥500/月超** 増の変更は `cost:review` ラベルで人間承認
* 行カバレッジ **70 %+** (`coverlet`)
* Key Vault シークレット値をログ・PR に出さない

---

## 9. MVP 終了基準

1. テスターが **clone → build → desktop run → static apps run → sync** を、シークレット登録以外の手作業なしで実施できる
2. CI/CD & デプロイ ワークフローが全緑。README バッジも通過
3. Azure コスト (Static Apps Free+、Function Consumption、Cosmos Free tier、Blob Hot LRS) 合算で **月 ¥800 以下**

> **EOF** — 仕様変更時は必ず本ファイルも更新すること。
