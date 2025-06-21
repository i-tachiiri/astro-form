using AstroForm.Domain.Entities;
using Xunit;

namespace Domain.Tests;

public class EntityTests
{
    [Fact]
    public void FormInitialization_DefaultsAreSet()
    {
        var form = new Form();
        Assert.NotNull(form.FormItems);
        Assert.NotNull(form.FormSubmissions);
    }
}
