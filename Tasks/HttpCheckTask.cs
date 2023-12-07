using Domain.Entities;

namespace Tasks;

public class HttpCheckTask : IExecutableTasks
{
    private static int count = 0;
    public async Task<Exception> Run(TaskEntry taskEntry, CancellationToken cancellationToken)
    {
        count++;

        System.Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        System.Console.WriteLine(DateTime.Now.ToLongTimeString());
        System.Console.WriteLine(taskEntry.TaskTemplate.Name);

        await Task.Delay(5000);

        // if (count % 3 == 0) // Mod alinarak yapilacak
        //     throw new Exception("Failed Test...");

        // if (count < 1)
        throw new Exception("Failed Test...");

        // System.Console.WriteLine(DateTime.Now.ToLongTimeString());
        // System.Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

        // return null;
    }
}