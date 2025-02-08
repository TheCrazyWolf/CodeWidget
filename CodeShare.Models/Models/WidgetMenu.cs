namespace CodeShare.Models.Models;

public class WidgetMenu
{
    /// <summary>
    /// ID виджета на основе пути файла
    /// </summary>
    public string IdListingCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Полный путь файла на локальной машине (со стороны сервера)
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Название текущего файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>
    /// Короткий путь к файлу \Folder\MyFile.cs
    /// </summary>
    public string ShortPath { get; set; } = string.Empty;
    
}