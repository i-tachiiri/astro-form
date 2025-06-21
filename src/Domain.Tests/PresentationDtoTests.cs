using System;
using Presentation.Shared;
using Xunit;

namespace Domain.Tests;

public class PresentationDtoTests
{
    [Fact]
    public void ActivityLogDto_HoldsValues()
    {
        var dto = new ActivityLogDto(1, DateTime.UtcNow, "u", Guid.NewGuid(), "A", "d");
        Assert.Equal(1, dto.Id);
        Assert.Equal("A", dto.ActionType);
    }

    [Fact]
    public void FormDto_HoldsValues()
    {
        var id = Guid.NewGuid();
        var dto = new FormDto(id, "t");
        Assert.Equal(id, dto.Id);
        Assert.Equal("t", dto.Title);
    }
}
