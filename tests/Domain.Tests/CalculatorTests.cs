using AstroForm.Domain;

namespace Domain.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        var result = Calculator.Add(2, 3);
        Assert.Equal(5, result);
    }
}
