using Domain.Entities;

namespace Tasks;

public class DnsResolverTask : IExecutableTasks
{
    public async Task<Exception> Run(TaskEntry taskEntry, CancellationToken cancellationToken)
    {
        System.Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
        System.Console.WriteLine(DateTime.Now.ToLongTimeString());
        System.Console.WriteLine(taskEntry.TaskTemplate.Name);

        await Task.Delay(2500);

        System.Console.WriteLine(DateTime.Now.ToLongTimeString());
        System.Console.WriteLine("|||||||||||||||||||||||||||||||");

        return null;
    }
}