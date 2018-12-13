//using System;
//using System.IO;
//using System.Threading;
//using System.Reflection;
////using System.Windows.Forms;

//namespace Commune.Diagnostics
//{
//  public interface IFileLog
//  {
//    int MaxLogLength { get;set;}
//    TimeSpan ScanPeriod { get;set;}
//    void AppendAllLogs();
//    string MainLogFileName { get;}
//  }
//  public class TraceFileLog : IDisposable, IFileLog
//  {
//    StreamWriter writer;
//    String LogFileName;
//    System.Threading.Timer timer;
//    int _MaxLogLength = 100 * 1000 * 1000;
//    TimeSpan _ScanPeriod = new TimeSpan(0, 10, 0);
//    string AppName
//    {
//      get
//      {
//        return Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath));
//      }
//    }
//    public string MainLogFileName
//    {
//      get { return AppName + ".log"; }
//    }
//    string ArchiveLogFileName
//    {
//      get { return AppName + ".archive.log"; }
//    }
//    public TraceFileLog()
//    {
//      try
//      {
//        LogFileName = GetNewLogName();
//        writer = CreateNewWriter();
//        //writer.WriteLine("{0}: Logging started at {1}", AppName, DateTime.Now);
//        //writer.WriteLine("<LogStarting appName='{0}' time='{1}'/>",
//        //System.Web.HttpUtility.HtmlEncode(AppName),
//        //DateTime.Now);
//        timer = new System.Threading.Timer
//            (new TimerCallback(delegate(object state) { AppendAllLogs(); }),
//             null,
//             TimeSpan.FromSeconds(2),
//             ScanPeriod);

//        Thread joinLogThread = new Thread(new ThreadStart(AppendAllLogs));
//        joinLogThread.Name = "Слияние логов";
//        joinLogThread.Priority = ThreadPriority.Lowest;
//        joinLogThread.Start();
//      }
//      catch
//      {
//        Console.WriteLine("Невозможно создать файл");
//      }

//      // AppendAllLogs();
//    }
//    StreamWriter CreateNewWriter()
//    {
//      StreamWriter sw = new StreamWriter(LogFileName, true);
//      sw.AutoFlush = true;
//      return sw;
//    }
//    public TraceFileLog(TimeSpan ScanPeriod, int MaxLogLength)
//      : this()
//    {
//      this.ScanPeriod = ScanPeriod;
//      this._MaxLogLength = MaxLogLength;
//    }
//    private string GetNewLogName()
//    {
//      for (int i = 0; ; i++)
//      {
//        string filename = String.Format("{0}.{1}.i.log", AppName, i);
//        //	Console.WriteLine(filename);
//        if (!File.Exists(filename))
//        {
//          return filename;
//        }
//      }
//    }

//    object synchro = new object();
//    public void WriteLine(string s)
//    {
//      lock (synchro)
//      {
//        writer.WriteLine(s);
//      }
//    }
//    public void Write(string s)
//    {
//      lock (synchro)
//      {
//        writer.Write(s);
//      }
//    }
//    public void AppendAllLogs()
//    {
//      try
//      {
//        string[] files =
//          Directory.GetFiles
//          (Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".*.i.log");
//        foreach (string logname in files)
//        {
//          if (String.Compare(logname, LogFileName) != 0)
//            AppendToLogFrom(logname);
//          else
//          {
//            lock (synchro)
//            {
//              writer.Close();
//              AppendToLogFrom(logname);
//              writer = CreateNewWriter();
//            }
//          }
//        }
//        if (!File.Exists(MainLogFileName))
//          File.Create(MainLogFileName);

//        long logFileLength = new FileInfo(MainLogFileName).Length;

//        if (logFileLength > _MaxLogLength * 2)
//          File.Delete(MainLogFileName);
//        else if (logFileLength > _MaxLogLength)
//          MoveToArchive();
//      }
//      catch
//      {
//      }
//    }
//    void MoveToArchive()
//    {
//      try
//      {
//        File.Delete(ArchiveLogFileName);
//        File.Move(MainLogFileName, ArchiveLogFileName);
//      }
//      catch (Exception)
//      {

//      }
//    }
//    void AppendToLogFrom(string filename)
//    {

//      if (new FileInfo(filename).Length > 0)
//        try
//        {
//          using (StreamWriter sWriter = new StreamWriter(MainLogFileName, true))
//          {
//            using (StreamReader sr = new StreamReader(filename))
//            {
//              sWriter.WriteLine("<!-- ============== Merging Logs ================ -->");
//              sWriter.WriteLine();
//              AppendFileToFilePartially(sWriter, sr);
//              //sWriter.WriteLine(sr.ReadToEnd());
//            }
//          }
//          File.Delete(filename);
//        }
//        catch (Exception e)
//        {
//          Console.WriteLine("Невозможно открыть файл");
//          Console.WriteLine(e.Message);
//        }
//      else
//        try
//        {
//          File.Delete(filename);
//        }
//        catch (Exception e)
//        {
//          Console.WriteLine("Невозможно открыть файл");
//          Console.WriteLine(e.Message);
//        }
//    }
//    void AppendFileToFilePartially(StreamWriter sw, StreamReader sr)
//    {
//      const int MaxSymbols = 100000;
//      char[] buf = new char[MaxSymbols];
//      int writes = MaxSymbols;
//      while (writes == MaxSymbols)
//      {
//        writes = sr.ReadBlock(buf, 0, MaxSymbols);
//        sw.Write(buf, 0, writes);
//      }
//    }
//    #region IDisposable Members

//    bool isDisposed = false;
//    public void Dispose()
//    {
//      if (isDisposed)
//        return;
//      if (writer != null)
//      {
//        writer.Close();
//        writer = null;
//      }
//      if (timer != null)
//      {
//        timer.Dispose();
//        timer = null;
//      }
//      AppendToLogFrom(LogFileName);
//      isDisposed = true;
//    }

//    #endregion
//    ~TraceFileLog()
//    {
//      try
//      {

//        Dispose();
//      }
//      catch
//      {
//      }
//    }
//    #region IFileLog Members

//    int IFileLog.MaxLogLength
//    {
//      get { return _MaxLogLength; }
//      set { _MaxLogLength = value; }
//    }

//    public TimeSpan ScanPeriod
//    {
//      get { return _ScanPeriod; }
//      set { _ScanPeriod = value; }
//    }

//    #endregion
//  }

//}
