using System;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace Commune.Diagnostics
{
	public class EnhancedTraceListener : TraceListener
	{
		private Log log;
		public EnhancedTraceListener(Log log)	
		{
			this.log = log;
			Trace.Listeners.Add(this);
		}
		
		
		public override void Write(string message)
		{
      try
      {
        TraceMessage tMsg = new TraceMessage(message);
        log.ProcessNewMessage(tMsg);
      }
      catch
      {
      }
		}
		public override void WriteLine(string message)
		{
      try
      {
        TraceMessage tMsg = new TraceMessage(message);
        log.ProcessNewMessage(tMsg);
      }
      catch
      {
      }
		}

		public override void Write(object o)
		{
      try
      {
        TraceMessage tMsg = o as TraceMessage;
        if (tMsg == null)
        {
          tMsg = new TraceMessage(o);
        }
				
        log.ProcessNewMessage(tMsg);
      }
      catch
      {
      }
		}
		public override void WriteLine(object o)
		{
      try
      {
        //TraceHlp2.TraceMessage tMsg;
        //if (o is TraceHlp2.TraceMessage)
        //{
        //  o = (TraceMessage)(o as TraceHlp2.TraceMessage);
        //}

        TraceMessage tMsg = o as TraceMessage;
        if (tMsg == null)
        {
          tMsg = new TraceMessage(o);
        }
        log.ProcessNewMessage(tMsg);
      
      }
      catch
      {
      }
		}	
	}
}



