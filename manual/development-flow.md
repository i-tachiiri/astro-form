## 実装

各実装において各テスト項目の実施と、`docs/ai-review.md`へのよりスムーズな実装のための指示改善提案の記載を行う。

- 下記を実施し、プロジェクトのひな形を作成してください。
  - `docs/architectures/application-design.md` を参照し、各プロジェクトとサンプル UnitTest を追加する
  - `docs/ci.md` を参照し、ymlファイルを配置
  - `docs/development-guideline.md` を参照し、Serilogや.editorconfigを設定
- 下記を実施し、開発環境を整えて下さい。
  - `docs/azure-setup.md` を参照し、Bicep/ARM でサービスを定義
  - `docs/docker-setup.md` を参照し、docker関連のセットアップを行う
- 下記を実施し、データ設計を行ってください。
  - `docs/data-design.md` を参照し、スキーマを作成
  - `docs/entities.md` を参照し、ドメイン層を設計する
- 下記を実施し、Azure functionsの実装を行ってください。
  - `docs/api-spec.md` と`docs/architectures/application-design.md` に基づいてAzure functionsのAPIを実装
  - `docs/ui-design.md` に記載のある全画面を作成。コンポーネントは`docs/components.md` に基づく
- 下記を実施し、現在の実装と、ユースケースや要求との差異が発生していれば修正してください。
  - `docs/use-cases.md` を参照し、現在の実装の不足分を実現できるよう設定を行う
  - `docs/requirements.md` を参照し、要求を実現できているか確認する

## テスト

  - 以下のコマンドでビルド・テスト・コードフォーマットを実行し、ローカル環境で問題がないことを確認する：
    - `dotnet build`
    - `dotnet test`
    - `dotnet format`
  - `nektos/act` を使用し、GitHub Actions のジョブがローカルでも成功することを確認する：

    ```bash
    act -j <ジョブ名> --container-architecture linux/amd64 \
      -P ubuntu-latest=catthehacker/ubuntu:act-latest
    ```
    ※ `-j <ジョブ名>` の `<ジョブ名>` は `.github/workflows/*.yml` の `jobs.<name>` に対応
