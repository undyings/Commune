using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace Commune.Diagnostics
{
  public interface IFileLog
  {
    int MaxLogLength { get; set; }
    TimeSpan ScanPeriod { get; set; }
    void AppendAllLogs();
    string MainLogFileName { get; }
  }

  public class FileLog : IFileLog, IDisposable
  {
    public FileLog(string baseLogFilePath, long maxLogSize)
    {
      this.baseLogFilePath = baseLogFilePath;
      this.maxLogSize = maxLogSize;
      writer = CaptureLogFile(baseLogFilePath, out logFilePath);
    }
    readonly string baseLogFilePath;
    readonly string logFilePath;
    long maxLogSize;
    StreamWriter writer;
    readonly object writer_locker = new object();

    //public static string DefaultLogFilePath
    //{
    //  get
    //  {
    //    if (HttpContext.Current != null)
    //      return HttpContext.Current.Server.MapPath("");
    //    return Path.ChangeExtension(System.Reflection.Assembly.GetEntryAssembly().Location, ".log");
    //  }
    //}
    //public static readonly long DefaultMaxLogSize = 2 * 1024 * 1024;

    static StreamWriter CaptureLogFile(string baseLogFilePath, out string logFilePath)
    {
      string ext = Path.GetExtension(baseLogFilePath);

      const int fileCountLimit = 100;
      for (int i = 0; i < fileCountLimit; i++)
      {
        string path = baseLogFilePath;
        if (i > 0)
          path = Path.ChangeExtension(baseLogFilePath, string.Format(".{0}{1}", i + 1, ext));
        try
        {
          StreamWriter writer = CreateWriter(path);
          logFilePath = path;
          return writer;
        }
        catch (IOException)
        {
          continue;
        }
      }
      throw new Exception("Превышено ограничение на количество одновременно открытых лог файлов: " + fileCountLimit);
    }

    static StreamWriter CreateWriter(string path)
    {
      StreamWriter writer = new StreamWriter(path, true);
      writer.AutoFlush = true;
      return writer;
    }
    static void Dispose<T>(ref T disposable) where T : class, IDisposable
    {
      if (disposable != null)
      {
        T disposable2 = disposable;
        disposable = null;
        disposable2.Dispose();
      }
    }

    public void WriteLine(string text)
    {
      lock (writer_locker)
      {
        if (writer == null)
          writer = CreateWriter(logFilePath);

        try
        {
          writer.WriteLine(text);
        }
        catch (IOException)
        {
          Dispose(ref writer);
          writer = CreateWriter(logFilePath);
          writer.WriteLine(text);
        }

        if (writer.BaseStream.Position > maxLogSize)
        {
          SwitchFiles();
        }
      }
    }

    void SwitchFiles()
    {
      Dispose(ref writer);

      string oldLogFilePath = Path.ChangeExtension(logFilePath,
        ".archive" + Path.GetExtension(logFilePath));
      if (File.Exists(oldLogFilePath))
        File.Replace(logFilePath, oldLogFilePath, null, false);
      else
        File.Move(logFilePath, oldLogFilePath);

      writer = CreateWriter(logFilePath);
    }

    public void Dispose()
    {
      lock (writer_locker)
      {
        Dispose(ref writer);
      }
    }

    #region IFileLog Members

    int IFileLog.MaxLogLength
    {
      get { return (int)maxLogSize; }
      set { maxLogSize = value; }
    }

    TimeSpan IFileLog.ScanPeriod
    {
      get { return TimeSpan.Zero; }
      set { }
    }

    void IFileLog.AppendAllLogs()
    {
    }

    string IFileLog.MainLogFileName
    {
      get { return logFilePath; }
    }

    #endregion
  }
}
