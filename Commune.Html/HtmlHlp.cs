﻿using System;
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
    static readonly HBuilder h = null;

    public static HtmlResult FirstHtmlResult(HElement page, object state, TimeSpan? refreshPeriod)
    {
      return new HtmlResult
      {
        Html = page,
        FirstHtmlTransformer = HtmlHlp.FirstHtmlTransformer,
        ToHtmlText = HtmlHlp.ToHtmlText,
        State = state,
        RefreshPeriod = refreshPeriod
      };
    }

    public static HElement FirstHtmlTransformer(HElement element)
    {
      return element;
    }

    public static string ToHtmlText(HElement element)
    {
      StringBuilder builder = new StringBuilder();
      ToHtmlText(builder, element);
      return builder.ToString();
    }

    static void ToHtmlText(StringBuilder builder, HElement element)
    {
      string elementType = element.Name.LocalName?.ToString();
      builder.Append("<");
      builder.Append(elementType);
      foreach (var attribute in element.Attributes)
      {
        builder.Append(' ');
        attribute.ToHtmlText(builder);
      }
      if (IsEmptyHtmlElementType(elementType))
        builder.Append("/>");
      else
      {
        if (!element.Nodes.Any())
          builder.Append(">");
        else
        {
          builder.Append(">");
          foreach (var node in element.Nodes)
          {
            if (node is HElement)
              ToHtmlText(builder, (HElement)node);
            else if (node is HText)
              builder.Append(HttpUtility.HtmlEncode(((HText)node).Text));
            else
              node.ToHtmlText(builder);
          }
        }
        builder.Append("</");
        builder.Append(elementType);
        builder.Append(">");
      }

    }

    static bool IsEmptyHtmlElementType(string elementType)
    {
      switch (elementType)
      {
        case "area":
        case "base":
        case "br":
        case "col":
        case "command":
        case "embed":
        case "hr":
        case "img":
        case "input":
        case "keygen":
        case "link":
        case "meta":
        case "param":
        case "source":
        case "track":
        case "wbr":
          return true;
        default:
          return false;
      }
    }

    public static IHtmlControl CKEditorCreate(string dataName, string text,
      string height, bool isRu, params string[] editorProps)
    {
      List<string> propList = new List<string>();
      propList.Add(string.Format("height: '{0}'", height));
      if (isRu)
        propList.Add("language: 'ru'");
      propList.AddRange(editorProps);

      return new HElementControl(
        h.TextArea(
          new HAttribute("id", dataName),
          h.@class(dataName),
          h.data("name", dataName),
          h.Attribute("js-init", string.Format("CKEDITOR.replace(this, {{ {0} }})",
            StringHlp.Join(", ", "{0}", propList)
          )),
          text
        ), dataName
      );
    }

    public static HElement CKEditorUpdateAll()
    {
      return h.Script(@"
               function CK_updateAll()
               {
                 try
                 {
                   for (instance in CKEDITOR.instances)
                     CKEDITOR.instances[instance].updateElement();
                 }
                 catch(ex)
                 {
                   console.log(ex);
                 }
               }
            ");
    }

    public static HElement GoogleAnalytics()
    {
      return h.Script(h.type("text/javascript"), h.src("http://www.google-analytics.com/ga.js"), "");
    }

    public static HElement YandexMetrics()
    {
      return h.Script(h.type("text/javascript"), h.src("https://mc.yandex.ru/metrika/watch.js"), "");
    }

    public static T CKEditorOnUpdateAll<T>(this T control) where T : IEditExtension
    {
      return control.OnClick("CK_updateAll();");
    }

    public static string Get(this HttpContext context, string argName)
    {
      string arg = context.Request.QueryString[argName];
      if (arg == null)
        return null;
      return arg.ToLower();
    }

    public static int? GetUInt(this HttpContext context, string argName)
    {
      return Parse(context.Request.QueryString[argName]);
    }

    //public static TState GetState<TState>(this HttpContext context)
    //{
    //  UpdateCycle<HElement>[] updates = HWebSynchronizeHandler.Updates(context);
    //  if (updates.Length == 0)
    //    return default(TState);

    //  return (TState)updates[0].State;
    //}

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

    public static void AddMediaToCss(StringBuilder css, string cssClassName,
      IEnumerable<MediaExtensionAttribute> mediaExtensions)
    {
      foreach (MediaExtensionAttribute media in mediaExtensions)
      {
        HStyle[] styles = media.Styles;
        if (styles == null || styles.Length == 0)
          continue;

        css.AppendLine(string.Format("@media {0} {{", media.Name));
        foreach (HStyle style in styles)
        {
          AddStyleToCss(css, cssClassName, style); 
        }
        css.AppendLine("}");
      }
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
