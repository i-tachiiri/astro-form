using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroForm.Domain.Entities
{
    public enum UserRole
    {
        FortuneTeller,
        Assistant,
        Admin
    }

    public enum FormStatus
    {
        Draft,
        Published
    }

    public class User
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public DateTime ConsentGivenAt { get; set; }

        public virtual ICollection<Form> Forms { get; set; } = new List<Form>();
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }

    public class Form
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(100)]
        public string? NavigationText { get; set; }

        public string? ThankYouPageUrl { get; set; }

        [Required]
        public FormStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
        public virtual ICollection<FormItem> FormItems { get; set; } = new List<FormItem>();
        public virtual ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();
    }

    public class FormItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid FormId { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Label { get; set; } = string.Empty;

        public string? Placeholder { get; set; }

        public string? ValidationRules { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [ForeignKey(nameof(FormId))]
        public virtual Form? Form { get; set; }
    }

    public class FormSubmission
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid FormId { get; set; }

        [Required]
        public string Answers { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }

        [Required]
        public DateTime ConsentGivenAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string? SubmitterInfo { get; set; }

        [ForeignKey(nameof(FormId))]
        public virtual Form? Form { get; set; }
    }

    public class ActivityLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string? UserId { get; set; }

        public Guid? FormId { get; set; }

        [Required]
        [StringLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public string? Details { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}
