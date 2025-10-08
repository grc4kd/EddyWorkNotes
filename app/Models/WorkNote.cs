using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eddy.Models;

public class WorkNote
{
    public int Id { get; set; }

    public DateTime RecordedAtTime { get; set; }

    [StringLength(1000)]
    [Column(TypeName = "nvarchar(1000)")]
    public string? Description { get; set; }
}