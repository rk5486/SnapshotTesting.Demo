using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests;

public class ApiTests
{
    private readonly VerifySettings _verifySettings;

    public ApiTests()
    {
        _verifySettings = new VerifySettings();
        _verifySettings.ScrubInlineGuids();
        _verifySettings.UseDirectory("snapshots");
    }

    [Fact]
    public async Task FetchOrderById()
    {
        // ARRANGE
        CreateOrderRequest request = new()
        {
            CustomerName = "John Doe",
        };
        
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var response = await client.PostAsync("/api/orders", JsonContent.Create(request));
        var json = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<OrderResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        // ACT
        var actual = await client.GetAsync($"/api/orders/{orderResponse!.OrderId}", CancellationToken.None);
        
        // ASSERT
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task CreateOrder()
    {
        // ARRANGE
        CreateOrderRequest request = new()
        {
            CustomerName = "John Doe",
        };
        
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        // ACT
        var actual = await client.PostAsync("/api/orders", JsonContent.Create(request));
        
        // ASSERT
        await Verify(actual, _verifySettings);
    }
}
