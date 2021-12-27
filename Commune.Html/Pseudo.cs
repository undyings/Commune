using Commune.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commune.Html
{
  public class HHover : HStyle
  {
    public HHover()
      : base(".{0}:hover")
    {
    }
  }

  public class HActive : HStyle
  {
    public HActive()
      : base(".{0}:active")
    {
    }
  }

  public class HFocus : HStyle
  {
    public HFocus()
      : base(".{0}:focus")
    {
    }
  }

  public class HBefore : HStyle
  {
    public HBefore()
      : base(".{0}::before")
    {
    }
  }

  public class HAfter : HStyle
  {
    public HAfter() :
      base(".{0}::after")
    {
    }
  }

	public class HPlaceholder
	{
		readonly HTone style;
		public HPlaceholder(HTone style)
		{
			this.style = style;
		}

		public HStyle[] ToStyles()
		{
			string[] names = new string[] {
				"::placeholder",
				"::-ms-input-placeholder",
				":-ms-input-placeholder",
				"::-webkit-input-placeholder",
				"::-moz-placeholder"
			};

			return ArrayHlp.Convert(names, delegate (string name)
			{
				HStyle pseudo = new HStyle(".{0}" + name);
				foreach (CssExtensionAttribute ext in style.CssExtensions)
				{
					pseudo.WithExtension(ext);
				}
				return pseudo;
			}
			);

		}
	}

}
