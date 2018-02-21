using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Commune.Forms
{
  public class VirtualPanel : Panel
  {
    public VirtualPanel()
      :
      base()
    {
      base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
        ControlStyles.UserPaint, true);
    }
  }
}