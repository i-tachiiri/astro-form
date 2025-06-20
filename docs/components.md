# 画面一覧 & コンポーネント

## 1 📋 画面概要

| 画面ID    | 画面名    | 説明   |
| ------- | ------------- | ----------------------------------------- |
| **S01** | Login         | Microsoft Entra External ID を利用したサインインページ |
| **S02** | FormList      | 作成済みフォームを一覧表示（フォーム管理情報を含む）                |
| **S03** | FormEditor    | 選択したフォームの編集・公開を行うページ                      |
| **S04** | FormPreview   | フォームデータから HTML を生成し /preview/ 配下に表示       |
| **S05** | PostSubmit    | フォーム送信後に遷移するサンクスページ（デフォルトまたは任意 URL）       |
| **S06** | LogViewer     | 占い師またはフォーム単位のログを閲覧（運用・開発担当限定）             |
| **S07** | Account       | プライバシーポリシーへのリンク、ログアウト、アカウント削除             |
| **S08** | PrivacyPolicy | プライバシーポリシーを表示する静的ページ                      |
| **S09** | AccountDelete | アカウント削除を二段階確認の上で実行するページ                   |

## 2 🧩 コンポーネントカタログ (Fluent UI ベース)

### 2.1 レイアウト

* `AppShell`
* `Header`
* `Footer`
* `SideNav`

### 2.2 ナビゲーション

* `Breadcrumb`
* `CommandBar`
* `NavMenuItem`

### 2.3 コンテンツ

#### Form（フォーム関連）

* `FormList`
* `FormListItem`
* `FormEditor`
* `FormPreview`

#### Log（ログ関連）

* `LogTable`
* `LogFilterBar`

#### Account（アカウント関連）

* `AccountCard`
* `DeleteAccountDialog`

### 2.4 コントロール

* `DatePickerField`
* `TimePickerField`
* `PlaceAutocomplete`
* `SaveButton`

### 2.5 ユーティリティ

* `LoadingSpinner`
* `Toast`
* `ConfirmDialog`

## 3 🔗 コンポーネント ⇄ 画面対応表

| コンポーネント                                                                               | 使用画面          |
| ------------------------------------------------------------------------------------- | ------------- |
| `AppShell`, `Header`, `Footer`, `SideNav`                                             | **S01 – S09** |
| `FormList`, `FormListItem`                                                            | **S02**       |
| `FormEditor`, `DatePickerField`, `TimePickerField`, `PlaceAutocomplete`, `SaveButton` | **S03**       |
| `FormPreview`                                                                         | **S04**       |
| `LogTable`, `LogFilterBar`                                                            | **S06**       |
| `AccountCard`                                                                         | **S07**       |
| `DeleteAccountDialog`, `ConfirmDialog`                                                | **S09**       |
| `LoadingSpinner`, `Toast`                                                             | 共通（全画面）       |

> **デザイン注記** : すべての画面はレスポンシブ対応で、Fluent UI のレイアウトプリミティブ (`Stack`, `Grid`) を利用してモバイル (単一カラム) からデスクトップ (2ペイン) まで最適化します。
