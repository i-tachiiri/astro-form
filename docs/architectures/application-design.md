## プロジェクト構成

本アプリケーションは以下の5つのプロジェクトに分かれる：

- `Presentation.Client` : Blazor WebAssembly による UI
- `Presentation.Shared` : クライアント・API間の DTO
- `Api` : Azure Functions ベースの API（Controller 相当）
- `Domain` : 業務ロジック（Entity・値オブジェクト・サービス）
- `Infra` : Cosmos DB や Blob Storage との接続処理
