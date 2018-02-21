using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Commune.Basis;
using System.Drawing.Drawing2D;

namespace Commune.Forms
{
  public interface ISimpleScroll
  {
    Control Control { get; }
    int Minumum { get; set; }
    int Maximum { get; set; }
    int LargeChange { get; set; }
    int SmallChange { get; set; }
    int Value { get; set; }
    event EventHandler ValueChanged;
  }

  public class WindowsFormsScroll : ISimpleScroll
  {
    readonly ScrollBar scrollControl;
    public Control Control
    {
      get { return scrollControl; }
    }

    public WindowsFormsScroll(ScrollBar scrollControl)
    {
      this.scrollControl = scrollControl;
    }

    public int Minumum
    {
      get
      {
        return scrollControl.Minimum;
      }
      set
      {
        scrollControl.Minimum = value;
      }
    }

    public int Maximum
    {
      get
      {
        return scrollControl.Maximum;
      }
      set
      {
        scrollControl.Maximum = value;
      }
    }

    public int LargeChange
    {
      get
      {
        return scrollControl.LargeChange;
      }
      set
      {
        scrollControl.LargeChange = value;
      }
    }

    public int SmallChange
    {
      get
      {
        return scrollControl.SmallChange;
      }
      set
      {
        scrollControl.SmallChange = value;
      }
    }

    public int Value
    {
      get
      {
        return scrollControl.Value;
      }
      set
      {
        scrollControl.Value = value;
      }
    }

    public event EventHandler ValueChanged
    {
      add
      {
        scrollControl.ValueChanged += value;
      }
      remove
      {
        scrollControl.ValueChanged -= value;
      }
    }
  }
}
