using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Commune.Forms
{
  public class NotifierWithDelay
  {
    public event EventHandler NotifyEvent;
    public readonly TimeSpan Delay;

    object cashedObj = null;
    DateTime? cashedTime = null;
    public void TakeUpEvent(object obj)
    {
      cashedObj = obj;
      cashedTime = DateTime.UtcNow;
      timer.Stop();
      timer.Start();
    }

    readonly Timer timer;
    public NotifierWithDelay(TimeSpan delay)
    {
      this.Delay = delay;
      timer = new Timer();
      timer.Interval = (int)(delay.TotalMilliseconds + 1);
      timer.Tick += new EventHandler(timer_Tick);
    }

    void timer_Tick(object sender, EventArgs e)
    {
      if (cashedTime != null && DateTime.UtcNow - cashedTime.Value > Delay)
      {
        NotifyEvent(cashedObj, EventArgs.Empty);
        cashedTime = null;
        timer.Stop();
      }
    }
  }
}
