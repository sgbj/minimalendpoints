using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Sgbj.MinimalEndpoints;

namespace Todos.Api;

public record GetTodoEndpoint(int Id, TodoDbContext Db, IMapper Mapper) : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/todos/{id}", ([AsParameters] GetTodoEndpoint endpoint, CancellationToken cancellationToken) => endpoint.HandleAsync(cancellationToken))
                 .WithName("GetTodo");

    public async Task<Results<Ok<GetTodoResponse>, NotFound>> HandleAsync(CancellationToken cancellationToken)
    {
        return await Db.Todos.FindAsync(new object?[] { Id }, cancellationToken) is Todo todo
            ? TypedResults.Ok(Mapper.Map<GetTodoResponse>(todo))
            : TypedResults.NotFound();
    }
}

public record GetTodoResponse(int Id, string Name, bool IsComplete);

public class GetTodoProfile : Profile
{
    public GetTodoProfile()
    {
        CreateMap<Todo, GetTodoResponse>();
    }
}
