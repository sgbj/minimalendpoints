using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Sgbj.MinimalEndpoints;

namespace Todos.Api;

public record CreateTodoEndpoint(CreateTodoRequest Request, TodoDbContext Db, IValidator<CreateTodoRequest> Validator, IMapper Mapper) : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/todos", ([AsParameters] CreateTodoEndpoint endpoint, CancellationToken cancellationToken) => endpoint.HandleAsync(cancellationToken));

    public async Task<Results<CreatedAtRoute<CreateTodoResponse>, ValidationProblem>> HandleAsync(CancellationToken cancellationToken)
    {
        var validationResult = await Validator.ValidateAsync(Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todo = Mapper.Map<Todo>(Request);

        Db.Todos.Add(todo);
        await Db.SaveChangesAsync(cancellationToken);

        var response = Mapper.Map<CreateTodoResponse>(todo);

        return TypedResults.CreatedAtRoute(response, "GetTodo", new { todo.Id });
    }
}

public record CreateTodoRequest(string Name);

public record CreateTodoResponse(int Id, string Name, bool IsComplete);

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public class CreateTodoProfile : Profile
{
    public CreateTodoProfile()
    {
        CreateMap<CreateTodoRequest, Todo>();
        CreateMap<Todo, CreateTodoResponse>();
    }
}
