# このドキュメントには実装の指示に対し、よりスムーズに実装を行うための改善点を記載してください。

## 実装との差異
- Microsoft Entra External ID を用いた **ユーザー登録** の仕組みを実装済み【F:src/Application/UserService.cs†L17-L28】【F:src/Functions/UserFunctions.cs†L18-L39】
- **フォーム回答項目**の追加・削除・編集 UI が存在せず、`FormEditorService` にも追加/削除メソッドがない【F:docs/use-cases.md†L7-L7】
- **フォーム回答結果**のCSV出力やHTMLメール送信機能が未実装【F:docs/use-cases.md†L13-L14】【F:docs/use-cases.md†L21-L22】
- **Azure FunctionsのWarm up** 関数が無く、公開フォームからの起動待ち対策が不足【F:docs/use-cases.md†L17-L18】
- **運用ユーザー設定**やログ管理ページのフィルタ機能が未実装【F:docs/use-cases.md†L24-L27】
- GDPR対応として暗号化に加え、利用者同意日時の保存と回答削除APIを実装済み【F:src/Domain/Entities.cs†L37-L43】【F:src/Functions/FormFunctions.cs†L81-L94】
- インフラはBicepで定義しているが、Azure無料枠の利用制限を検証する仕組みが無い【F:docs/requirements.md†L17-L18】

## 実装箇所
- フォーム自動保存とデフォルト項目生成: `FormEditorService`【F:src/Application/FormEditorService.cs†L20-L87】
- 公開・プレビューHTML生成と入力データの一時保存スクリプト: `FormPublishService`【F:src/Application/FormPublishService.cs†L41-L76】
- フォーム保存・プレビュー・公開API: `FormFunctions`【F:src/Functions/FormFunctions.cs†L52-L98】
- 活動ログ取得API: `ActivityLogService` と `LogFunctions`【F:src/Application/ActivityLogService.cs†L18-L27】【F:src/Functions/LogFunctions.cs†L18-L26】
- ユーザー登録とロール更新API: `UserService` と `UserFunctions`【F:src/Application/UserService.cs†L17-L40】【F:src/Functions/UserFunctions.cs†L18-L37】
