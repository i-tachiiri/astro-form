## 業務手順

### 占い師・占い師のアシスタント

- 占い師の**ユーザー登録**を行ってもらう
- **フォーム情報**と**デフォルトの回答項目**が設定されたフォーム作成画面へ進む
- ユーザーは**フォーム回答項目**の追加・削除・編集、**カスタム回答項目**の**回答プロパティ**の設定、**フォーム情報**の変更を行う
  - 編集中には**フォームの自動保存**がされる
  - ユーザーは**フォームの手動保存**ができる
- **フォームのプレビュー**でフォームを確認する
- **フォームの公開**をする
- フォーム一覧画面で**フォーム管理情報**を表示する
- **フォーム回答結果**を確認する
  - CSVダウンロードを可能にする

 ### ユーザー
 - **フォームの公開**で配置されたHTMLへアクセスする
   - アクセス時点で**Azure FunctionsのWarm up**を行う
- フォームを入力する
   - **入力データの一時保存**を行う
- フォームに回答する
  - 回答結果をHTMLメールで送る

### システム運用担当
- 通常のユーザーと同様にユーザー登録
- システム管理者が**運用ユーザー設定**をする
- **ログ管理ページ**を開く

## 用語
- **ユーザー登録** : Microsoft Entra External IDを利用し、IDを登録してもらう事 
- **フォーム情報** : 一意のID、フォーム名、フォーム説明文、ナビゲーションテキスト、回答後ページ
- **既定のフォームコントロール** : 生まれた年・月・日はカレンダーピッカー、生まれた時・分はタイムピッカー、出生地は Places API の Autocomplete/Details で検索して取得する
- **デフォルトの回答項目** : 氏、名、メールアドレス、生まれた年・月・日、生まれた時・分、生まれた場所の国・都道府県・市町村
- **フォーム回答項目** : ユーザーが設定した回答項目。必須項目はないが、1つ以上必要
- **カスタム回答項目** : ユーザーが任意で追加できる項目。メールアドレス、年月日、時分、国、都道府県、市町村、テキスト、数値、自由入力欄
- **回答プロパティ** : 各プロパティのID、項目名、バリデーションの種類、補足説明
- **フォームの自動保存** : `blur`イベントで入力データをCosmos DBに保存、`setTimeout`で10秒入力がなかった場合にCosmos DBへ保存する事を指す。
- **フォームの手動保存** : 「下書き保存」ボタンを押し、フォームの設定項目を保存する事
- **フォーム管理情報** : フォームの名前、編集画面へのリンク、削除用ボタン、公開⇔下書きの変更ボタンを指す。公開されているフォームは回答結果ページへのリンク、公開フォームへのリンクも含む。
- **フォームの公開** : 入力フォームのデータを元にHTMLを生成し、公開フォルダ配下に配置する事。HTMLのファイル名はフォームIDで定義され、更新時は上書きされる
- **フォームのプレビュー** : 入力フォームのデータを元にHTMLを生成し、プレビュー用フォルダ配下に配置し、アクセスする事。HTMLファイルはプレビュー後に削除
- **フォーム回答結果** : フォームへの回答結果の一覧を表示したもの
- **Azure FunctionsのWarm up** : フォームの起動ログだけを残す関数を実行する事。Azure Functionsは無料枠だと常時稼働していないため、先にこれを実行してユーザーを待たせないようにする
- **入力データの一時保存** : `blur`イベントで、ローカルストレージに各プロパティのIDと入力内容をセットで保存する事。再度ページを開いた際に、各項目に値を自動入力する。
- **運用ユーザー設定** : システム管理者がMicrosoft Entra External IDで運用担当のロールを付与する事

## スキーマ

```yaml
# ユーザー情報です。
container: user
document:
  "id": "..."  # sub
  "sub": "..."
  "email": "user@example.com"
  "display_name": "伊藤"
  "registered_at": "2024-06-22T10:00:00Z"
  "role": "teller"

# フォームのメタ情報です。
container: form
document:
  id : "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
  form_id: "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
  name : "フォーム名"
  description : "占い用のフォームです"
  next_page_url : "/thank-you.html"
  created_at :  "2024-01-01T12:00:00Z"
  updated_at :  "2024-01-01T12:00:00Z"
  published: false

container:

# ユーザー別のデフォルトのフォーム項目です。  
container: default_properties
sub: "..."
properties:
  first_name:
    id: "..."
    type: text
    label: "名字"
    required: true
    order: 1
    tooltip: "全角で入力してください"
    visible: true
    editable: true

  last_name:
    id: "..."
    type: text
    label: "名前"
    required: true
    order: 2
    tooltip: "姓に続く名前を入力してください"
    visible: true
    editable: true

  mail_address:
    id: "..."
    type: mail
    label: "メールアドレス"
    required: true
    order: 3
    tooltip: "有効なメールアドレスを入力してください"
    visible: true
    editable: true
    validation_preset: email

  birth_date:
    id: "..."
    type: date
    label: "生年月日"
    required: true
    order: 4
    tooltip: "カレンダーから選択してください"
    visible: true
    editable: true

  birth_time:
    id: "..."
    type: date
    label: "出生時刻"
    required: true
    order: 5
    tooltip: "不明な場合は省略できます"
    visible: true
    editable: true

  birth_place:
    id: "..."
    type: place
    label: "生まれた場所"
    required: true
    order: 6
    tooltip: "市区町村まで入力してください（例：東京都港区）"
    visible: true
    editable: true

  accept_policy:
    id: "..."
    type: checkbox
    label: "プライバシーポリシーへの同意"
    link: "https://exampla.com"
    required: true
    order: 7
    tooltip: "チェックを入れないと送信できません"
    visible: true
    editable: false

# ユーザー別のカスタムのフォーム項目です。  
container: custom_properties
document:
  text:
    id: "..."
    type: text
    label: ""
    link: ""
    required: false
    order: 0
    tooltip: ""
    visible: true
    editable: true  
  date:
    id: "..."
    type: date
    label: ""
    link: ""
    required: false
    order: 0
    tooltip: ""
    visible: true
    editable: true  
  time:
    id: "..."
    type: time
    label: ""
    link: ""
    required: false
    order: 0
    tooltip: ""
    visible: true
    editable: true    
  place:
    id: "..."
    type: place
    label: ""
    link: ""
    required: false
    order: 0
    tooltip: ""
    visible: true
    editable: true    
  
  checkbox:
    id: "..."
    type: checkbox
    label: ""
    link: ""
    required: false
    order: 0
    tooltip: ""
    visible: true
    editable: true    
```
