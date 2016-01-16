using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public class HtmlHlp
  {
    public static string ColorToHtml(Color color)
    {
      return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
    }

    public static void AddClassToCss(StringBuilder css, string className, IEnumerable<CssExtensionAttribute> cssExtensions)
    {
      if (!_.CountMore(cssExtensions, 0))
        return;

      css.AppendLine(string.Format(".{0}{{", className));
      foreach (CssExtensionAttribute extension in cssExtensions)
        css.AppendLine(string.Format("  {0}:{1};", extension.Name, extension.Value));
      css.AppendLine("}");
    }

    public static void AddClassToCss(StringBuilder css, ExtensionContainer style)
    {
      AddClassToCss(css, style.Name, style.CssExtensions);
    }

    public static object[] ContentForHElement(IHtmlControl control, string cssClassName, params object[] coreAttrs)
    {
      List<object> content = new List<object>();
      content.Add(HtmlHlp.GetClassNames(control, cssClassName));
      content.AddRange(coreAttrs);
      foreach (TagExtensionAttribute extension in control.TagExtensions)
        content.Add(new HAttribute(extension.Name, extension.Value));
      return content.ToArray();
    }

    static HBuilder h = null;

    public static HAttribute GetClassNames(IHtmlControl control, string cssClassName)
    {
      string[] classNames = control.GetExtended("classNames") as string[];
      if (classNames != null)
        return h.@class(ArrayHlp.Merge(new string[] { cssClassName }, classNames));
      return h.@class(cssClassName);
    }

    public static void AddHoverToCss(StringBuilder css, string cssClassName, IReadExtension control)
    {
      AddPseudoClassToCss(css, "hover", cssClassName, control);
    }

    public static void AddPseudoClassToCss(StringBuilder css,
      string pseudoClassType, string cssClassName, IReadExtension control)
    {
      PseudoCssClass pseudoClass = control.GetExtended(pseudoClassType) as PseudoCssClass;
      if (pseudoClass == null)
        return;

      if (!_.CountMore(pseudoClass.CssExtensions, 0))
        return;

      css.AppendLine(string.Format(pseudoClass.Name + "{{", cssClassName));
      foreach (CssExtensionAttribute extension in pseudoClass.CssExtensions)
        css.AppendLine(string.Format("  {0}:{1};", extension.Name, extension.Value));
      css.AppendLine("}");
    }
  }
}
