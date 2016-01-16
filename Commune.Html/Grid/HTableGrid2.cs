//using System;
//using System.Collections.Generic;
//using System.Text;
//using NitroBolt.Wui;
//using Commune.Basis;
//using Commune.Html.Standart;

//namespace Commune.Html
//{
//  public class HTableGrid2 : ExtensionContainer, IHGrid
//  {
//    readonly HRowStyle rowStyle;
//    readonly IHtmlColumn[] columns;
//    public HTableGrid2(string name, HRowStyle rowStyle, params IHtmlColumn[] columns) :
//      base("HTableGrid", name)
//    {
//      this.rowStyle = rowStyle ?? new HRowStyle();
//      this.columns = columns;

//    }

//    static HBuilder h = null;

//    public HElement ToHtml(string cssClassName, StringBuilder css)
//    {
//      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

//      foreach (IHtmlColumn column in columns)
//      {
//        DefaultExtensionContainer defaults = new DefaultExtensionContainer(column);
//        defaults.Display("table-cell");
//        defaults.Overflow();
//      }

//      string[] columnClassNames = new string[columns.Length];
//      {
//        int columnIndex = -1;
//        foreach (IHtmlColumn column in columns)
//        {
//          ++columnIndex;
//          string columnName = column.Name;
//          if (StringHlp.IsEmpty(columnName))
//            columnName = string.Format("column{0}", columnIndex + 1);
//          string columnClassName = string.Format("{0}_{1}", cssClassName, columnName);
//          columnClassNames[columnIndex] = columnClassName;

//          HtmlHlp.AddClassToCss(css, columnClassName, column.CssExtensions);
//        }
//      }

//      Getter<HElement, object, HElement[]> rowWrapperCreator =
//        rowStyle.GetExtended<Getter<HElement, object, HElement[]>>("rowWrapperCreator");

//      string anyRowClassName = string.Format("{0}_row", cssClassName);
//      HTone anyRowStyle = rowStyle.GetExtended<HTone>("anyRowStyle") ?? new HTone();
//      //{
//      //  DefaultExtensionContainer defaults = new DefaultExtensionContainer(anyRowStyle);
//      //  defaults.CssAttribute("white-space", "nowrap");
//      //}
//      HtmlHlp.AddClassToCss(css, anyRowClassName, anyRowStyle.CssExtensions);

//      HTone evenRowStyle = rowStyle.GetExtended<HTone>("rowEvenStyle");
//      string evenRowClassName = "";
//      if (evenRowStyle != null)
//      {
//        evenRowClassName = string.Format("{0}_even_row", cssClassName);
//        HtmlHlp.AddClassToCss(css, evenRowClassName, evenRowStyle.CssExtensions);
//      }

//      HTone hoverRowStyle = rowStyle.GetExtended<HTone>("rowHoverStyle");
//      if (hoverRowStyle != null)
//      {
//        HtmlHlp.AddExtensionsToCss(css, hoverRowStyle.CssExtensions, ".{0} div:hover", cssClassName);
//      }

//      HTone anyCellStyle = rowStyle.GetExtended<HTone>("anyCellStyle");
//      string anyCellClassName = "";
//      if (anyCellStyle != null)
//      {
//        anyCellClassName = string.Format("{0}_any_cell", cssClassName);
//        HtmlHlp.AddClassToCss(css, anyCellClassName, anyCellStyle.CssExtensions);
//      }

//      object[] rows = (GetExtended("rows") as object[]) ?? new object[0];
//      List<HElement> trs = new List<HElement>();
//      {
//        int rowIndex = -1;
//        foreach (object row in rows)
//        {
//          ++rowIndex;

//          List<IHtmlControl> rowCells = new List<IHtmlControl>();

//          int columnIndex = -1;
//          foreach (IHtmlColumn column in columns)
//          {
//            ++columnIndex;

//            bool isHide = (column.GetExtended("hide") as bool?) ?? false;
//            if (isHide)
//              continue;

//            IHtmlControl cell = column.GetCell(row);
//            ((IEditExtension)cell).ExtraClassNames(columnClassNames[columnIndex], anyCellClassName);

//            rowCells.Add(cell);
//          }

//          HXPanel rowControl = std.RowPanel(rowCells.ToArray());

//          List<string> rowClassNames = new List<string>();
//          if (anyRowStyle != null)
//            rowClassNames.Add(anyRowClassName);
//          if (evenRowStyle != null && rowIndex % 2 == 1)
//            rowClassNames.Add(evenRowClassName);

//          rowControl.ExtraClassNames(rowClassNames.ToArray());

//          HElement rowElement = rowControl.ToHtml(
//            string.Format("{0}_{1}", cssClassName, rowIndex + 1), css);

//          HElement rowContent;
//          if (rowWrapperCreator != null)
//            rowContent = rowWrapperCreator(row, new HElement[] { rowElement });
//          else
//            rowContent = rowElement;

//          trs.Add(rowContent);
//        }
//      }

//      return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, trs.ToArray()));
//    }
//  }

//}