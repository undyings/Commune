using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;
using Commune.Diagnostics;

namespace Commune.Basis
{
  /// <summary>
  /// Класс управления процессом логгирования
  /// </summary>
  public class Logger
  {
    static Log Current = new Log();

    private static EnhancedTraceListener _Listener = null;
    
    public static void EnableLogging()
    {
      try
      {
        if (_Listener == null)
          _Listener = new EnhancedTraceListener(Current);

        //			    //cheated : обращение к переменной создаёт одиночку
        //			    Logging.Log.Current.GetType();
        System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
      }
      catch
      {
      }
    }

    public static void EnableLogging(string logFilePath, int maxLogSizeInMegaBytes)
    {
      Current = new Log(logFilePath, maxLogSizeInMegaBytes * 1024 * 1024);
      EnableLogging();
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Logger.Write("Необработанное исключение: " + e.ExceptionObject);
    }
    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      Logger.WriteException(e.Exception);
    }

        /// <summary>
    /// Пишет в лог форматированный текст, при этом не выкидывает наверх исключение, 
    /// если есть ошибки в формате
    /// </summary>
    /// <param name="formatText"></param>
    /// <param name="args"></param>
    public static void AddMessage(string formatText, params object[] args)
    {
      try
      {
        //Trace.WriteLine(getFormatString(format, args)); //string.Format(format, args));
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()).WithText(getFormatString(formatText, args)));
      }
      catch
      {
      }
    }

    public static bool IsTraceMessageWithMethod = false;
    public static bool IsTraceMessageWithThread = false;

    public static void IsTraceMessageWithExtraInfo()
    {
      IsTraceMessageWithMethod = true;
      IsTraceMessageWithThread = true;
    }
 
    private static TraceMessage CreateDefaultTraceMessage(StackTrace trace)
    {
      TraceMessage message = new TraceMessage();
      if (Logger.IsTraceMessageWithMethod && trace.FrameCount >= 2)
        message.WithMethod(trace.GetFrame(1).GetMethod());
      if (Logger.IsTraceMessageWithThread)
        message.WithThread();
      return message;
    }
    private static string getFormatString(string format, params object[] args)
    {
      try
      {
        if (args == null || args.Length == 0)
          return format;
        return string.Format(format, args);
      }
      catch (Exception exc)
      {
        return string.Format(@"Ошибка форматирования: '{1}' в '{0}', {2}",
          format, exc.Message, new StackTrace());
      }
    }
    public static void WriteMethod()
    {
      try
      {
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()));
      }
      catch
      {
      }
    }
 
    public static void WriteException(Exception exc)
    {
      try
      {
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()).WithExc(exc));
      }
      catch (Exception exc2)
      {
        Console.Error.WriteLine(exc2);
      }
    }
    public static void WriteException(Exception exc, object obj)
    {
      try
      {
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()).WithExc(exc)
          .WithHash(obj != null ? obj.GetHashCode() : 0));
      }
      catch
      {
      }
    }
    public static void WriteException(Exception ex, string formatText, params object[] args)
    {
      try
      {
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()).
          WithText(string.Format("{0}{1}{2}", getFormatString(formatText, args),
          Environment.NewLine, ex)));
      }
      catch
      {
      }
    }

    public static void Write(string formatText)
    {
      try
      {
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()).WithText(formatText));
        //Write(formatText, null);
      }
      catch
      {
      }
    }

    public static void Write(string formatText, params object[] args)
    {
      try
      {
        //todo добавить время вывода сообщения
        //Trace.WriteLine(getFormatString(formatText, args));
        Trace.WriteLine(CreateDefaultTraceMessage(new StackTrace()).WithText(getFormatString(formatText, args)));
      }
      catch (Exception)
      {
      }
    }

    public static void MakeConsoleTraceListener()
    {
      Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
    }
    public static void MakeFileTraceListener()
    {
      Directory.CreateDirectory("Logs");
      Trace.AutoFlush = true;
      Trace.Listeners.Add(GetListener("Logs"));
      Trace.AutoFlush = true;
    }
    public static void MakeFileTraceListener(string dir, string name)
    {
      string path = Path.Combine(dir, "Logs");
      Directory.CreateDirectory(path);
      Trace.AutoFlush = true;
      Trace.Listeners.Add(GetListener(path, name));
    }

    static TextWriterTraceListener GetListener(string dir)
    {
      string name = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
      return GetListener(dir, name);
    }


    static TextWriterTraceListener GetListener(string dir, string name)
    {
      for (int i = 0; i < 10; ++i)
      {
        try
        {
          return new TextWriterTraceListener(Path.Combine(dir, string.Format("{0}.{1}.log", name, i)));
        }
        catch (Exception)
        {
        }
      }
      return new TextWriterTraceListener(Path.Combine (dir, string.Format("{0}.{1}.log", name, DateTime.Now.Ticks)));
    }
    public class TraceArg
    {
      public TraceArg(string name, object value)
        :
        this(name, value , null)
      {
      }
      public TraceArg(string name, object value, string measuringUnit)
      {
        this.Name = name;
        this.Value = value;
        this.MeasuringUnit = measuringUnit;
      }
      public readonly string Name;
      public readonly object Value;
      public readonly string MeasuringUnit;
    }
  }
}
