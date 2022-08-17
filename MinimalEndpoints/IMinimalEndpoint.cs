using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Sgbj.MinimalEndpoints;

public interface IMinimalEndpoint
{
    static abstract IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints);
}
