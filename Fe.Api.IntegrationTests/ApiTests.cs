using System.Net.Http.Json;
using System.Text.Json;
using Fe.Api.Features.CreateOrder;
using Fe.Api.Features.FetchOrderById;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fe.Api.IntegrationTests;

public class ApiTests
{
    private readonly VerifySettings _verifySettings;

    public ApiTests()
    {
        _verifySettings = new VerifySettings();
        _verifySettings.ScrubInlineGuids();
        _verifySettings.ScrubMember("requestId");
        _verifySettings.ScrubMember("traceId");
        _verifySettings.UseDirectory("snapshots");
    }

    [Fact]
    public async Task FetchOrderById()
    {
        // ARRANGE
        CreateOrderRequest request = new()
        {
            CustomerName = "Mr Nobody",
        };
        
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var response = await client.PostAsync("/api/orders", JsonContent.Create(request));
        var json = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<FetchOrderByIdResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
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
            CustomerName = "Mr Nobody",
        };
        
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        // ACT
        var actual = await client.PostAsync("/api/orders", JsonContent.Create(request));
        
        // ASSERT
        await Verify(actual, _verifySettings);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("x")]
    public async Task GivenCustomerNameIsEmpty_WhenCreateOrder_ThenBadRequest(string customerName)
    {
        // ARRANGE
        CreateOrderRequest request = new()
        {
            CustomerName = customerName,
        };
        
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        // ACT
        var actual = await client.PostAsync("/api/orders", JsonContent.Create(request));
        
        // ASSERT
        await Verify(actual, _verifySettings).UseParameters(customerName);
    }
    
    [Fact]
    public async Task GivenOrderIdNotExist_WhenFetchOrder_ThenNotFound()
    {
        // ARRANGE
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        // ACT
        var actual = await client.GetAsync($"/api/orders/{Guid.NewGuid()}", CancellationToken.None);
        
        // ASSERT
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task ThrowServerError()
    {
        // ARRANGE
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        // ACT
        var actual = await client.GetAsync("/api/others/problematic-call");
        
        // ASSERT
        await Verify(actual, _verifySettings);
    }
}
