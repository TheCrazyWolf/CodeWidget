namespace CodeVijetWeb.Models;

public class ListingCode
{
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public string ShortPath { get; set; }
    public string Code { get; set; }
    public bool IsCopyable { get; set; }
}