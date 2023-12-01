using System.Diagnostics.CodeAnalysis;
using Data;
using Domain.Entities;
using Tasks;

namespace Services;

public class TaskService
{
    private readonly TaskData _taskData;
    public TaskService(TaskData taskData)
    {
        _taskData = taskData;
    }

    public async Task Run()
    {
        while (true)
        {
            var taskTemplateIds = await _taskData.GetExecutableTaskTemplates(DateTime.Now);
            System.Console.WriteLine($"Total Task Count: {taskTemplateIds.Count}");

            foreach (var taskTemplateId in taskTemplateIds)
            {
                var taskTemplate = await _taskData.GetTemplateById(taskTemplateId);

                if (taskTemplate.LastEntryStatus == TaskEntryStatus.Running)
                {
                    System.Console.WriteLine($"Skipping Task Template: {taskTemplate.Name}");
                    continue;
                }

                System.Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                System.Console.WriteLine($"Starting Task Template Name: {taskTemplate.Name}");

                var taskEntry = new TaskEntry()
                {
                    Id = Guid.NewGuid(),
                    TaskEntryStatus = TaskEntryStatus.Running,
                    TaskTemplateId = taskTemplate.Id,
                    TaskTemplate = taskTemplate,
                    ExecuteStartAt = DateTime.Now,
                };
                await _taskData.AddTaskEntry(taskEntry);

                // if (taskTemplate.LastEntryStatus == TaskEntryStatus.Successed)
                //     taskTemplate.LastSuccessExecutedAt = DateTime.Now;
                // else
                //     taskTemplate.LastFailedExecutedAt = DateTime.Now;

                taskTemplate.LastEntryStatus = TaskEntryStatus.Running;
                taskTemplate.LastSuccessExecutedAt = taskTemplate.NextSuccessExecutionAt;
                taskTemplate.NextSuccessExecutionAt = taskTemplate.LastSuccessExecutedAt.AddSeconds(taskTemplate.PeriodAsSeconds);
                await _taskData.UpdateTaskTemplate(taskTemplate);

                IExecutableTasks iExecutable;

                switch (taskTemplate.TaskType)
                {
                    case TaskTypes.HttpCheck:
                        iExecutable = new HttpCheckTask();
                        break;
                    case TaskTypes.DnsResolver:
                        iExecutable = new DnsResolverTask();
                        break;

                    default:
                        throw new Exception("Task Template Type Unkown");
                }
                _ = Task.Run(async () => await RunTaskEntry(taskTemplate, taskEntry, iExecutable));
            }

            await Task.Delay(1000);
        }
    }

    private async Task RunTaskEntry(TaskTemplate taskTemplate, TaskEntry taskEntry, IExecutableTasks iExecutable)
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        try
        {
            var task = iExecutable.Run(taskEntry, cancellationToken);

            int timeoutMs = taskTemplate.MaxExecutableAsSeconds * 1000;
            if (await Task.WhenAny(task, Task.Delay(timeoutMs, cancellationToken)) == task)
            {
                var operationException = await task;
                System.Console.WriteLine($"::: Task Completed {taskTemplate.Name}");
                if (operationException != null)
                {
                    System.Console.WriteLine("::::::::::::::::::::::::::::::: Task Failed Try If");
                    await HandleFail(taskTemplate, taskEntry, operationException, TaskEntryStatus.Failed);
                }
                else
                {
                    await HandleSuccess(taskTemplate, taskEntry);
                }
            }
            else
            {
                System.Console.WriteLine("Timeout Won");
                System.Console.WriteLine("::::::::::::::::::::::::::::::: Task Failed Try Else");
                var timeOutException = new Exception("Timeout");

                await HandleFail(taskTemplate, taskEntry, timeOutException, TaskEntryStatus.Timeouted);
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("::::::::::::::::::::::::::::::: Task Failed Catch");
            await HandleFail(taskTemplate, taskEntry, ex, TaskEntryStatus.Failed);
        }
    }

