using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;
using System.Globalization;

namespace Commune.Html
{
  public class HIonSlider : ExtensionContainer, IHtmlControl
  {
    readonly HAttribute[] gauges;
    public HIonSlider(string name, params HAttribute[] gauges) : 
      base("HIonSlider", name)
    {
      this.gauges = gauges;
    }

    public HIonSlider(string name, bool isDouble, decimal min, decimal max, decimal step, 
      string postfix, bool showMinMax, string onFinish,
      params HAttribute[] gauges) :
      this (name, ArrayHlp.Merge(new HAttribute[]
        {
          new HAttribute("type", isDouble ? "'double'" : "'single'"),
          new HAttribute("min", min.ToString(CultureInfo.InvariantCulture)),
          new HAttribute("max", max.ToString(CultureInfo.InvariantCulture)),
          new HAttribute("step", step.ToString(CultureInfo.InvariantCulture)),
          new HAttribute("postfix", string.Format("'{0}'", postfix)),
          new HAttribute("hide_min_max", showMinMax ? "false" : "true"),
          new HAttribute("onFinish", string.Format("function (data) {{ {0} }}", onFinish))
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
        h.Input(h.type("text"), new HAttribute("id", Name), h.data("name", Name)), // new HAttribute("name", Name)),
        h.Script(h.Raw(string.Format("$(function () {{ $('#{0}').ionRangeSlider({{ {1} }}); }});",
          Name, StringHlp.Join(",", gauges, delegate(HAttribute attr)
            { return string.Format("{0}: {1}", attr.Name, attr.Value); })
          ))
        )
      );
    }
  }
}
