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
    readonly T selected;
    readonly Getter<string, T> displayGetter;
    readonly T[] comboItems;
    public HComboEdit(string dataName, T selected, Getter<string, T> displayGetter, params T[] comboItems) :
      base("HComboEdit", dataName)
    {
      this.selected = selected;
      this.displayGetter = displayGetter ?? new Getter<string, T>(delegate(T item) { return StringHlp.ToString(item); });
      this.comboItems = comboItems;
    }

    static readonly HBuilder h = null;

    public HElement ToHtml(string cssClassName, StringBuilder css)
    {
      DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
      defaults.Cursor(CursorStyle.Pointer);

      HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);
      HtmlHlp.AddHoverToCss(css, cssClassName, this);

      HElement[] options = ArrayHlp.Convert(comboItems, delegate(T item)
      {
        object[] content = new object[] { h.value(item), displayGetter(item) };
        if (_.Equals(selected, item))
          content = ArrayHlp.Merge(content, new object[] { h.selected() });
        return h.Option(content);
      });

      return h.Select(HtmlHlp.ContentForHElement(this, cssClassName, h.data("name", Name), options)
      );
    }

    public IHtmlControl[] Controls
    {
      get { return Array<IHtmlControl>.Empty; }
    }
  }
}
