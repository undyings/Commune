using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Windows.Forms;

namespace Commune.Task
{
  public class UIPullThread : IForcePullThread
  {
    readonly object lockObj = new object();

    readonly TaskQueue taskQueue = new TaskQueue();

    public int TaskCount
    {
      get { return taskQueue.Count; }
    }

    readonly System.Windows.Forms.Timer timer;
    public readonly Control Control;
    public UIPullThread(Control control)
    {
      this.Control = control;
      this.timer = new System.Windows.Forms.Timer();
      timer.Interval = 50;
      timer.Tick += delegate
      {
        try
        {
          Task task = null;
          lock (lockObj)
          {
            int timeout;
            task = taskQueue.GetCurrentTask(out timeout);
            if (task == null)
            {
              timer.Interval = Math.Max(1, timeout);
              return;
            }
            timer.Interval = 1;
          }

          task.ExecuteStep();

          lock (lockObj)
          {
            taskQueue.FinishStep(task);
          }
        }
        catch (Exception ex)
        {
          Logger.WriteException(ex);
        }
      };

      this.Control.Disposed += delegate
      {
        timer.Stop();
        timer.Dispose();
      };

      timer.Start();
    }

    public void AddTask(Task task)
    {
      if (task == null)
        return;

      lock (lockObj)
      {
        taskQueue.AddTask(task);
        if (Control.IsHandleCreated)
          Control.BeginInvoke(new Executter(delegate { timer.Interval = 1; }));
        //timer.Interval = 1;
      }
    }

    public void ForceTask(Task task)
    {
      if (task == null)
        return;

      lock (lockObj)
      {
        taskQueue.ForceTask(task);

        if (Control.IsHandleCreated)
          Control.BeginInvoke(new Executter(delegate 
            { 
              timer.Interval = 1;
            }));
        //timer.Interval = 1;
      }
    }

    public LabeledThreadStatus Status
    {
      get
      {
        return taskQueue.GetStatus("UI");
      }
    }
  }
}
