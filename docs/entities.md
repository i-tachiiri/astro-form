```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroForm.Models
{
    /// <summary>
    /// ユーザーの役割を定義します。
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 占い師
        /// </summary>
        FortuneTeller,
        /// <summary>
        /// 占い師のアシスタント
        /// </summary>
        Assistant,
        /// <summary>
        /// システム運用担当
        /// </summary>
        Admin
    }

    /// <summary>
    /// フォームの状態を定義します。
    /// </summary>
    public enum FormStatus
    {
        /// <summary>
        /// 下書き
        /// </summary>
        Draft,
        /// <summary>
        /// 公開済み
        /// </summary>
        Published
    }

    /// <summary>
    /// ユーザーエンティティ。占い師、アシスタント、システム運用担当など、システムにログインする全ての関係者を管理します。
    /// </summary>
    public class User
    {
        /// <summary>
        /// ユーザーID (主キー)。Microsoft Entra External IDから払い出される一意の識別子。
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// ユーザーの表示名。
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        /// <summary>
        /// ログインに使用するメールアドレス。
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// ユーザーの役割。
        /// </summary>
        [Required]
        public UserRole Role { get; set; }

        /// <summary>
        /// アカウントの作成日時。
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// アカウント情報の最終更新日時。
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        // ナビゲーションプロパティ
        public virtual ICollection<Form> Forms { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
    }

    /// <summary>
    /// 占い師が作成するフォーム全体の情報を管理するエンティティ。
    /// </summary>
    public class Form
    {
        /// <summary>
        /// フォームID (主キー)。
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 作成者ユーザーのID (外部キー)。
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// フォーム名。
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// フォームの説明文。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ナビゲーションテキスト。
        /// </summary>
        [StringLength(100)]
        public string NavigationText { get; set; }

        /// <summary>
        /// 回答後に遷移するページのURL。
        /// </summary>
        public string ThankYouPageUrl { get; set; }

        /// <summary>
        /// フォームのステータス（下書き or 公開済み）。
        /// </summary>
        [Required]
        public FormStatus Status { get; set; }

        /// <summary>
        /// フォームの作成日時。
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// フォームの最終更新日時。
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // ナビゲーションプロパティ
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<FormItem> FormItems { get; set; }
        public virtual ICollection<FormSubmission> FormSubmissions { get; set; }
    }
    
    /// <summary>
    /// フォームを構成する個々の質問項目を管理するエンティティ。
    /// </summary>
    public class FormItem
    {
        /// <summary>
        /// 項目ID (主キー)。
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// この項目が属するフォームのID (外部キー)。
        /// </summary>
        [Required]
        public Guid FormId { get; set; }

        /// <summary>
        /// 項目の種類（テキスト、メール、日付など）。
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        /// <summary>
        /// フォームに表示される項目名（ラベル）。
        /// </summary>
        [Required]
        [StringLength(150)]
        public string Label { get; set; }

        /// <summary>
        /// 入力欄に表示されるプレースホルダーや補足説明文。
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// 入力値の検証ルール (JSON形式でシリアライズして格納)。
        /// </summary>
        public string ValidationRules { get; set; }

        /// <summary>
        /// フォーム内での表示順序。
        /// </summary>
        [Required]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// デフォルト項目かカスタム項目かを区別するフラグ。
        /// </summary>
        [Required]
        public bool IsDefault { get; set; }

        // ナビゲーションプロパティ
        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }
    }

    /// <summary>
    /// エンドユーザーが送信した回答データを管理するエンティティ。
    /// </summary>
    public class FormSubmission
    {
        /// <summary>
        /// 回答ID (主キー)。
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 回答対象のフォームID (外部キー)。
        /// </summary>
        [Required]
        public Guid FormId { get; set; }

        /// <summary>
        /// 回答データ (JSON形式でシリアライズして格納)。PIIとして扱います。
        /// </summary>
        [Required]
        public string Answers { get; set; }

        /// <summary>
        /// フォームが送信された日時。
        /// </summary>
        public DateTime SubmittedAt { get; set; }
        
        /// <summary>
        /// 送信元情報 (IPアドレスなど、JSON形式でシリアライズして格納)。
        /// </summary>
        public string SubmitterInfo { get; set; }

        // ナビゲーションプロパティ
        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }
    }

    /// <summary>
    /// システムの操作やイベントの履歴を記録するエンティティ。
    /// </summary>
    public class ActivityLog
    {
        /// <summary>
        /// ログID (主キー)。
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// イベントが発生した日時。
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 操作を行ったユーザーのID (外部キー、システムイベントの場合はnull)。
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// イベントが関連するフォームのID (任意)。
        /// </summary>
        public Guid? FormId { get; set; }

        /// <summary>
        /// イベントの種類。
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ActionType { get; set; }

        /// <summary>
        /// イベントに関する追加情報 (JSON形式でシリアライズして格納)。
        /// </summary>
        public string Details { get; set; }

        // ナビゲーションプロパティ
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}

```
