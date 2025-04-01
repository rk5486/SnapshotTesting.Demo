using FastEndpoints;

namespace Fe.Api.Features.Others;

public class ProblematicEndpoint
    : Ep.NoReq.NoRes
{
    public override void Configure()
    {
        Get("/others/problematic-call");
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        throw new ApplicationException("This should never happen");
    }
}
