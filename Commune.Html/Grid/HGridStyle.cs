//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Commune.Basis;
//using NitroBolt.Wui;

//namespace Commune.Html
//{
//  public class HGridStyle : ToneContainer
//  {
//    public HGridStyle() : base()
//    {
//    }
//  }

//  public static class HGridExt
//  {
//    public static HGridStyle RowEven(this HGridStyle gridStyle, HTone rowEvenStyle)
//    {
//      gridStyle.WithExtension("rowEvenStyle", rowEvenStyle);
//      return gridStyle;
//    }

//    public static HGridStyle RowOdd(this HGridStyle gridStyle, HTone rowOddStyle)
//    {
//      gridStyle.WithExtension("rowOddStyle", rowOddStyle);
//      return gridStyle;
//    }

//    public static HGridStyle Hover(this HGridStyle gridStyle, HTone rowHoverStyle)
//    {
//      gridStyle.WithExtension("rowHoverStyle", rowHoverStyle);
//      return gridStyle;
//    }

//    public static HGridStyle Rows<TRow>(this HGridStyle gridStyle, IEnumerable<TRow> rows)
//    {
//      gridStyle.WithExtension("rows", rows);
//      return gridStyle;
//    }

//    public static HGridStyle Header(this HGridStyle gridStyle, params IHtmlControl[] headers)
//    {
//      gridStyle.WithExtension("header", headers);
//      return gridStyle;
//    }

//    public static HGridStyle Footer(this HGridStyle gridStyle, params IHtmlControl[] footers)
//    {
//      gridStyle.WithExtension("footer", footers);
//      return gridStyle;
//    }
//  }
//}
