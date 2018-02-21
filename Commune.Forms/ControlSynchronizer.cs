using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Commune.Forms
{
  /// <summary>
  /// Базовый класс для синхронизации. Самостоятельно удаляется при умирании контрола
  /// В наследнике перекрыть все override-ы и при изменении Control-а вызывать UpdateTimer
  /// </summary>
  public abstract class ControlSynchronizer :
    IDisposable
  {
    protected abstract Control Control_ForSync { get; }
    protected abstract TimeSpan UpdateInterval_ForSync { get; }

    Timer timer;

    Control old_control;
    protected void UpdateTimer(bool isRemoveAndCreate)
    {
      if (this.timer != null)
      {
        timer.Tick -= new EventHandler(internal_timer_Tick);
        timer.Stop();
        timer.Dispose();
        timer = null;
      }
      if (old_control != null)
      {
        old_control.Disposed -= new EventHandler(old_control_Disposed);
        old_control = null;
      }

      if (isRemoveAndCreate && Control_ForSync != null && !Control_ForSync.IsDisposed)
      {
        old_control = Control_ForSync;
        old_control.Disposed += new EventHandler(old_control_Disposed);

        Timer timer = new Timer();
        timer.Interval = (int)UpdateInterval_ForSync.TotalMilliseconds;
        timer.Tick += new EventHandler(internal_timer_Tick);
        timer.Start();
        this.timer = timer;
      }

    }

    void internal_timer_Tick(object sender, EventArgs e)
    {
      try
      {
        timer_Tick(sender, e);
      }
      catch (Exception exc)
      {
        System.Diagnostics.Trace.WriteLine(exc);
      }
    }

    protected virtual void timer_Tick(object sender, EventArgs e)
    {
    }

    void old_control_Disposed(object sender, EventArgs e)
    {
      //UpdateTimer(false);
      Dispose();
    }

    public virtual void Dispose()
    {
      UpdateTimer(false);
    }
  }
}
