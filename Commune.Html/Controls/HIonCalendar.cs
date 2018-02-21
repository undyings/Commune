using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;
using System.Globalization;

namespace Commune.Html
{
  public class HIonCalendar : ExtensionContainer, IHtmlControl
  {
    readonly HAttribute[] gauges;
    public HIonCalendar(string name, params HAttribute[] gauges) : 
      base("HIonCalendar", name)
    {
      this.gauges = gauges;
    }

    public HIonCalendar(string name, string years,
      params HAttribute[] gauges) :
      this (name, ArrayHlp.Merge(new HAttribute[]
        {
          new HAttribute("lang", "'ru'"),
          new HAttribute("years", string.Format("'{0}'", years))
          //new HAttribute("format", string.Format("'{0}'", format)),
          //new HAttribute("onClick", string.Format("function (data) {{ {0} }}", onClick))
        },
        gauges
      ))
    {

    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      return h.Div(
        h.Div(h.@class(Name), new HAttribute("id", Name)),
        h.Script(h.Raw(string.Format("$(function () {{ $('#{0}').ionCalendar({{ {1} }}); }});",
          Name, StringHlp.Join(",", gauges, delegate (HAttribute attr)
            { return string.Format("{0}: {1}", attr.Name, attr.Value); })
          ))
        )
      );
    }
  }
}
