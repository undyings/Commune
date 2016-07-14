using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;

namespace Commune.Task
{
  class TaskQueue
  {
    public LabeledThreadStatus GetStatus(string label)
    {
      DateTime lastStartingTaskTime = LastStartingTaskTime;
      DateTime lastFinishingTaskTime = LastFinishingTaskTime;
      TimeSpan? workingCurrentStepTime = null;
      if (lastStartingTaskTime > lastFinishingTaskTime)
        workingCurrentStepTime = DateTime.UtcNow - lastStartingTaskTime;
      return new LabeledThreadStatus(label, Count, workingCurrentStepTime);
    }

    class TaskComparer : IComparer<TaskWithTicks>
    {
      public static TaskComparer Default = new TaskComparer();
      public int Compare(TaskWithTicks task1, TaskWithTicks task2)
      {
        return task1.Ticks.CompareTo(task2.Ticks);
      }
    }
    struct TaskWithTicks
    {
      public readonly long Ticks;
      public readonly Task Task;
      public TaskWithTicks(long ticks, Task task)
      {
        this.Ticks = ticks;
        this.Task = task;
      }
    }

    readonly List<TaskWithTicks> sortedTasks = new List<TaskWithTicks>();

    void InsertTask(Task task, DateTime nextExecuteTime)
    {
      TaskWithTicks taskWithTicks = new TaskWithTicks(Math.Max(
        nextExecuteTime.Ticks, DateTime.UtcNow.Ticks), task);

      int position = sortedTasks.BinarySearch(taskWithTicks, TaskComparer.Default);
      if (position < 0)
        position = ~position;
      sortedTasks.Insert(position, taskWithTicks);
    }

    DateTime lastStartingTaskTime = DateTime.UtcNow;
    public DateTime LastStartingTaskTime
    {
      get
      {
        return lastStartingTaskTime;
      }
    }
    DateTime lastFinishingTaskTime = DateTime.UtcNow;
    public DateTime LastFinishingTaskTime
    {
      get
      {
        return lastFinishingTaskTime;
      }
    }

    public void AddTask(Task task)
    {
      if (task == null)
        return;

      InsertTask(task, DateTime.UtcNow);
    }

    readonly Queue<Task> forceTasks = new Queue<Task>();
    public void ForceTask(Task task)
    {
      if (task.IsCompleted)
        return;

      forceTasks.Enqueue(task);
    }

    public Task GetCurrentTask(out int timeoutMilliseconds)
    {
      timeoutMilliseconds = 0;
      if (forceTasks.Count != 0)
      {
        Task task = forceTasks.Dequeue();
        for (int i = 0; i < sortedTasks.Count; ++i)
        {
          if (sortedTasks[i].Task == task)
          {
            sortedTasks.RemoveAt(i);
            break;
          }
        }
        lastStartingTaskTime = DateTime.UtcNow;
        return task;
      }

      if (sortedTasks.Count == 0)
      {
        timeoutMilliseconds = 3600 * 1000;
        return null;
      }

      TaskWithTicks taskWithTicks = sortedTasks[0];
      long waitTicks = taskWithTicks.Ticks - DateTime.UtcNow.Ticks;
      if (waitTicks > 10000)
      {
        timeoutMilliseconds = (int)(waitTicks / 10000);
        return null;
      }

      sortedTasks.RemoveAt(0);
      lastStartingTaskTime = DateTime.UtcNow;
      return taskWithTicks.Task;
    }

    public bool FinishStep(Task task)
    {
      lastFinishingTaskTime = DateTime.UtcNow;
      if (!task.IsCompleted)
      {
        //if (((int)task.MaxExecuteRate.TotalMilliseconds)%100 != 0)
        //  Logger.AddMessage("FinishStep: {0}, {1}", lastStartingTaskTime, task.MaxExecuteRate.TotalMilliseconds);
        this.InsertTask(task, lastStartingTaskTime.Add(task.MaxExecuteRate));
        return false;
      }
      return true;
    }

    public int Count
    {
      get 
      {
        return sortedTasks.Count; 
      }
    }
  }
}
