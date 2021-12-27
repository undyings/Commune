using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
  public class HComboEdit<T> : ExtensionContainer, IHtmlControl
  {
    //readonly T selected;
    //readonly Getter<string, T> displayGetter;
    //readonly T[] comboItems;
    public HComboEdit(string dataName, T selected, Getter<string, T> displayGetter, params T[] comboItems) :
      base("HComboEdit", dataName)
    {
      this.selected = selected;
      this.comboItems = ArrayHlp.Convert(comboItems, delegate (T comboItem)
        {
          string value = displayGetter != null ? displayGetter(comboItem) : comboItem?.ToString();
          return _.Tuple(comboItem, value);
        }
      );

      //this.selected = selected;
      //this.displayGetter = displayGetter ?? new Getter<string, T>(delegate (T item) { return StringHlp.ToString(item); });
      //this.comboItems = comboItems;
    }

    readonly T selected;
    readonly Tuple<T, string>[] comboItems;

    public HComboEdit(string dataName, T selected, params Tuple<T, string>[] comboItems) :
      base("HComboEdit", dataName)
    {
      this.selected = selected;
      this.comboItems = comboItems;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.Cursor(CursorStyle.Pointer);

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

			HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

			// hack чтобы в IE скрывалась оригинальная стрелочка
			if (this.GetExtended("appearance") as string == "none")
			{
				HtmlHlp.AddClassToCss(css, ".{0}::-ms-expand", 
					new CssExtensionAttribute[] { new CssExtensionAttribute("display", "none") }
				);
			}

			HElement[] options = ArrayHlp.Convert(comboItems, delegate (Tuple<T, string> item)
      {
        object[] content = new object[] { h.value(item.Item1), item.Item2 };
        if (_.Equals(selected, item.Item1))
          content = ArrayHlp.Merge(content, new object[] { h.selected() });
        return h.Option(content);
      });

      return h.Select(HtmlHlp.ContentForHElement(this, cssClassName, h.data("name", Name), options)
      );
    }
  }

  //public class HComboEdit<T> : ExtensionContainer, IHtmlControl
  //{
  //  readonly T selected;
  //  readonly Getter<string, T> displayGetter;
  //  readonly T[] comboItems;
  //  public HComboEdit(string dataName, T selected, Getter<string, T> displayGetter, params T[] comboItems) :
  //    base("HComboEdit", dataName)
  //  {
  //    this.selected = selected;
  //    this.displayGetter = displayGetter ?? new Getter<string, T>(delegate(T item) { return StringHlp.ToString(item); });
  //    this.comboItems = comboItems;
  //  }

  //  static readonly HBuilder h = null;

  //  public HElement ToHtml(string cssClassName, StringBuilder css)
  //  {
  //    DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
  //    defaults.Cursor(CursorStyle.Pointer);

  //    HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

  //    HElement[] options = ArrayHlp.Convert(comboItems, delegate(T item)
  //    {
  //      object[] content = new object[] { h.value(item), displayGetter(item) };
  //      if (_.Equals(selected, item))
  //        content = ArrayHlp.Merge(content, new object[] { h.selected() });
  //      return h.Option(content);
  //    });

  //    return h.Select(HtmlHlp.ContentForHElement(this, cssClassName, h.data("name", Name), options)
  //    );
  //  }
  //}
}
