using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Commune.Diagnostics
{
  public class SafeStopwatch
  {
    Stopwatch innerWatch = new Stopwatch();
    DateTime startUTC;
    DateTime stopUTC;
    //   TimeSpan totalUTC;
    TimeSpan total;

    public void Start()
    {
      startUTC = DateTime.UtcNow;
      innerWatch.Reset();
      innerWatch.Start();
    }

    public void Stop()
    {
      stopUTC = DateTime.UtcNow;
      innerWatch.Stop();

      if (Math.Abs(innerWatch.Elapsed.Ticks - (stopUTC - startUTC).Ticks) < 160000)
        total += innerWatch.Elapsed;
      else
        total += (stopUTC - startUTC);
    }

    public void Reset()
    {
      total = TimeSpan.Zero;
    }

    public TimeSpan Elapsed
    {
      get
      {
        //    stopUTC = DateTime.UtcNow;
        //    innerWatch.Stop();
        if (innerWatch.IsRunning)
        {
          if (Math.Abs(innerWatch.Elapsed.Ticks - (DateTime.UtcNow - startUTC).Ticks) < 160000)
            return innerWatch.Elapsed;
          else
            return (DateTime.UtcNow - startUTC);
        }
        else
          return total;
      }
    }
  }
}
