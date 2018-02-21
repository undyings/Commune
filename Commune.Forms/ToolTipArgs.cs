using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Commune.Forms
{
  public class ToolTipArgs
  {
    public readonly Point MouseScreenLocation;
    public readonly Point ToolTipShift;
    public readonly string ToolTipCaption;

    public ToolTipArgs(Point mouseScreenLocation, Point toolTipShift, string toolTipCaption)
    {
      this.MouseScreenLocation = mouseScreenLocation;
      this.ToolTipShift = toolTipShift;
      this.ToolTipCaption = toolTipCaption;
    }
  }
}
