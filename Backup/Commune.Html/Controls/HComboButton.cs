using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;
using System.Drawing;

namespace Commune.Html
{
  public class HComboButton : ExtensionContainer, IHtmlControl
  {
    readonly string caption;
    readonly IHtmlControl[] comboItems;
    public HComboButton(string caption, params IHtmlControl[] comboItems) :
      base("HComboButton", "")
    {
      this.caption = caption;
      this.comboItems = comboItems;
    }

    public HElement ToHtml(string cssPrefix, StringBuilder css)
    {
      throw new NotImplementedException();
    }

    public IHtmlControl[] Controls
    {
      get { return comboItems; }
    }

  }
}
