## コーディングスタイル

- インデントは 4 スペース
- `{` は常に改行後に配置
- 自動整形：`dotnet format` を使用（CI で実行されます）

## ログ出力方針

- ログはすべて Serilog を使用します
- `Console.WriteLine` の使用は禁止（CA1303 エラー扱い）
- ログレベル：Information / Warning / Error を適切に使い分けること

## 静的解析とフォーマットチェック

- `.editorconfig` に従ったスタイルで書くこと
- GitHub Actions にて `dotnet format` によるチェックが実行されます
