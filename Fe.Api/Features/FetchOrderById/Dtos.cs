using Microsoft.AspNetCore.Mvc;

namespace Fe.Api.Features.FetchOrderById;

public record FetchOrderByIdRequest
{
    [FromRoute]
    public required Guid OrderId { get; init; }
}


public record FetchOrderByIdResponse
{
    public required Guid OrderId { get; init; }
    public required string CustomerName { get; init; }
    public required DateTimeOffset DateOrdered { get; init; }
}
