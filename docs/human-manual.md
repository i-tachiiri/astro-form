下記を実施し、プロジェクトのひな形を作成してください。
- docs/architectures/application-design.md を参照し、各プロジェクトとサンプル UnitTest を追加する
- docs/development-guideline.md を参照し、Serilogや.editorconfigを設定
  - UnitTestにはSmokeTestを追加し、起動時の例外が出ないかチェックできるようにしてください
  - Blazorのプロジェクトにはiconやfaviconのpngが含まれるが、バイナリがあるとPRが作成できないのでSVG等に置き換えて対応 
  
下記を実施し、開発環境を整えて下さい。
- docs/azure-setup.md を参照し、Bicep/ARM でサービスを定義
- docs/docker-setup.md を参照し、docker関連のセットアップを行う
- docs/ci.md を参照し、ymlファイルを配置
  - `ci-error.md` を参照し、CI時のエラーが発生しないか確認     

下記を実施し、データ設計を行ってください。
- docs/data-design.md を参照し、スキーマを作成
- docs/entities.md を参照し、ドメイン層を設計する
- docs/security.md を参照し、データの扱いにおいて必要な実装があれば行ってください
  
下記を実施し、Azure functionsの実装を行ってください。
- docs/api-spec.md とdocs/architectures/application-design.md に基づいてAzure functionsのAPIを実装
- docs/ui-design.md に記載のある全画面を作成
- 各画面にdocs/components.md に基づいてコンポーネントを実装
  
下記を実施し、現在の実装と、ユースケースや要求との差異が発生していれば修正してください。
- docs/use-cases.md を参照し、現在の実装の不足分をdocs/ai-review.mdに記載
- docs/requirements.md を参照し、要求に対する不足をdocs/ai-review.mdに記載
