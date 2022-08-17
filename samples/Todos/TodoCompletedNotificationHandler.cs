using MediatR;

namespace Todos;

public class TodoCompletedNotificationHandler : INotificationHandler<TodoCompletedNotification>
{
    private readonly ILogger<TodoCompletedNotificationHandler> _logger;

    public TodoCompletedNotificationHandler(ILogger<TodoCompletedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoCompletedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Todo completed: {Id}", notification.Id);
        return Task.CompletedTask;
    }
}
