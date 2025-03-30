using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests;

public class Tests
{
    [Fact]
    public async Task GetOrderById()
    {
        // ARRANGE
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        // ACT
        var actual = await client.GetAsync($"/api/orders/{Guid.NewGuid()}", CancellationToken.None);
        
        // ASSERT
        Assert.True(actual.IsSuccessStatusCode);
    }
}
