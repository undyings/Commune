using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Task
{
  interface IForcePullThread
  {
    void ForceTask(Task task);
  }

  interface IForceTask
  {
    void Initialize(IForcePullThread pullThread);
  }
}
