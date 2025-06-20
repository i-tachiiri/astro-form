# 🚀 Astro Form テンプレート — リポジトリ概要

> **対象読者**: 本リポジトリを操作・拡張する AI／人間コントリビュータ
> **目的**: Azure Static Apps（Functions, Blob Storage, Key Vault, Cosmos DB）構成の最小コスト SaaS テンプレートを、AI ペアコーディングで素早く立ち上げる。

---

## 📂 ディレクトリ構成（抜粋）

| パス                   | 役割・内容                                          |
| -------------------- | ---------------------------------------------- |
| `/AGENTS.md`         | **エージェント実行規約**。AI が守るべきミッション・ワークフロー・ガードレールを定義。 |
| `/src/`              | アプリケーション本体（Functions / BFF など）。  |
| `/infra/`            | IaC スクリプト（Bicep / ARM / Terraform など想定）。       |
| `/docs/`             | 仕様・設計ドキュメント一式。詳細は下記参照。                         |
| `/tests/`            | Unit / Integration テスト群。                       |
| `.github/workflows/` | CI/CD（build / test / deploy / release）。        |

### `/docs` 配下

| ファイル              | 概要（AI が参照すべきポイント）                  |
| ----------------- | ---------------------------------- |
| `components.md`   | 画面や Functions の **再利用コンポーネント** 定義。 |
| `data-design.md`  | データモデル・ER 図・Cosmos DB コレクション設計。    |
| `entities.md`     | ドメイン層のエンティティ仕様（C# クラスとのマッピング）。     |
| `requirements.md` | ビジネス要求・課題・目的。**最初に読むこと**。          |
| `security.md`     | 認証・認可・暗号化・脆弱性対策などの非機能要件。           |
| `ui-design.md`    | 画面遷移図・レイアウト・レスポンシブ指針。              |
| `use-cases.md`    | ユースケース＆ユーザーストーリー。                  |

> 🔖 **ドキュメント更新の原則**
>
> * `requirements.md` → 他ドキュメント → 実装コードの順で整合性を保つ。
> * 仕様変更時は `/AGENTS.md` のガードレールも同時に見直すこと。

---

## 🛠️ AI 向け作業フロー

1. **Read → Plan → Modify** の 3 ステップを厳守。
2. 仕様矛盾を検出した場合は PR ではなく **Issue** を生成し、`spec:conflict` ラベルを付与。
3. コード変更は **Conventional Commits** ＋ 小粒 PR（§6 in AGENTS.md）で提出。
4. Infra/CI 変更はコスト増チェックを自動実行。🪙 +¥500/月 を超える場合 `cost:review` ラベル必須。

---

## ⚡ クイックスタート（人間開発者向け）

```bash
# 1. clone & dev container
gh repo clone <fork>
cd <repo>
# 推奨: Dev Containers で開いて必要ツールを自動インストール

# 2. ローカル実行
make run-desktop      # Avalonia
make run-functions    # Azure Functions + Azurite + Cosmos Emulator

# 3. Static Apps (front-end) 開発
swa start --run "dotnet watch run"
```

> 💡 **Tips**: `.env.sample` をコピーして `.env` を作成すると、ローカル開発で Key Vault 依存をバイパスできます。

---

## 📜 ライセンス

MIT License

---

### 🤝 コントリビュート

AI／人間ともに歓迎！ まずは Issue か Discussion で声を掛けてください。PR には **チェックリストの自己承認** が必要です。
