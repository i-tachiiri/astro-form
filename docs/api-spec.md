# API 実装仕様（api-spec.md）

## 認証とシークレット管理

- Cosmos DB への接続情報は Azure Key Vault に保管し、`DefaultAzureCredential` 経由で取得。
- ローカルでは `local.settings.json` または `.env` で代替。

## OpenAPI ドキュメント

- Swashbuckle を使って OpenAPI を自動生成。
- `/openapi.json` で公開される。
- Functions 用の Startup クラスに `AddSwaggerGen()` を実装。

## 関数構成とルーティング

- 各エンドポイントは `HttpTrigger` により個別に定義。
- パスパラメータやクエリパラメータの構造は以下を参照：

```csharp
[Function(\"GetFormById\")]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, \"get\", Route = \"forms/{id}\")] HttpRequest req,
    string id)
