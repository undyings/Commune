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
  public class VirtualPropertyGridDrawSettings : VirtualGridDrawSettings
  {
    public readonly GridColumn<IGridColumn> DefaultCaptionColumn = new GridColumn<IGridColumn>("",
      delegate (IGridColumn column) { return SynchronizerHlp.GetDisplayName(column); }, null);

    Brush oddCaptionBackBrush = Brushes.White;
    public Brush OddCaptionBackBrush
    {
      get { return oddCaptionBackBrush; }
      set
      {
        if (value != null)
          oddCaptionBackBrush = value;
      }
    }
    Brush evenCaptionBackBrush = Brushes.White;
    public Brush EvenCaptionBackBrush
    {
      get { return evenCaptionBackBrush; }
      set
      {
        if (value != null)
          evenCaptionBackBrush = value;
      }
    }


    public VirtualPropertyGridDrawSettings()
      : base()
    {
    }
  }

  public class VirtualGridDrawSettings
  {
    public bool LeftBorderPaintDisabled;

    private Pen gridBorderPen = Pens.DimGray;
    public Pen GridBorderPen
    {
      get
      {
        return this.gridBorderPen;
      }
      set
      {
        if (value == null)
          return;
        this.gridBorderPen = value;
      }
    }


    IGridCellFiller defaultGridCellFiller = TextGridCellFiller.Default;
    public IGridCellFiller DefaultGridCellFiller
    {
      get { return defaultGridCellFiller; }
      set
      {
        if (value != null)
          defaultGridCellFiller = value;
      }
    }
    Font defaultGridFont = new Font("Microsoft Sans Serif", 8);
    public Font DefaultGridFont
    {
      get { return defaultGridFont; }
      set
      {
        if (value != null)
          defaultGridFont = value;
      }
    }
    Pen gridMarkingPen = Pens.DimGray;
    public Pen GridMarkingPen
    {
      get { return gridMarkingPen; }
      set
      {
        if (value != null)
          gridMarkingPen = value;
      }
    }

    public bool FillRowBackWithMarking = false;

    Brush oddRowBackBrush = Brushes.White;
    public Brush OddRowBackBrush
    {
      get { return oddRowBackBrush; }
      set
      {
        if (value != null)
          oddRowBackBrush = value;
      }
    }
    Brush evenRowBackBrush = Brushes.White;
    public Brush EvenRowBackBrush
    {
      get { return evenRowBackBrush; }
      set
      {
        if (value != null)
          evenRowBackBrush = value;
      }
    }
    Brush inactiveFilterBackBrush = Brushes.Gainsboro;
    public Brush InactiveFilterBackBrush
    {
      get { return inactiveFilterBackBrush; }
      set
      {
        if (value != null)
          inactiveFilterBackBrush = value;
      }
    }
    Brush selectedCellsBrush = new SolidBrush(Color.FromArgb(60, Color.Indigo));
    public Brush SelectedCellsBrush
    {
      get { return selectedCellsBrush; }
      set
      {
        if (value != null)
          selectedCellsBrush = value;
      }
    }

    public VirtualGridDrawSettings()
    {
    }

    public void ResetEventRowBackBrush()
    {
      this.evenRowBackBrush = this.oddRowBackBrush;
    }
  }
}
