using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NitroBolt.Wui;
using Commune.Basis;
using System.Web;

namespace Commune.Html
{
  public static class SliderHlp
  {
    static readonly HBuilder h = null;

    public static string prevScript(string sliderName)
    {
      return string.Format("{0}_prev();", sliderName);
    }

    public static string nextScript(string sliderName)
    {
      return string.Format("{0}_next();", sliderName);
    }

    public static IHtmlControl[] PrepareSlides<T>(string sliderName, IEnumerable<T> units, 
      Getter<IHtmlControl, T> slideGetter, Getter<bool, T> isMaxSlide)
    {
      List<IHtmlControl> slideControls = new List<IHtmlControl>();
      int index = -1;
      foreach (T unit in units)
      {
        index++;
        // добавляем статично самый большой слайд, чтобы задать размер слайдера
        if (isMaxSlide(unit))
        {
          slideControls.Add(
            slideGetter(unit).Opacity("0")
          );
        }

        IHtmlControl slide = slideGetter(unit).PositionAbsolute().Left(0).Right(0).Top(0).Bottom(0)
            .ZIndex(index == 0 ? 1 : -1).ExtraClassNames(sliderName);

        if (index > 0)
          slide.Display("none");

        slideControls.Add(slide);
      }

			slideControls.Add(new HButton(string.Format("{0}_prev", sliderName), "")
				.Display("none").OnClick(string.Format("{0}_prev();", sliderName))
			);
			slideControls.Add(new HButton(string.Format("{0}_next", sliderName), "")
				.Display("none").OnClick(string.Format("{0}_next();", sliderName))
			);

			return slideControls.ToArray();
    }

    public static IHtmlControl GetSliderToggle(string sliderName, int slideCount,
      int size, int hMargin, string background, string selectedBackground,
			int mediaMaxWidth, int mediaSize, int mediaHMargin)
    {
      IHtmlControl[] buttons = new IHtmlControl[slideCount];
      for (int i = 0; i < buttons.Length; ++i)
      {
        buttons[i] = new HPanel(
          new HPanel().Height("100%").BorderRadius("50%").Background(selectedBackground)
            .Display(i == 0 ? "block" : "none")
            .ExtraClassNames(string.Format("{0}Toggle", sliderName))
        ).InlineBlock().Size(size, size).MarginRight(hMargin).CursorPointer()
          .BorderRadius("50%").Background(background)
          .OnClick(string.Format("{0}_set({1});", sliderName, i))
					.Media(mediaMaxWidth, new HStyle().Size(mediaSize, mediaSize).MarginRight(mediaHMargin));
      }

      return new HPanel(
        buttons
      );
    }

    public static IHtmlControl SliderScript(string sliderName, int intervalInMs)
    {
      return new HElementControl(
        h.Script(h.Raw(@"
function <<slider>>_init()
{
  if (window.<<slider>>Id != null)
    return;
  window.<<slider>>Index = 0;
  window.<<slider>>Tick = new Date().getTime();
  window.setTimeout(function <<slider>>_tick() {
   var interval = <<interval>>;
   try {
    var tick = new Date().getTime();
    var diff = tick - window.<<slider>>Tick;
    if (diff < interval)
      interval = interval - diff;
    else
      <<slider>>_next();
   }
   finally {
    window.setTimeout(<<slider>>_tick, interval);
   }    
  }, <<interval>>);
}
function <<slider>>_set(newIndex)
{
  window.<<slider>>Tick = new Date().getTime();
  var sliders = $('.<<slider>>');
  if (sliders.length == 0) return;
  var sliderIndex = window.<<slider>>Index;
  newIndex = (newIndex + sliders.length) % sliders.length;
  for(var i=0; i<sliders.length; i++)
  {
    var slider = sliders[i];
    if (i == sliderIndex)
    {
      slider.style.animationName = 'none';
      slider.style.display = 'none';
      slider.style.zIndex = 0;
    }
    else if (i == newIndex)
    {
      slider.style.animationName = '<<slider>>Animation';
      slider.style.display = 'block';
      slider.style.zIndex = 1;
    }
    else
    {
      slider.style.animationName = 'none';
      slider.style.display = 'none';
    }
  }
  window.<<slider>>Index = newIndex;

  var toggle = $('.<<slider>>Toggle');
  for (var i = 0; i < toggle.length; ++i)
  {
    if (i == newIndex)
      toggle[i].style.display = 'block';
    else
      toggle[i].style.display = 'none';
  }
}
function <<slider>>_next()
{
  <<slider>>_set(window.<<slider>>Index + 1);
}
function <<slider>>_prev()
{
  <<slider>>_set(window.<<slider>>Index - 1);
}

<<slider>>_init();
"
          .Replace("<<slider>>", sliderName).Replace("<<interval>>", intervalInMs.ToString())
        )),
        string.Format("{0}_script", sliderName)
      );
    }
  }
}
