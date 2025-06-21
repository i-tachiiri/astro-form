namespace Presentation.Shared;

public record ActivityLogDto(long Id, DateTime Timestamp, string? UserId, Guid? FormId, string ActionType, string? Details);
