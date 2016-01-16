using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Data
{
  public class LinkConstraint
  {
    public readonly bool IsForeign;
    public readonly Getter<bool, IRowLink> IsValidRow;

    public LinkConstraint(Getter<bool, IRowLink> isValidRow) :
      this(false, isValidRow)
    {
    }

    public LinkConstraint(bool isForeign, Getter<bool, IRowLink> isValidRow)
    {
      this.IsForeign = isForeign;
      this.IsValidRow = isValidRow;
    }
  }
}
