//using System;
//using System.Collections.Generic;
//using System.Text;
//using NitroBolt.Wui;
//using Commune.Basis;

//namespace Commune.Html
//{
//  public class HTableGrid : ExtensionContainer, IHGrid
//  {
//    readonly HRowStyle rowStyle;
//    readonly IHtmlColumn[] columns;
//    public HTableGrid(string name, HRowStyle rowStyle, params IHtmlColumn[] columns) :
//      base("HTableGrid", name)
//    {
//      this.rowStyle = rowStyle ?? new HRowStyle();
//      this.columns = columns;

//    }

//    static HBuilder h = null;

//    public HElement ToHtml(string cssClassName, StringBuilder css)
//    {
//      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

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

//      Getter<IHtmlControl, IHtmlColumn, object, IHtmlControl> anyCellMutate =
//        rowStyle.GetExtended<Getter<IHtmlControl, IHtmlColumn, object, IHtmlControl>>("anyCellMutate");

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

//            ToneContainer columnProps = (ToneContainer)column;

//            bool isHide = columnProps.GetExtended<bool>("hide");
//            if (isHide)
//              continue;

//            IHtmlControl cell = column.GetCell(row);
//            if (anyCellMutate != null)
//              cell = anyCellMutate(column, row, cell);
//            if (columnProps.GetExtended<bool>("widthFill"))
//            {
//              ((IEditExtension)cell).Width("100%");
//              cell = new HPanel(cell).WidthFill();
//            }

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

//  //public class HTableGrid : ExtensionContainer, IHGrid
//  //{
//  //  readonly HRowStyle rowStyle;
//  //  readonly IHtmlColumn[] columns;
//  //  public HTableGrid(string name, HRowStyle rowStyle, params IHtmlColumn[] columns) :
//  //    base("HTableGrid", name)
//  //  {
//  //    this.rowStyle = rowStyle ?? new HRowStyle();
//  //    this.columns = columns;

//  //  }

//  //  static HBuilder h = null;

//  //  public HElement ToHtml(string cssClassName, StringBuilder css)
//  //  {
//  //    HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

//  //    foreach (IHtmlColumn column in columns)
//  //    {
//  //      DefaultExtensionContainer defaults = new DefaultExtensionContainer(column);
//  //      defaults.Display("inline-block");
//  //      defaults.Overflow();
//  //    }

//  //    string[] columnClassNames = new string[columns.Length];
//  //    {
//  //      int columnIndex = -1;
//  //      foreach (IHtmlColumn column in columns)
//  //      {
//  //        ++columnIndex;
//  //        string columnName = column.Name;
//  //        if (StringHlp.IsEmpty(columnName))
//  //          columnName = string.Format("column{0}", columnIndex + 1);
//  //        string columnClassName = string.Format("{0}_{1}", cssClassName, columnName);
//  //        columnClassNames[columnIndex] = columnClassName;

//  //        HtmlHlp.AddClassToCss(css, columnClassName, column.CssExtensions);
//  //      }
//  //    }

//  //    Getter<HElement, object, HElement[]> rowWrapperCreator =
//  //      rowStyle.GetExtended<Getter<HElement, object, HElement[]>>("rowWrapperCreator");

//  //    string anyRowClassName = string.Format("{0}_row", cssClassName);
//  //    HTone anyRowStyle = rowStyle.GetExtended<HTone>("anyRowStyle") ?? new HTone();
//  //    {
//  //      DefaultExtensionContainer defaults = new DefaultExtensionContainer(anyRowStyle);
//  //      defaults.CssAttribute("white-space", "nowrap");
//  //    }
//  //    HtmlHlp.AddClassToCss(css, anyRowClassName, anyRowStyle.CssExtensions);

//  //    HTone evenRowStyle = rowStyle.GetExtended<HTone>("rowEvenStyle");
//  //    string evenRowClassName = "";
//  //    if (evenRowStyle != null)
//  //    {
//  //      evenRowClassName = string.Format("{0}_even_row", cssClassName);
//  //      HtmlHlp.AddClassToCss(css, evenRowClassName, evenRowStyle.CssExtensions);
//  //    }

//  //    HTone hoverRowStyle = rowStyle.GetExtended<HTone>("rowHoverStyle");
//  //    if (hoverRowStyle != null)
//  //    {
//  //      HtmlHlp.AddExtensionsToCss(css, hoverRowStyle.CssExtensions, ".{0} div:hover", cssClassName);
//  //    }

//  //    HTone anyCellStyle = rowStyle.GetExtended<HTone>("anyCellStyle");
//  //    string anyCellClassName = "";
//  //    if (anyCellStyle != null)
//  //    {
//  //      anyCellClassName = string.Format("{0}_any_cell", cssClassName);
//  //      HtmlHlp.AddClassToCss(css, anyCellClassName, anyCellStyle.CssExtensions);
//  //    }

//  //    object[] rows = (GetExtended("rows") as object[]) ?? new object[0];
//  //    List<HElement> trs = new List<HElement>();
//  //    {
//  //      int rowIndex = -1;
//  //      foreach (object row in rows)
//  //      {
//  //        ++rowIndex;

//  //        List<HElement> tds = new List<HElement>();

//  //        int columnIndex = -1;
//  //        foreach (IHtmlColumn column in columns)
//  //        {
//  //          ++columnIndex;

//  //          IHtmlControl cell = column.GetCell(row);

//  //          string cellClassName = string.Format("{0}_{1}", columnClassNames[columnIndex], rowIndex + 1);

//  //          HElement cellElement = cell.ToHtml(cellClassName, css);

//  //          bool isHide = (column.GetExtended("hide") as bool?) ?? false;
//  //          if (!isHide)
//  //          {
//  //            object[] cellClasses;
//  //            if (StringHlp.IsEmpty(anyCellClassName))
//  //              cellClasses = new object[] { columnClassNames[columnIndex] };
//  //            else
//  //              cellClasses = new object[] { anyCellClassName, columnClassNames[columnIndex] };
//  //            tds.Add(h.Div(h.@class(cellClasses), cellElement));
//  //          }
//  //        }

//  //        List<object> rowContent = new List<object>();

//  //        List<object> rowClassNames = new List<object>();
//  //        if (anyRowStyle != null)
//  //          rowClassNames.Add(anyRowClassName);
//  //        if (evenRowStyle != null && rowIndex % 2 == 1)
//  //          rowClassNames.Add(evenRowClassName);

//  //        if (rowClassNames.Count != 0)
//  //          rowContent.Add(h.@class(rowClassNames.ToArray()));

//  //        if (rowWrapperCreator != null)
//  //          rowContent.Add(rowWrapperCreator(row, tds.ToArray()));
//  //        else
//  //          rowContent.AddRange(tds);

//  //        trs.Add(h.Div(rowContent.ToArray()));
//  //      }
//  //    }

//  //    return h.Div(HtmlHlp.ContentForHElement(this, cssClassName, trs.ToArray()));
//  //  }
//  //}
//}