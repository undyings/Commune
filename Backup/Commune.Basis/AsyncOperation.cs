using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Commune.Diagnostics;

namespace Commune.Basis
{
  public class AsyncAction<TResult, TError> : IDisposable
  {
    readonly object lockObj = new object();

    public volatile bool ActionCanceled = false;
    public bool AsyncContinue
    {
      get { return IsSuccessful == null; }
    }
    TResult result = default(TResult);
    public TResult Result
    {
      get
      {
        lock (lockObj)
          return result;
      }
    }

    public bool SetResult(TResult result)
    {
      lock (lockObj)
        this.result = result;
      return true;
    }

    TError error = default(TError);
    public TError Error
    {
      get
      {
        lock (lockObj)
          return error;
      }
    }

    public bool SetError(TError error)
    {
      lock (lockObj)
        this.error = error;
      return false;
    }

    bool? isSuccessful = null;
    public bool? IsSuccessful
    {
      get
      {
        lock (lockObj)
          return isSuccessful;
      }
    }

    public readonly string ActionName;
    readonly Getter<bool, AsyncAction<TResult, TError>> action;
    readonly Executter finisher;
    readonly System.Timers.Timer asyncTimer = new System.Timers.Timer();
    readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
    public AsyncAction(Getter<bool, AsyncAction<TResult, TError>> action, Executter finisher) :
      this("", action, finisher)
    {
    }
    public AsyncAction(string actionName, Getter<bool, AsyncAction<TResult, TError>> action,
      Executter finisher)
    {
      this.ActionName = actionName;
      this.action = action;
      this.finisher = finisher;

      asyncTimer.AutoReset = false;
      asyncTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

      asyncTimer.Start();

      timer.Interval = 100;
      timer.Enabled = true;
      timer.Tick += delegate
      {
        if (AsyncContinue)
          return;

        timer.Stop();
        if (finisher != null)
          finisher();
      };
    }

    void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      try
      {
        bool result = action(this);
        lock (lockObj)
          this.isSuccessful = result;
      }
      catch (Exception ex)
      {
        Logger.WriteException(ex, "Необработанная ошибка в AsyncOperation");
        lock (lockObj)
          isSuccessful = false;
      }
      finally
      {
        //finisher();
      }
    }

    public void Dispose()
    {
      asyncTimer.Elapsed -= timer_Elapsed;
      asyncTimer.Stop();
      asyncTimer.Dispose();

      timer.Dispose();
    }
  }
}
