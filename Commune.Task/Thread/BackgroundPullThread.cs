using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Commune.Basis;

namespace Commune.Task
{
  public class BackgroundPullThread : IForcePullThread
  {
    public LabeledThreadStatus Status
    {
      get
      {
        return taskQueue.GetStatus(Label);
      }
    }

    readonly object lockObj = new object();

    public readonly string Label;
    readonly Thread thread;

//    Queue<Task> tasks = new Queue<Task>();
    public void AddTask(Task task)
    {
      if (task == null)
        return;

      lock (lockObj)
      {
        taskQueue.AddTask(task);
        waitHandle.Set();
      }
    }

    public void ForceTask(Task task)
    {
      if (task == null)
        return;

      lock (lockObj)
      {
        taskQueue.ForceTask(task);
        waitHandle.Set();
      }
    }

    public volatile bool IsFinishing = false;

    readonly TaskQueue taskQueue = new TaskQueue();

    public int TaskCount
    {
      get { return taskQueue.Count; }
    }

    void ThreadHandler()
    {
      try
      {
        int? timeoutMilliseconds = null;
        while (!IsFinishing)
        {
          if (timeoutMilliseconds != null)
          {
            waitHandle.WaitOne(timeoutMilliseconds.Value, false);
            timeoutMilliseconds = null;
          }

          Task task = null;
          lock (lockObj)
          {
            int timeout;
            task = taskQueue.GetCurrentTask(out timeout);
            if (task == null)
            {
              timeoutMilliseconds = timeout;
              continue;
            }
          }

          task.ExecuteStep();

          lock (lockObj)
          {
            bool isCompleted = taskQueue.FinishStep(task);
          }
        }
        Logger.AddMessage("Завершен поток с меткой '{0}'", Label);
      }
      catch (Exception e)
      {
        Logger.WriteException(e);
      }
    }

    readonly AutoResetEvent waitHandle = new AutoResetEvent(true);
    public BackgroundPullThread(ThreadLabel label)
    {
      this.Label = label.Label;
      thread = new Thread(ThreadHandler);
      thread.Priority = label.Priority;
      thread.IsBackground = true;
      thread.Start();
    }
  }
}
