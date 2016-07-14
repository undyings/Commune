using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Commune.Task
{
  public class ThreadLabel
  {
    public readonly string Label;
    public readonly ThreadPriority Priority;

    public ThreadLabel(string label, ThreadPriority priority)
    {
      this.Label = label;
      this.Priority = priority;
    }
  }
}
