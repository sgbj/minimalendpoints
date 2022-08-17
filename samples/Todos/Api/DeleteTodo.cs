using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Sgbj.MinimalEndpoints;

namespace Todos.Api;

public record DeleteTodoEndpoint(int Id, TodoDbContext Db, IMapper Mapper) : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete("/todos/{id}", ([AsParameters] DeleteTodoEndpoint endpoint, CancellationToken cancellationToken) => endpoint.HandleAsync(cancellationToken));

    public async Task<Results<Ok<DeleteTodoResponse>, NotFound>> HandleAsync(CancellationToken cancellationToken)
    {
        var todo = await Db.Todos.FindAsync(new object?[] { Id }, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        Db.Todos.Remove(todo);
        await Db.SaveChangesAsync(cancellationToken);

        var response = Mapper.Map<DeleteTodoResponse>(todo);

        return TypedResults.Ok(response);
    }
}

public record DeleteTodoResponse(int Id, string Name, bool IsComplete);

public class DeleteTodoProfile : Profile
{
    public DeleteTodoProfile()
    {
        CreateMap<Todo, DeleteTodoResponse>();
    }
}
