namespace CodeShare.Models.Models;

public class ListingCode : WidgetMenu
{
    /// <summary>
    /// Текст кода
    /// </summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>
    /// Допускается ли копирование кода
    /// </summary>
    public bool IsCopyable { get; set; }

    /// <summary>
    /// История кода
    /// </summary>
    public IList<HistoryCode> History { get; set; }
        = new List<HistoryCode>();
}