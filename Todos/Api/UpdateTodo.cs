using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Sgbj.MinimalEndpoints;

namespace Todos.Api;

public record UpdateTodoEndpoint(int Id, UpdateTodoRequest Request, TodoDbContext Db, IValidator<UpdateTodoRequest> Validator, IMapper Mapper, IMediator Mediator) : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut("/todos/{id}", ([AsParameters] UpdateTodoEndpoint endpoint, CancellationToken cancellationToken) => endpoint.HandleAsync(cancellationToken));

    public async Task<Results<Ok<UpdateTodoResponse>, NotFound, ValidationProblem>> HandleAsync(CancellationToken cancellationToken)
    {
        var validationResult = await Validator.ValidateAsync(Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todo = await Db.Todos.FindAsync(new object?[] { Id }, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        var isComplete = !todo.IsComplete && Request.IsComplete;

        Mapper.Map(Request, todo);

        await Db.SaveChangesAsync(cancellationToken);

        if (isComplete)
        {
            await Mediator.Publish(new TodoCompletedNotification(todo.Id), cancellationToken);
        }

        var response = Mapper.Map<UpdateTodoResponse>(todo);

        return TypedResults.Ok(response);
    }
}

public record UpdateTodoRequest(string Name, bool IsComplete);

public record UpdateTodoResponse(int Id, string Name, bool IsComplete);

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public class UpdateTodoProfile : Profile
{
    public UpdateTodoProfile()
    {
        CreateMap<UpdateTodoRequest, Todo>();
        CreateMap<Todo, UpdateTodoResponse>();
    }
}
