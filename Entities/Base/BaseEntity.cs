using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Base;

public class BaseEntity : CoreEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RowNumber { get; set; }

}