using System;
using System.Collections.Generic;
using System.Text;
using Commune.Diagnostics;

namespace Commune.Basis
{
  public class TestStopwatch : IDisposable
  {
    readonly SafeStopwatch innerWatch = new SafeStopwatch();
    readonly StringBuilder traceMessage = new StringBuilder();
    public TestStopwatch(string formatMessage, params object[] args)
    {
      traceMessage.AppendFormat(formatMessage, args);
      innerWatch.Reset();
      innerWatch.Start();
    }

    public void AddMessage(string formatMessage, params object[] args)
    {
      traceMessage.AppendFormat(formatMessage, args);
    }

    public void Finish(string formatMessage, params object[] args)
    {
      AddMessage(formatMessage, args);
      Dispose();
    }

    public void Dispose()
    {
      innerWatch.Stop();
      Logger.AddMessage("{0}: {1} мс", traceMessage.ToString(), innerWatch.Elapsed.TotalMilliseconds);
    }
  }
}
