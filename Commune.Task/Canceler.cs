using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Task
{
  public interface ICanceler
  {
    bool Cancelation { get; }
    void Cancel();
  }

  public class Canceler : ICanceler
  {
    volatile bool cancellation = false;
    public bool Cancelation
    {
      get
      {
        if (cancellation)
          return true;

        if (cancelArgGetter != null)
        {
          object arg = cancelArgGetter();
          if (!object.Equals(arg, cacheCancelArg))
          {
            cancellation = true;
          }
        }
        return cancellation;
      }
    }

    public void Cancel()
    {
      this.cancellation = true;
    }

    readonly Getter<object> cancelArgGetter;
    readonly object cacheCancelArg;

    public Canceler(Getter<object> cancelArgGetter)
    {
      this.cancelArgGetter = cancelArgGetter;
      this.cacheCancelArg = cancelArgGetter();
    }

    public Canceler()
      : this(null)
    {
    }
  }
}
