using System;
using System.Collections.Generic;
using System.Text;
using NitroBolt.Wui;
using Commune.Basis;

namespace Commune.Html
{
  public class HTableGrid : ExtensionContainer, IHGrid
  {
    readonly IHtmlColumn[] columns;
    public HTableGrid(string name, params IHtmlColumn[] columns) :
      base("HTableGrid", name)
    {
      this.columns = columns;
    }

    HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      string[] columnClassNames = new string[columns.Length];
      {
        int columnIndex = -1;
        foreach (IHtmlColumn column in columns)
        {
          ++columnIndex;
          string columnName = column.Name;
          if (StringHlp.IsEmpty(columnName))
            columnName = string.Format("column{0}", columnIndex + 1);
          string columnClassName = string.Format("{0}_{1}", cssClassName, columnName);
          columnClassNames[columnIndex] = columnClassName;

          HtmlHlp.AddClassToCss(css, columnClassName, column.CssExtensions);
          HtmlHlp.AddHoverToCss(css, columnClassName, this);
        }
      }

      object[] rows = (GetExtended("rows") as object[]) ?? new object[0];
      List<HElement> trs = new List<HElement>();
      {
        int rowIndex = -1;
        foreach (object row in rows)
        {
          ++rowIndex;

          List<HElement> tds = new List<HElement>();

          int columnIndex = -1;
          foreach (IHtmlColumn column in columns)
          {
            ++columnIndex;

            IHtmlControl cell = column.GetCell(row);
            ((IEditExtension)cell).Class(columnClassNames[columnIndex]);

            string cellClassName = string.Format("{0}_{1}", columnClassNames[columnIndex], rowIndex + 1);

            HElement cellElement = cell.ToHtml(cellClassName, css);

            bool isHide = (column.GetExtended("hide") as bool?) ?? false;
            if (!isHide)
              //tds.Add(HtmlHlp.ContentForHElement(
              tds.Add(h.Td(cellElement));
          }

          trs.Add(h.Tr(tds.ToArray()));
        }
      }

      return h.Table(HtmlHlp.GetClassNames(this, cssClassName), h.TBody(trs.ToArray()));
    }

  }
}
