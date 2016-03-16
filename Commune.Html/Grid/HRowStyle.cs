using Commune.Basis;
using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Html
{
  public class HRowStyle : ToneContainer
  {
    public HRowStyle() : base()
    {
    }
  }

  public static class HRowStyleExt
  {
    public static HRowStyle Even(this HRowStyle gridStyle, HTone rowEvenStyle)
    {
      gridStyle.WithExtension("rowEvenStyle", rowEvenStyle);
      return gridStyle;
    }

    public static HRowStyle Odd(this HRowStyle gridStyle, HTone rowOddStyle)
    {
      gridStyle.WithExtension("rowOddStyle", rowOddStyle);
      return gridStyle;
    }

    public static HRowStyle Hover(this HRowStyle gridStyle, HTone rowHoverStyle)
    {
      gridStyle.WithExtension("rowHoverStyle", rowHoverStyle);
      return gridStyle;
    }

    //public static HRowStyle AnyRow(this HRowStyle rowStyle, HTone anyRowStyle)
    //{
    //  rowStyle.WithExtension("anyRowStyle", anyRowStyle);
    //  return rowStyle;
    //}

    //public static HRowStyle Even(this HRowStyle rowStyle, HTone rowEvenStyle)
    //{
    //  rowStyle.WithExtension("rowEvenStyle", rowEvenStyle);
    //  return rowStyle;
    //}

    //public static HRowStyle Hover(this HRowStyle rowStyle, HTone rowHoverStyle)
    //{
    //  rowStyle.WithExtension("rowHoverStyle", rowHoverStyle);
    //  return rowStyle;
    //}

    //public static HRowStyle Wrapper(this HRowStyle rowStyle, Getter<HElement, object, HElement[]> rowWrapperCreator)
    //{
    //  rowStyle.WithExtension("rowWrapperCreator", rowWrapperCreator);
    //  return rowStyle;
    //}

    //public static HRowStyle AnyCell(this HRowStyle rowStyle, HTone anyCellStyle)
    //{
    //  rowStyle.WithExtension("anyCellStyle", anyCellStyle);
    //  return rowStyle;
    //}

    //public static HRowStyle AnyCell<TRow>(this HRowStyle rowStyle,
    //  Getter<IHtmlControl, IHtmlColumn, TRow, IHtmlControl> anyCellMutate)
    //{
    //  rowStyle.WithExtension("anyCellMutate", new Getter<IHtmlControl, IHtmlColumn, object, IHtmlControl>(
    //    delegate(IHtmlColumn column, object row, IHtmlControl cell)
    //    {
    //      return anyCellMutate(column, (TRow)row, cell);
    //    })
    //  );
    //  return rowStyle;
    //}

    //static HBuilder h = null;

    //public static HRowStyle ALink<TRow>(this HRowStyle rowStyle, Getter<string, TRow> urlGetter)
    //{
    //  rowStyle.Wrapper(
    //    new Getter<HElement, object, HElement[]>(delegate(object row, HElement[] cells)
    //    {
    //      return h.A(h.href(urlGetter((TRow)row)), cells);
    //    })
    //  );
    //  return rowStyle;
    //}

    //public static HColumn<TRow> Hide<TRow>(this HColumn<TRow> control, bool hide)
    //{
    //  control.WithExtension("hide", hide);
    //  return control;
    //}

    //public static HColumn<TRow> WidthFill<TRow>(this HColumn<TRow> control)
    //{
    //  control.WithExtension("widthFill", true);
    //  return control;
    //}
  }
}
