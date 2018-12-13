using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Commune.Basis;

namespace Commune.Task
{
  public class UITaskPull
  {
    readonly UIPullThread uiThread;
    readonly TaskPull taskPull;
    readonly ThreadLabel backgroundLabel;

    public UITaskPull(Control control, TaskPull taskPull, ThreadLabel backgroundLabel)
    {
      this.uiThread = new UIPullThread(control);
      this.taskPull = taskPull;
      this.backgroundLabel = backgroundLabel;
    }

    public LabeledThreadStatus Status
    {
      get
      {
        return uiThread.Status;
      }
    }

    public Control UIControl
    {
      get { return uiThread.Control; }
    }

    public Task StartUITask(IEnumerable<Step> taskSteps)
    {
      return StartUITask(new Task(taskSteps));
    }

    public Task StartUITask(Task task)
    {
      if (uiThread == null)
        throw new Exception("Запуск задач в главном потоке не доступен, т.к. при создании taskPull не был передан контрол");

      ((IForceTask)task).Initialize(uiThread);
      uiThread.AddTask(task);
      return task;
    }

    public Task StartBackgroundTask(IEnumerable<Step> taskSteps)
    {
      return taskPull.StartTask(backgroundLabel, new Task(taskSteps));
    }

    public Task StartBackgroundTask(Task task)
    {
      return taskPull.StartTask(backgroundLabel, task);
    }
  }
}
