# Sgbj.MinimalEndpoints

A simple and unopinionated way to map minimal API endpoints.

Map endpoints using the routing APIs you're already familiar with.

## Installing this package

This package is available on [NuGet](https://www.nuget.org/packages/Sgbj.MinimalEndpoints).

Package Manager Console

```
Install-Package Sgbj.MinimalEndpoints
```

.NET CLI

```
dotnet add package Sgbj.MinimalEndpoints
```

## Using this package

```c#
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapMinimalEndpoints(typeof(Program));

app.Run();
```

### Choose the best pattern for your project

#### Lambda expression

```c#
public class HelloWorld : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/", () => "Hello World!");
}
```

#### Static method

```c#
public class GetTodo : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/api/todos/{id}", HandleAsync);

    public static async Task<Results<Ok<Todo>, NotFound>> HandleAsync(int id, TodoDbContext db) =>
        await db.Todos.FindAsync(id) is Todo todo ? TypedResults.Ok(todo) : TypedResults.NotFound();
}
```

#### Instance method (record or class)

```c#
public record GetTodos(TodoDbContext Db) : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/api/todos/{id}", ([AsParameters] GetTodos handler) => handler.HandleAsync());

    public async Task<Ok<List<Todo>>> HandleAsync() => TypedResults.Ok(await Db.Todos.ToListAsync());
}
```

#### Entire APIs

```c#
public class TodoApi : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/todos");
        group.MapGet("/", GetTodosAsync);
        group.MapGet("/{id}", GetTodoAsync);
        group.MapPost("/", CreateTodoAsync);
        group.MapPut("/{id}", UpdateTodoAsync);
        group.MapDelete("/{id}", DeleteTodoAsync);
        return group;
    }

    ...
}
```

#### [REPR design pattern](https://deviq.com/design-patterns/repr-design-pattern) (e.g., [FastEndpoints](https://github.com/FastEndpoints/Library))

```c#
public class CreateTodoEndpoint : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/todos", HandleAsync);

    public static async Task<Results<Created<CreateTodoResponse>, ValidationProblem>> HandleAsync(
        CreateTodoRequest request, TodoDbContext db, IValidator<CreateTodoRequest> validator, IMapper mapper)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todo = mapper.Map<Todo>(request);

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        var response = mapper.Map<CreateTodoResponse>(todo);

        return TypedResults.Created($"/api/todos/{todo.Id}", response);
    }
}

public record CreateTodoRequest(string Name);

public record CreateTodoResponse(int Id, string Name, bool IsComplete);

public class CreateTodoValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoValidator()
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
```

#### MediatR

```c#
public class DeleteTodoEndpoint : IMinimalEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete("/api/todos/{id}", ([AsParameters] DeleteTodoRequest request, IMediator mediator) => mediator.Send(request));
}

public record DeleteTodoRequest(int Id) : IRequest<IResult>;

public class DeleteTodoHandler : IRequestHandler<DeleteTodoRequest, IResult>
{
    ...
}
```
