using Domain.Base;

namespace Domain.Entities;

public class TaskEntry : BaseEntity
{
    public DateTime ExecuteStartAt { get; set; }
    public DateTime ExecuteEndAt { get; set; }
    public double WorkingTimeAsSeconds { get; set; }
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
    public int RetryIndex { get; set; }
    public TaskEntryStatus TaskEntryStatus { get; set; }
    public Guid? GroupId { get; set; }

    // Parent Objects
    public Guid TaskTemplateId { get; set; }
    public TaskTemplate TaskTemplate { get; set; }
}

public enum TaskEntryStatus
{
    Unkown = 0,
    Running = 1,
    Successed = 2,
    Failed = 3,
    Timeouted = 4,
}