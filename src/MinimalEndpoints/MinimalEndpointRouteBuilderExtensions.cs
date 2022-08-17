using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Sgbj.MinimalEndpoints;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder;

public static class MinimalEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapMinimalEndpoints(this IEndpointRouteBuilder endpoints, params Type[] markerTypes)
    {
        var group = endpoints.MapGroup("/");

        var parameterTypes = new[] { typeof(IEndpointRouteBuilder) };
        var parameters = new[] { endpoints };

        foreach (var type in markerTypes.SelectMany(t => t.Assembly.GetTypes()))
        {
            if (type.IsAssignableTo(typeof(IMinimalEndpoint)))
            {
                type.GetMethod(nameof(IMinimalEndpoint.Map), BindingFlags.Public | BindingFlags.Static, parameterTypes)?.Invoke(null, parameters);
            }
        }

        return group;
    }
}
