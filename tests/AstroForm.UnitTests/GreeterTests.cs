using AstroForm.Domain;

namespace AstroForm.UnitTests;

public class GreeterTests
{
    [Fact]
    public void Greet_ReturnsExpectedMessage()
    {
        var greeter = new Greeter();
        var result = greeter.Greet("UnitTest");
        Assert.Equal("Hello, UnitTest!", result);
    }
}
