using Commune.Basis;
using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Html
{
  public static class std
  {
    public static HClickDropdown ComboButton(HBefore beforeIcon, string caption, 
      bool isLeftDropListAlignment, params IHtmlControl[] listControls)
    {
      return new HClickDropdown(
        new HDropStyle().
          DropList(new HTone()
            .Top("31px")
            .CssAttribute(isLeftDropListAlignment ? "left" : "right", "0px")
            .Background("#f7f7f7")
            .Border("1px", "solid", "#e0e0e0", "2px")
            .CssAttribute("border-top", "none")
            .Padding(4, 0)
          ).
          RootWhenDropped(new HTone()
            .Background("#f7f7f7")
            .Border("1px", "solid", "#e0e0e0", "0px")
            .CssAttribute("border-bottom", "none")
          ).
          AnyItem(new HTone().Padding(4, 8)).
          SelectedItem(new HTone()
            .Color("#f5ffe6")
            .Background("#999999")
          ),
        new HButton(caption,
          beforeIcon,
          new HAfter().Content(@"▼").FontSize("60%").MarginLeft(5).MarginRight(-2).VAlign(1),
          new HHover().Border("1px", "solid", "#aaaaaa", "2px")
            .Background("#eaeaea")
            .LinearGradient("to top right", "#cccccc", "#eaeaea")
        ).Padding(6, 12)
          .Background("#f1f1f1")
          .LinearGradient("to top right", "#dddddd", "#f1f1f1")
          .Border("1px", "solid", "#bbbbbb", "2px"),
        listControls
      );
    }

    public static HSeparator Separator()
    {
      return new HSeparator().CssAttribute("border-bottom", "1px solid #fff").Height("1px")
        .Background("#c9c9c9").Margin(2, 0, 1, 0);
    }

    public static HLabel Label(string caption)
    {
      return new HLabel(caption).Padding("4px 8px");
    }

    public static HButton Button(string caption, params HStyle[] prefixStyles)
    {
      return Button(caption, 6, 12, prefixStyles);
    }

    public static HButton Button(string caption, int vertPadding, int horPadding, params HStyle[] prefixStyles)
    {
      HStyle hover = new HStyle(".{0}:hover")
        .Border("1px", "solid", "#aaaaaa", "2px")
        .Background("#eaeaea")
        .LinearGradient("to top right", "#cccccc", "#eaeaea");

      HStyle active = new HStyle(".{0}:active")
        .Border("2px", "double", "#2c628b", "2px")
        .Padding(vertPadding - 1, horPadding, vertPadding - 1, horPadding - 2)
        .Background("#e5f4fc")
        .LinearGradient("to top right", "#68b3db", "#e5f4fc");

      HStyle[] allStyles = new HStyle[] { hover, active };
      if (prefixStyles.Length != 0)
        allStyles = ArrayHlp.Merge(allStyles, prefixStyles);

      return new HButton(caption, allStyles)
        .Color("black")
        .Padding(vertPadding, horPadding)
        .Border("1px", "solid", "#bbbbbb", "2px")
        .Background("#f1f1f1")
        .LinearGradient("to top right", "#dddddd", "#f1f1f1");
    }

    public static HBefore BeforeAwesome(string content, int marginRight)
    {
      return new HBefore().Content(content).FontFamily("FontAwesome").MarginRight(marginRight);
    }

    public static HAfter AfterAwesome(string content, int marginLeft)
    {
      return new HAfter().Content(content).FontFamily("FontAwesome").MarginLeft(marginLeft);
    }

    /// <summary>
    /// Создает HXPanel
    /// Оборачивает дочерние контролы не являющиеся панелями в HPanel
    /// По умолчанию выставляет всем дочерним панелям VAlign(null)
    /// </summary>
    public static HXPanel RowPanel(params IHtmlControl[] controls)
    {
      IHtmlControl[] panelControls = ArrayHlp.Convert(controls, delegate(IHtmlControl control)
      {
        if (!(control is HPanel || control is HXPanel))
          control = new HPanel(control);
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(control);
        defaults.VAlign(null);
        return control;
      });

      return new HXPanel(panelControls); //.NoWrap();
    }

    /// <summary>
    /// В HXPanel занимает все свободное место между левой и правой частью
    /// </summary>
    /// <returns></returns>
    public static HPanel DockFill()
    {
      return new HPanel(new HPanel().Width("100%")).WidthFill();
    }

    public static HPanel DockFill(IHtmlControl control)
    {
      ((IEditExtension)control).Width("100%");
      return new HPanel(control).WidthFill();
    }

    public static HXPanel SimpleHeader(params IHtmlControl[] cells)
    {
      foreach (IHtmlControl cell in cells)
      {
        DefaultExtensionContainer defaults = new DefaultExtensionContainer(cell);
        defaults.Align(null);
        defaults.VAlign(null);
        defaults.FontBold();
      }

      return new HXPanel(cells);
    }

    public static void AddStyleForFileUploaderButtons(StringBuilder css)
    {
      HtmlHlp.AddStyleToCss(css, "qq-upload-button", new HStyle()
        .InlineBlock()
        .Padding(6, 12)
        .Border("1px", "solid", "#bbbbbb", "2px")
        .Background("#f1f1f1")
        .LinearGradient("to top right", "#dddddd", "#f1f1f1")
      );

      HtmlHlp.AddStyleToCss(css, "qq-upload-button-hover", new HStyle()
        .Border("1px", "solid", "#aaaaaa", "2px")
        .Background("#eaeaea")
        .LinearGradient("to top right", "#cccccc", "#eaeaea")
      );
    }


    public static HPanel OperationWarning(WebOperation operation)
    {
      string titleColor = "#FFF"; // "#F1F6CD";
      string titleBackground = "#880000"; // "#CC0000"; // "#597BA5";
      return new HPanel(
        new HPanel(
          new HLabel("",
            new HBefore().Content(@"\f0e7")
              .Color(titleColor).Opacity("0.6").FontFamily("FontAwesome").FontBold()
          ).Cursor("default"),
          new HLabel(operation.Message).MarginLeft(8)
            .Color(titleColor).FontFamily("Arial").FontSize("13px").FontBold(),
          new HButton("",
            new HAfter().Content(@"\f00d")
              .FontFamily("FontAwesome").FontSize("1.15em"),
            new HHover().Opacity("1")
          ).FloatRight().MarginTop(-1).Color("#FFF").Opacity("0.6")
            .Event("warning_hide", "", delegate (JsonData json)
            {
              operation.Reset();
            })
        ).Padding(6, 10, 7, 14).Background(titleBackground).BorderRadius(16)
      ).Hide(operation.Completed || StringHlp.IsEmpty(operation.Message))
      .MarginTop(12);
    }

    [Obsolete()]
    public static HPanel OperationState(WebOperation operation)
    {
      //HHover closeHover = new HHover().Background(Color.FromArgb(189, 216, 249));

      HBefore alert = new HBefore().Content(@"!").CssAttribute("border-radius", 5).MarginRight(4);

      return new HPanel(
        new HButton("",
          new HAfter().Content(@"\2A2F").FontWeight("700").FontSize("125%"))
          .CssAttribute("margin-top", "-6px")
          .MarginRight("4px").FloatRight().Color("#4e545b")
          .Event("currentOperationState_Hide", "currentOperationState",
            delegate(JsonData json)
            {
              operation.Message = "";
            }
          ),
        new HLabel(operation.Message, alert).Padding(10, 12, 8, 12)
      ).EditContainer("currentOperationState").
      CssAttribute("position", "relative").Display("inline-block").
      Border("2px", "outset", HtmlHlp.ColorToHtml(Color.Blue), "2px").Color("#dc322f").
      Background(HtmlHlp.ColorToHtml(Color.LightBlue)).WidthLimit("200px", "").
      Hide(operation.Completed || StringHlp.IsEmpty(operation.Message));
    }
  }
}
