using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Commune.Task
{
  public class LabeledThreadStatus
  {
    public string Label;
    public int TaskCount;
    [XmlIgnore()]
    public readonly TimeSpan? WorkingCurrentStepTime;
    [XmlElement("WorkingCurrentStepTime")]
    public string InternalWorkingCurrentStepTime
    {
      get
      {
        if (WorkingCurrentStepTime != null)
          return WorkingCurrentStepTime.Value.ToString();
        return null;
      }
      set { }
    }

    public LabeledThreadStatus(string label, int taskCount, TimeSpan? workingCurrentStepTime)
    {
      this.Label = label;
      this.TaskCount = taskCount;
      this.WorkingCurrentStepTime = workingCurrentStepTime;
    }
    public LabeledThreadStatus() { }
  }
  public class TaskPullStatus
  {
    public int ThreadCount;
    public string MaxWorkingStepTime;
    public string[] HoverThreads = null;
    [XmlElement("Thread")]
    public LabeledThreadStatus[] Threads;

    public TaskPullStatus() { }
    public TaskPullStatus(int threadCount, TimeSpan maxWorkingStepTime, string[] hoverThreads, LabeledThreadStatus[] threads)
    {
      this.ThreadCount = threadCount;
      this.MaxWorkingStepTime = maxWorkingStepTime.ToString();
      if (hoverThreads != null && hoverThreads.Length != 0)
        this.HoverThreads = hoverThreads;
      this.Threads = threads;
    }
  }

}
