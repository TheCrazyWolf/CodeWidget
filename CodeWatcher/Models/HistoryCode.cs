namespace CodeWatcher.Models;

public class HistoryCode
{
    public int Id { get; set; }
    public TimeOnly TimeStampt { get; set; }
    public string Code { get; set; } = string.Empty;

    public HistoryCode()
    {
        
    }

    public HistoryCode(int newId, TimeOnly stamp, string code)
    {
        Id = newId;
        TimeStampt = stamp;
        Code = code;
    }
}