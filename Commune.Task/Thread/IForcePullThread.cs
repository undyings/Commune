using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Task
{
  public interface IForcePullThread
  {
    void ForceTask(Task task);
  }

  public interface IForceTask
  {
    void Initialize(IForcePullThread pullThread);
  }
}
