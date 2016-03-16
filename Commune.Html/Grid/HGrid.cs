using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HGrid<TRow> : ExtensionContainer, IHGrid
  {
    readonly IHtmlControl header;
    readonly IEnumerable<TRow> rows;
    readonly Getter<IHtmlControl, TRow> rowControlGetter;
    readonly HRowStyle rowStyle;
    readonly IHtmlControl footer;

    public HGrid(IHtmlControl header, 
      IEnumerable<TRow> rows, Getter<IHtmlControl, TRow> rowControlGetter, HRowStyle rowStyle,
      IHtmlControl footer) :
      base("HGrid", "")
    {
      this.header = header;
      this.rows = rows;
      this.rowControlGetter = rowControlGetter;
      this.rowStyle = rowStyle ?? new HRowStyle();
      this.footer = footer;
    }

    public HGrid(IEnumerable<TRow> rows, Getter<IHtmlControl, TRow> rowControlGetter, HRowStyle rowStyle) :
      this(null, rows, rowControlGetter, rowStyle, null)
    {
    }

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

      HTone rowHoverStyle = rowStyle.GetExtended<HTone>("rowHoverStyle");
      string rowAnyClassName = "";
      if (rowHoverStyle != null)
      {
        rowAnyClassName = string.Format("{0}_any_row", cssClassName);
        HtmlHlp.AddExtensionsToCss(css, rowHoverStyle.CssExtensions, ".{0}:hover", rowAnyClassName);
      }

      HTone rowEvenStyle = rowStyle.GetExtended<HTone>("rowEvenStyle");
      string rowEvenClassName = "";
      if (rowEvenStyle != null)
      {
        rowEvenClassName = string.Format("{0}_even_row", cssClassName);
        HtmlHlp.AddClassToCss(css, rowEvenClassName, rowEvenStyle.CssExtensions);
      }

      HTone rowOddStyle = rowStyle.GetExtended<HTone>("rowOddStyle");
      string rowOddClassName = "";
      if (rowOddStyle != null)
      {
        rowOddClassName = string.Format("{0}_odd_row", cssClassName);
        HtmlHlp.AddClassToCss(css, rowOddClassName, rowOddStyle.CssExtensions);
      }

      List<IHtmlControl> controls = new List<IHtmlControl>();
      if (header != null)
        controls.Add(header);

      if (rows != null)
      {
        int rowIndex = -1;
        foreach (TRow row in rows)
        {
          rowIndex++;
          IHtmlControl rowControl = rowControlGetter(row);
          {
            List<string> rowExtraClassNames = new List<string>();
            if (rowHoverStyle != null)
              rowExtraClassNames.Add(rowAnyClassName);
            if (rowEvenStyle != null && rowIndex % 2 == 1)
              rowExtraClassNames.Add(rowEvenClassName);
            if (rowOddStyle != null && rowIndex % 2 == 0)
              rowExtraClassNames.Add(rowOddClassName);
            if (rowExtraClassNames.Count != 0)
              rowControl.ExtraClassNames(rowExtraClassNames.ToArray());
          }

          controls.Add(rowControl);
        }
      }

      if (footer != null)
        controls.Add(footer);

      HPanel gridPanel = new HPanel(controls.ToArray());
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(gridPanel);
      defaults.Width("100%");

      return gridPanel.ToHtml(cssClassName, css);
    }
  }
}
