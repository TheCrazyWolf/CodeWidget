using System.ComponentModel.DataAnnotations;

namespace CodeShare.DB;

public class Log
{
    [Key] public int LogsId { get; set; }
    public string Path { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
}