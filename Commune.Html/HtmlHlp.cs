using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NitroBolt.Wui;
using Commune.Basis;
using System.Web;

namespace Commune.Html
{
  public static class HtmlHlp
  {
    public static string Get(this HttpContext context, string argName)
    {
      return context.Request.QueryString[argName];
    }

    public static int? GetUInt(this HttpContext context, string argName)
    {
      return Parse(context.Request.QueryString[argName]);
    }

    public static int? Parse(string arg)
    {
      if (StringHlp.IsEmpty(arg))
        return null;

      int value;
      if (int.TryParse(arg, out value) && value >= 0)
        return value;
      return null;
    }

    public static string ColorToHtml(Color color)
    {
      return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
    }

    public static object[] ContentForHElement(IHtmlControl control, string cssClassName, params object[] coreAttrs)
    {
      HAttribute classAttr;
      {
        string[] extraClassNames = (control.GetExtended("extraClassNames") as string[]) ?? new string[0];
        string[] classNames = ArrayHlp.Merge(extraClassNames, new string[] { cssClassName });
        classAttr = h.@class(classNames);
      }
        
      List<object> content = new List<object>();
      content.Add(classAttr);
      content.AddRange(coreAttrs);
      foreach (TagExtensionAttribute extension in control.TagExtensions)
        content.Add(new HAttribute(extension.Name, extension.Value));
      return content.ToArray();
    }

    static HBuilder h = null;

    //public static void AddHoverToCss(StringBuilder css, string cssClassName, IReadExtension control)
    //{
    //  AddPseudoClassToCss(css, "hover", cssClassName, control);
    //}

    public static void AddClassToCss(StringBuilder css, string className, 
      IEnumerable<CssExtensionAttribute> cssExtensions)
    {
      if (!_.CountMore(cssExtensions, 0))
        return;

      css.AppendLine(string.Format(".{0} {{", className));
      foreach (CssExtensionAttribute extension in cssExtensions)
        css.AppendLine(string.Format("  {0}:{1};", extension.Name, extension.Value));
      css.AppendLine("}");
    }

    public static void AddStyleToCss(StringBuilder css, string cssClassName, HStyle style)
    {
      if (style == null)
        return;

      AddExtensionsToCss(css, style.CssExtensions, style.Name, cssClassName);
    }

    public static void AddExtensionsToCss(StringBuilder css,
      IEnumerable<CssExtensionAttribute> cssExtensions,
      string cssPrefixFormat, params string[] cssClassNames)
    {
      if (!_.CountMore(cssExtensions, 0))
        return;

      css.AppendLine(string.Format(cssPrefixFormat, cssClassNames) + " {");
      foreach (CssExtensionAttribute extension in cssExtensions)
        css.AppendLine(string.Format("  {0}:{1};", extension.Name, extension.Value));
      css.AppendLine("}");
    }

    public static string BreakLongestWords(string plainText)
    {
      return BreakLongestWords(plainText, 30);
    }

    public static string BreakLongestWords(string plainText, int maxWordLength)
    {
      if (plainText == null)
        return "";

      Stack<int> delimiters = new Stack<int>();
      int lastSpace = 0;
      for (int i = 0; i < plainText.Length; ++i)
      {
        if (char.IsWhiteSpace(plainText[i]))
        {
          lastSpace = i;
          continue;
        }

        if (i - lastSpace > maxWordLength)
        {
          delimiters.Push(i);
          lastSpace = i;
          continue;
        }
      }

      if (delimiters.Count == 0)
        return plainText;

      List<char> chars = new List<char>(plainText.Length + delimiters.Count);
      chars.AddRange(plainText);
      while (delimiters.Count > 0)
      {
        int delimiter = delimiters.Pop();
        chars.Insert(delimiter, ' ');
      }

      return new string(chars.ToArray());
    }

  //  public static void AddPseudoClassToCss(StringBuilder css,
  //    string pseudoClassType, string cssClassName, IReadExtension control)
  //  {
  //    HStyle pseudoClass = control.GetExtended(pseudoClassType) as HStyle;
  //    if (pseudoClass == null)
  //      return;

  //    if (!_.CountMore(pseudoClass.CssExtensions, 0))
  //      return;

  //    css.AppendLine(string.Format(pseudoClass.Name + "{{", cssClassName));
  //    foreach (CssExtensionAttribute extension in pseudoClass.CssExtensions)
  //      css.AppendLine(string.Format("  {0}:{1};", extension.Name, extension.Value));
  //    css.AppendLine("}");
  //  }
  }
}