    private async Task HandleSuccess(TaskTemplate taskTemplate, TaskEntry taskEntry)
    {
        taskTemplate.LastEntryStatus = TaskEntryStatus.Successed;
        taskTemplate.NextFailedExecutionAt = null;
        taskTemplate.TryingCount = 0;
        await _taskData.UpdateTaskTemplate(taskTemplate);

        taskEntry.ExecuteEndAt = DateTime.Now;
        taskEntry.TaskEntryStatus = TaskEntryStatus.Successed;
        taskEntry.WorkingTimeAsSeconds = (DateTime.Now - taskEntry.ExecuteStartAt).TotalSeconds;
        await _taskData.UpdateTaskEntry(taskEntry);

        System.Console.WriteLine("::::::::::::::::::::::::::::::: Task Succeded Try");
        PrintTaskStatus(taskTemplate, "Success");
    }

    private static void PrintTaskStatus(TaskTemplate taskTemplate, string status)
    {
        System.Console.WriteLine("===");
        System.Console.WriteLine($"Status: {status}");
        System.Console.WriteLine($"Last Success: {taskTemplate.LastSuccessExecutedAt.ToLongTimeString()}");
        System.Console.WriteLine($"Next Success: {taskTemplate.NextSuccessExecutionAt.ToLongTimeString()}");
        System.Console.WriteLine($"Period As Seconds: {taskTemplate.PeriodAsSeconds}");
        System.Console.WriteLine("=");
        System.Console.WriteLine($"Last Failed: {taskTemplate.LastFailedExecutedAt?.ToLongTimeString()}");
        System.Console.WriteLine($"Next Failed: {taskTemplate.NextFailedExecutionAt?.ToLongTimeString()}");
        System.Console.WriteLine($"Failed Period As Seconds: {taskTemplate.PeriodAsSecondsWhenFailed}");
        System.Console.WriteLine("===");
    }

    private async Task HandleFail(TaskTemplate taskTemplate, TaskEntry taskEntry, Exception ex, TaskEntryStatus taskEntryStatus)
    {
        taskEntry.ErrorMessage = ex.Message;
        taskEntry.StackTrace = ex.StackTrace;
        taskEntry.ExecuteEndAt = DateTime.Now;
        taskEntry.TaskEntryStatus = taskEntryStatus;
        taskEntry.WorkingTimeAsSeconds = (DateTime.Now - taskEntry.ExecuteStartAt).TotalSeconds;
        taskEntry.RetryIndex = taskTemplate.TryingCount;
        await _taskData.UpdateTaskEntry(taskEntry);

        if (taskTemplate.TryingCount + 1 >= taskTemplate.MaxTryingCount)
        {
            System.Console.WriteLine("::::::::::::::::::::::::::::::: Max Trying Count Excedded");

            taskTemplate.LastEntryStatus = taskEntryStatus;
            taskTemplate.LastFailedExecutedAt = DateTime.Now;
            taskTemplate.NextFailedExecutionAt = null;
            taskTemplate.TryingCount = 0;
            await _taskData.UpdateTaskTemplate(taskTemplate);

            PrintTaskStatus(taskTemplate, "Failed-Max Trying Excedded");
        }
        else
        {
            System.Console.WriteLine("::::::::::::::::::::::::::::::: Retry Count Increased");

            taskTemplate.LastEntryStatus = taskEntryStatus;
            taskTemplate.LastFailedExecutedAt = DateTime.Now;
            taskTemplate.NextFailedExecutionAt = DateTime.Now.AddSeconds(taskTemplate.PeriodAsSecondsWhenFailed);
            taskTemplate.TryingCount += 1;
            await _taskData.UpdateTaskTemplate(taskTemplate);

            PrintTaskStatus(taskTemplate, "Failed-Normal");
        }
    }
}

// TODO: use Stopwatch instead time difference.