# 🔁 Continuous Integration（CI）構成ガイド

> 本プロジェクトでは GitHub Actions を用いて、自動ビルド・テスト・フォーマットチェックを行います。

## 📁 ワークフローファイル

GitHub Actions の設定ファイルは`.github/workflows/test-and-build.yml`に配置されています：

---

## ⚙ 実行タイミング

以下のイベントで自動実行されます：

- `push` または `pull_request` による変更
- 対象ブランチ：`main`, `develop`, `feature/**`
- 手動実行（`workflow_dispatch`）にも対応

---

## 🔍 実行内容の詳細

| ステップ           | 内容                                                                 |
|------------------|----------------------------------------------------------------------|
| Checkout         | リポジトリを取得（`actions/checkout`）                                          |
| .NET Install     | 必要な SDK をインストール（例: .NET 8）                                       |
| Format Check     | `dotnet format --verify-no-changes` によりコード整形ルールの逸脱を検出                    |
| Build            | `dotnet build --configuration Release` によるビルド                          |
| Test             | `dotnet test --collect:\"XPlat Code Coverage\"` による単体テスト＋カバレッジ取得         |
| Coverage Check   | `coverlet` によるカバレッジ評価（70%以上で成功）                                  |

---

## 📊 カバレッジポリシー

`coverlet` を使用して行カバレッジを取得し、以下の閾値を満たすことを必須とします：

- **最低カバレッジ率**：70%
- 閾値未満の場合、ビルドは失敗となります

---

## 📌 注意事項

- **Console.WriteLine は禁止**：Serilog によるロギングに統一。`CA1303` 等のルールがエラーとして扱われます。
- **CI に失敗した PR はマージ不可**：レビュー時にステータスチェックが有効になります。
- **`.editorconfig` に違反するコードは reject されます。**


