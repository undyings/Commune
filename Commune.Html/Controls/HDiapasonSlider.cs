using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  //public class HDiapasonSlider : ExtensionContainer, IHtmlControl
  //{
  //  readonly HTone handler1Style;
  //  readonly HTone handler2Style;
  //  readonly HStyle[] pseudoClasses;

  //  public HDiapasonSlider(string dataName, HTone handler1Style, HTone handler2Style,
  //    params HStyle[] pseudoClasses) :
  //    base("HDiapasonSlider", dataName)
  //  {
  //    this.handler1Style = handler1Style;
  //    this.handler2Style = handler2Style;
  //    this.pseudoClasses = pseudoClasses;
  //  }

  //  static readonly HBuilder h = null;

  //  public HElement ToHtml(string cssClassName, StringBuilder css)
  //  {
  //    HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
  //    foreach (HStyle pseudo in pseudoClasses)
  //      HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

  //    List<object> elements = new List<object>();
  //    elements.Add(new HAttribute("id", cssClassName));
  //    //elements.Add(h.data("name", Name));
  //    //elements.Add(h.data("id", cssClassName));

  //    return h.Div(h.@class("slider nativeMultiple"),
  //      h.Div(
  //        h.@class("nativeMultiple-one"), h.style("width: 131.444px;"),
  //        h.Div(
  //          h.style("width: 398px;"),
  //          h.Input(
  //            new HAttribute("min", "0"), new HAttribute("max", "180"),
  //            h.value("0,70"), new HAttribute("name", "three"), h.type("range")
  //          )
  //        )
  //      ),
  //      h.Div(
  //        h.@class("nativeMultiple-two"),
  //        h.Div(
  //          h.style("width: 398px;"),
  //          h.Input(
  //            new HAttribute("min", "0"), new HAttribute("max", "180"),
  //            h.value("0,70"), h.type("range")
  //          )
  //        )
  //      ),
  //      h.Script(h.Raw(@"
  //        $('input[name=three]').nativeMultiple({
  //            stylesheet: 'slider',
  //            onCreate: function() {
  //                  console.log(this);
  //                },
  //            onChange: function(first_value, second_value) {
  //                  console.log('onchange', [first_value, second_value]);
  //                },
  //            onSlide: function(first_value, second_value) {
  //                  console.log('onslide', [first_value, second_value]);
  //                }
  //              });
  //        "
  //      ))
  //    );

  //    return h.Div(
  //      HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
  //    );
  //  }
  //}

  //public class HDiapasonSlider : ExtensionContainer, IHtmlControl
  //{
  //  readonly HTone handler1Style;
  //  readonly HTone handler2Style;
  //  readonly HStyle[] pseudoClasses;

  //  public HDiapasonSlider(string dataName, HTone handler1Style, HTone handler2Style,
  //    params HStyle[] pseudoClasses) :
  //    base("HDiapasonSlider", dataName)
  //  {
  //    this.handler1Style = handler1Style;
  //    this.handler2Style = handler2Style;
  //    this.pseudoClasses = pseudoClasses;
  //  }

  //  static readonly HBuilder h = null;

  //  public HElement ToHtml(string cssClassName, StringBuilder css)
  //  {
  //    HtmlHlp.AddExtensionsToCss(css,
  //      handler1Style.CssExtensions,
  //      ".{0} .ui-slider-handle ",
  //      cssClassName
  //    );

  //    HtmlHlp.AddExtensionsToCss(css,
  //      handler2Style.CssExtensions,
  //      ".{0} .ui-slider-handle + .ui-slider-handle",
  //      cssClassName
  //    );

  //    HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
  //    foreach (HStyle pseudo in pseudoClasses)
  //      HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

  //    List<object> elements = new List<object>();
  //    elements.Add(new HAttribute("id", cssClassName));
  //    //elements.Add(h.data("name", Name));
  //    //elements.Add(h.data("id", cssClassName));

  //    return h.Div(
  //      HtmlHlp.ContentForHElement(this, cssClassName, elements.ToArray())
  //    );
  //  }
  //}
}
