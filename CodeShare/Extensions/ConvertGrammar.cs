using PrismSharp.Core;

namespace CodeShare.Extensions;

public class ConvertGrammar
{
    public static Grammar Convert(string fileExtension)
    {
        switch (fileExtension.ToLower())
        {
            case "cs":
                return LanguageGrammars.CSharp;
            case "py":
                return LanguageGrammars.Python;
            case "css":
                return LanguageGrammars.Css;
            case "html":
            case "htm":
            case "razor":
                return LanguageGrammars.CSHtml;
            default:
                return LanguageGrammars.Markup;
        }
    } 
}