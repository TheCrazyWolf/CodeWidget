namespace CodeVijetWeb.Models;

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
    
}