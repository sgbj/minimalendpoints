using MediatR;

namespace Todos;

public record TodoCompletedNotification(int Id) : INotification;
