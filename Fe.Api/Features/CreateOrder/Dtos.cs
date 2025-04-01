using FastEndpoints;
using FluentValidation;

namespace Fe.Api.Features.CreateOrder;

public record CreateOrderRequest
{
    public required string CustomerName { get; init; }
}

public class CreateOrderRequestValidator : Validator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MinimumLength(2);
    }
}

public record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
    public required string CustomerName { get; init; }
    public required DateTimeOffset DateOrdered { get; init; }
}
