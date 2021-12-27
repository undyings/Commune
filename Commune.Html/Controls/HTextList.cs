using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
	public class HTextList : ExtensionContainer, IHtmlControl
	{
		readonly string[] listItems;
		public HTextList(string dataName, params string[] listItems) :
			base("HTextList", dataName)
		{
			this.listItems = listItems;
		}

		static readonly HBuilder h = null;

		public HElement ToHtml(string cssClassName, StringBuilder css)
		{
			DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
			defaults.InlineBlock();

			HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

			HElement[] options = ArrayHlp.Convert(listItems, delegate (string li) { return h.Option(li); });

			string listName = string.Format("{0}_list", Name);

			return h.Div(
				HtmlHlp.ContentForHElement(this, cssClassName,
					h.Input(h.type("text"), new HAttribute("Id", Name), h.data("name", Name), new HAttribute("list", listName)),
					new HElement("datalist", new HAttribute("Id", listName), options)
				)
			);
		}
	}
}
