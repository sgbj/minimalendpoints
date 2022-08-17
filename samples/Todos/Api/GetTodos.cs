using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Sgbj.MinimalEndpoints;

namespace Todos.Api;

public record GetTodosEndpoint(TodoDbContext Db, IMapper Mapper) : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/todos", ([AsParameters] GetTodosEndpoint endpoint, CancellationToken cancellationToken) => endpoint.HandleAsync(cancellationToken));

    public async Task<Ok<List<GetTodosResponse>>> HandleAsync(CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await Db.Todos.ProjectTo<GetTodosResponse>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken));
    }
}

public record GetTodosResponse(int Id, string Name, bool IsComplete);

public class GetTodosProfile : Profile
{
	public GetTodosProfile()
	{
		CreateMap<Todo, GetTodosResponse>();
	}
}
