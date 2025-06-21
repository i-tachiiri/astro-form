using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AppStarts_ReturnsNotFoundForRoot()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
