using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Basis
{
  public struct PointD
  {
    public readonly double X;
    public readonly double Y;

    public PointD(double x, double y)
    {
      this.X = x;
      this.Y = y;
    }
  }
}
