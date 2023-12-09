namespace CodeVijetWeb.Models;

public class ListingCode
{
    /// <summary>
    /// Название текущего файла
    /// </summary>
    public string FileName { get; set; }
    /// <summary>
    /// Полный путь файла на локальной машине (со стороны сервера)
    /// </summary>
    public string FullPath { get; set; }
    /// <summary>
    /// Короткий путь к файлу \Folder\MyFile.cs
    /// </summary>
    public string ShortPath { get; set; }
    /// <summary>
    /// Текст кода
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Допускается ли копирование кода
    /// </summary>
    public bool IsCopyable { get; set; }
}