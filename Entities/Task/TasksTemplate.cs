using Domain.Base;

namespace Domain.Entities;

public class TaskTemplate : BaseEntity
{
    public string Name { get; set; }
    public TaskTypes TaskType { get; set; }

    public int PeriodAsSeconds { get; set; }
    public DateTime LastSuccessExecutedAt { get; set; }
    public DateTime NextSuccessExecutionAt { get; set; }

    public int PeriodAsSecondsWhenFailed { get; set; }
    public DateTime? LastFailedExecutedAt { get; set; }
    public DateTime? NextFailedExecutionAt { get; set; }
    public int TryingCount { get; set; }
    public int MaxTryingCount { get; set; }
    public Guid? FailedEntryGroupId { get; set; }

    public int MaxExecutableAsSeconds { get; set; }
    public TaskEntryStatus LastEntryStatus { get; set; }


    // Child Objects
    public List<TaskEntry> TaskEntries { get; set; }
}

public enum TaskTypes
{
    HttpCheck = 0,
    DnsResolver = 1,
}