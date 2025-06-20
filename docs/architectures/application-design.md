## プロジェクト構成

本アプリケーションは以下の5つのプロジェクトに分かれる：

- `Presentation.Client` : Blazor WebAssembly による UI
- `Presentation.Shared` : クライアント・API間の DTO
- `Api` : Azure Functions ベースの API（Controller 相当）
- `Domain` : 業務ロジック（Entity・値オブジェクト・サービス）
- `Infra` : Cosmos DB や Blob Storage との接続処理

## Functions API の構成

- Azure Functions で構成され、HTTPトリガーで以下の CRUD エンドポイントを提供する。
- 各エンドポイントは Cosmos DB の該当リポジトリ経由で読み書きされる。

| エンドポイント   | 説明                   |
|------------------|------------------------|
| `/forms`         | フォームのCRUD             |
| `/questions`     | 質問のCRUD               |
| `/answers`       | 回答の登録と取得           |
| `/users`         | ユーザーの登録・照会         |
| `/logs`          | 操作ログの記録・取得         |
