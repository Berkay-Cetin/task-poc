using Domain.Entities;

namespace Data;

public class TaskData
{
    private static List<TaskTemplate> _tasks = new List<TaskTemplate>()
    {
        new TaskTemplate()
        {
            Id = Guid.Parse("e91496d2-27ee-44f6-9bb3-93c99c2e2171"),
            Name = "Http Check",
            PeriodAsSeconds = 30,
            MaxTryingCount = 2,
            MaxExecutableAsSeconds = 10,
            TaskType = TaskTypes.HttpCheck,
            PeriodAsSecondsWhenFailed  = 3,
            LastSuccessExecutedAt = DateTime.Now.AddSeconds(5),
            NextSuccessExecutionAt = DateTime.Now.AddSeconds(5),
            NextFailedExecutionAt = null,
        },
        // new TaskTemplate()
        // {
        //     Id = Guid.Parse("b813a164-b754-424e-ac70-d01ca7253681"),
        //     Name = "Dns Revolver",
        //     PeriodAsSeconds = 15,
        //     MaxTryingCount = 1,
        //     MaxExecutableAsSeconds = 5,
        //     TaskType = TaskTypes.DnsResolver,
        //     PeriodAsSecondsWhenFailed  = 1,
        //     NextSuccessExecutionAt = DateTime.Now.AddSeconds(2),
        //     NextFailedExecutionAt = null,
        // }
    };

    private static List<TaskEntry> _entries = new List<TaskEntry>() { };


    public async Task<List<Guid>> GetExecutableSuccessTaskTemplates(DateTime dateTime)
    {
        return await Task.FromResult(_tasks.Where(t => (t.NextSuccessExecutionAt <= dateTime) &&
                        t.LastEntryStatus != TaskEntryStatus.Running).Select(t => t.Id).ToList());
    }

    public async Task<List<Guid>> GetExecutableFailedTaskTemplates(DateTime dateTime)
    {
        return await Task.FromResult(_tasks.Where(t => t.NextFailedExecutionAt != null &&
                                                        t.NextFailedExecutionAt <= dateTime &&
                                                        t.LastEntryStatus != TaskEntryStatus.Running)
                                                        .Select(t => t.Id)
                                                        .ToList());
    }

    public async Task<TaskTemplate> GetTemplateById(Guid taskTemplateId)
    {
        return await Task.FromResult(_tasks.FirstOrDefault(t => t.Id == taskTemplateId));
    }

    public async Task UpdateTaskTemplate(TaskTemplate taskTemplate)
    {
        // NOP:
        await Task.CompletedTask;

        var tempIndex = _tasks.FindIndex(t => t.Id == taskTemplate.Id);
        _tasks[tempIndex] = taskTemplate;
    }

    public async Task AddTaskEntry(TaskEntry taskEntry)
    {
        // NOP:
        await Task.CompletedTask;

        _entries.Add(taskEntry);
    }

    public async Task UpdateTaskEntry(TaskEntry taskEntry)
    {
        // NOP:
        await Task.CompletedTask;

        var tempIndex = _entries.FindIndex(t => t.Id == taskEntry.Id);
        _entries[tempIndex] = taskEntry;
    }
}