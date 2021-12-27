using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commune.Basis;
using NitroBolt.Wui;

namespace Commune.Html
{
	public class HInputLabel : ExtensionContainer, IHtmlControl
	{
		readonly string identifier;
		readonly string caption;
		readonly HStyle[] pseudoClasses;
		public HInputLabel(string identifier, object value, params HStyle[] pseudoClasses) :
			base("HInputLabel", "")
		{
			this.identifier = identifier;
			this.caption = value != null ? value.ToString() : "";
			this.pseudoClasses = pseudoClasses;
		}

		static readonly HBuilder h = null;

		public HElement ToHtml(string cssClassName, StringBuilder css)
		{
			//DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
			//defaults.InlineBlock();

			HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

			foreach (HStyle pseudo in pseudoClasses)
				HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

			HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

			HAttribute forAttr = new HAttribute("for", identifier);

			return new HElement("label", HtmlHlp.ContentForHElement(this, cssClassName, forAttr, caption));
		}
	}

	//public class HInputLabel : ExtensionContainer, IHtmlControl
	//{
	//	readonly IHtmlControl input;
	//	readonly string caption;
	//	readonly HStyle[] pseudoClasses;
	//	public HInputLabel(IHtmlControl input, object value, params HStyle[] pseudoClasses) :
	//		base("HInputLabel", "")
	//	{
	//		this.input = input;
	//		this.caption = value != null ? value.ToString() : "";
	//		this.pseudoClasses = pseudoClasses;
	//	}

	//	static readonly HBuilder h = null;

	//	public HElement ToHtml(string cssClassName, StringBuilder css)
	//	{
	//		//DefaultExtensionContainer defaults = new DefaultExtensionContainer(this);
	//		//defaults.InlineBlock();

	//		HtmlHlp.AddClassToCss(css, cssClassName, CssExtensions);

	//		foreach (HStyle pseudo in pseudoClasses)
	//			HtmlHlp.AddStyleToCss(css, cssClassName, pseudo);

	//		HtmlHlp.AddMediaToCss(css, cssClassName, MediaExtensions);

	//		HElement inputElement = input.ToHtml(input.Name, css);

	//		return new HElement("label", HtmlHlp.ContentForHElement(this, cssClassName, inputElement, caption));
	//	}
	//}
}
