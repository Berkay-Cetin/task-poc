using Domain.Entities;

namespace Tasks;

public interface IExecutableTasks
{
    Task<Exception> Run(TaskEntry taskEntry, CancellationToken cancellationToken);
}