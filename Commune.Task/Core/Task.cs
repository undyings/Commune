using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Task
{
  public class Task : IForceTask
  {
    readonly IEnumerable<Step> steps;
    readonly IEnumerator<Step> stepsEnumerator;
    volatile Exception error = null;
    object result = null;
    public T Finish<T>()
    {
      if (!isCompleted)
        throw new Exception("Задача еще не завершена");

      if (error != null)
      {
        //Logger.WriteException(error);
        throw error;
      }
      return (T)result;
    }
    volatile bool isCompleted = false;
    public bool IsCompleted
    {
      get
      {
        return isCompleted;
      }
    }

    public bool IsError
    {
      get
      {
        return error != null;
      }
    }

    void CompleteTask()
    {
      isCompleted = true;
      try
      {
        stepsEnumerator.Dispose();
        if (steps is IDisposable)
          ((IDisposable)steps).Dispose();
      }
      catch (Exception ex)
      {
        Logger.WriteException(ex);
      }

      if (completedInformer != null)
        completedInformer();
    }

    public bool IsCanceled
    {
      get { return error is OperationCanceledException; }
    }

    volatile WaitStep waitStep = null;
    public void ExecuteStep()
    {
      try
      {
        if (isCompleted)
          return;

        if (Canceler != null && Canceler.Cancelation)
        {
          error = new OperationCanceledException("");
          CompleteTask();
          return;
        }

        if (waitStep != null && !waitStep.IsEnd)
        {
          return;
        }

        bool isContinue = stepsEnumerator.MoveNext();
        if (!isContinue)
        {
          CompleteTask();
        }
        else
        {
          Step step = stepsEnumerator.Current;
          if (step is FinishStep)
          {
            result = ((FinishStep)step).Result;
            CompleteTask();
          }
          else if (step is WaitStep)
          {
            waitStep = (WaitStep)step;
            return;
          }
        }
      }
      catch (Exception exc)
      {
        Logger.WriteException(exc);
        error = exc;
        CompleteTask();
      }
    }

    public TimeSpan MaxExecuteRate
    {
      get
      {
        if (waitStep != null)
          return waitStep.MaxExecuteRate;
        return TimeSpan.Zero;
      }
    }

    public void Force()
    {
      if (pullThread != null)
        pullThread.ForceTask(this);
    }

    public Task(IEnumerable<Step> steps) :
      this(steps, null, null)
    {
    }

    public Task(IEnumerable<Step> steps, Task completedInformTask) :
      this(steps, null, delegate { completedInformTask.Force(); })
    {
    }

    readonly Executter completedInformer;
    public readonly ICanceler Canceler;
    public Task(IEnumerable<Step> steps, ICanceler canceler, Executter completedInformer)
    {
      this.steps = steps;
      this.stepsEnumerator = steps.GetEnumerator();
      this.Canceler = canceler;
      this.completedInformer = completedInformer;
    }

    IForcePullThread pullThread = null;
    void IForceTask.Initialize(IForcePullThread pullThread)
    {
      this.pullThread = pullThread;
    }
  }
}
