## 実装

各実装において各テスト項目の実施と、`docs/ai-review.md`へのよりスムーズな実装のための指示改善提案の記載を行う。

- `docs/architectures/application-design.md` を参照し、各プロジェクトとサンプル UnitTest を追加する
- `docs/ci.md` を参照し、ymlファイルを配置
- `docs/development-guideline.md` を参照し、Serilogや.editorconfigを設定
- `docs/azure-setup.md` を参照し、Bicep/ARM でサービスを定義
- `docs/docker-setup.md` を参照し、docker関連のセットアップを行う
- `docs/data-design.md` を参照し、スキーマを作成
- `docs/entities.md` を参照し、ドメイン層を設計する
- `docs/api-spec.md` と`docs/architectures/application-design.md` に基づいてAzure functionsのAPIを実装
- `docs/ui-design.md` に記載のある全画面を作成。コンポーネントは`docs/components.md` に基づく
- `docs/use-cases.md` を参照し、現在の実装の不足分を実現できるよう設定を行う

## テスト項目

- `dotnet test`が通る事を確認
- nektos/actでテストが通るか確認
 - `dotnet format`で差分がない事を確認
- `dotnet build`が通る事を確認
