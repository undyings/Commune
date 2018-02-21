using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.IO.Compression;

namespace Commune.Basis
{
  public class StateSaver : IDisposable
  {
    public static StateSaver Current =
      new StateSaver(ApplicationHlp.MapPath(@"State\current.xml"),
      TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(47), 150);

    public StateSaver(string path, TimeSpan interval)
      : this(path, interval, TimeSpan.Zero, 0)
    {
    }

    public StateSaver(string path, TimeSpan saveInterval, TimeSpan backupInterval, int maxStateFiles)
    {
      this.path = path;
      this.SaveInterval = saveInterval;
      this.BackupInterval = backupInterval;
      this.MaxStateFiles = maxStateFiles;

      timer.Tick += new EventHandler(timer_Tick);
    }

    string path;
    Timer timer = new Timer();

    public TimeSpan SaveInterval;
    public TimeSpan BackupInterval;
    public TimeSpan? RegularBackupInterval;
    public DateTime RegularBackupZeroTime;
    public int MaxStateFiles;
    bool isExistLastState = true;
    public bool CompressToZip;

    public void Start()
    {
      timer.Interval = (int)SaveInterval.TotalMilliseconds;
      timer.Start();
    }

    public void Stop()
    {
      timer.Stop();
    }

    void timer_Tick(object sender, EventArgs e)
    {
      try
      {
        Tick();
      }
      catch (Exception exc)
      {
        Logger.WriteException(exc);
      }
    }

    public void Tick()
    {
      try
      {
        DateTime now = DateTime.UtcNow;

        bool stateUpdated = false;
        if (lastSaveTime + SaveInterval < now)
        {
          SaveState();
          stateUpdated = true;
          lastSaveTime = now;

          if (BackupInterval >= SaveInterval &&
            lastBackupTime + BackupInterval < now)
          {
            BackupState();
            lastBackupTime = now;
          }
        }

        if (RegularBackupInterval != null)
        {
          DateTime currentRegularIntervalStartTime = RegularBackupZeroTime +
            new TimeSpan(MathHlp.AlignLow((now - RegularBackupZeroTime).Ticks,
              RegularBackupInterval.Value.Ticks)
            );
          if (lastRegularBackupTime < currentRegularIntervalStartTime &&
            lastBackupTime < currentRegularIntervalStartTime)
          {
            if (!stateUpdated)
            {
              SaveState();
            }
            BackupState();
            lastRegularBackupTime = now;
          }
        }
      }
      catch (Exception exc)
      {
        Logger.WriteException(exc);
      }
    }

    DateTime lastSaveTime = DateTime.MinValue;
    DateTime lastBackupTime = DateTime.MinValue;
    DateTime lastRegularBackupTime = DateTime.MinValue;

    public void SaveState()
    {
      if (isExistLastState && File.Exists(CurrentStateFilePath))
      {
        isExistLastState = false;
        FileInfo info = new FileInfo(CurrentStateFilePath);
        BackupState(GetBackupStateFilePath(info.LastWriteTimeUtc));
      }
      Save(CurrentStateFilePath, CompressToZip);
    }

    public void BackupState()
    {
      BackupState(GetBackupStateFilePath(DateTime.UtcNow));
      if (MaxStateFiles > 1)
        ApplicationHlp.RemoveOldFiles(StateDir, "*" + StateFileExtension, MaxStateFiles);
    }
    void BackupState(string backupFilePath)
    {
      File.Copy(CurrentStateFilePath, GetBackupStateFilePath(DateTime.UtcNow), true);
    }

    public string CurrentStateFilePath
    {
      get { return path; }
      set { path = value; }
    }

    public const string DefaultBackupFileMask = "{0:yyMMdd.HHmm}{1}";
    public string BackupFileMask = DefaultBackupFileMask;

    public string GetBackupStateFilePath(DateTime time)
    {
      return Path.Combine(StateDir,
        string.Format(BackupFileMask, time, StateFileExtension));
    }

    string StateFileExtension
    {
      get
      {
        string filename = Path.GetFileName(path);
        int dotIndex = filename.IndexOf('.');
        if (dotIndex >= 0)
          return filename.Substring(dotIndex);
        return "";
      }
    }

    string StateDir
    {
      get { return Path.GetDirectoryName(path); }
    }

    public void Save(string path)
    {
      Save(path, false);
    }

    public void Save(string path, bool compressToZip)
    {
      string dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      using (FileStream stream = new FileStream(path, FileMode.Create))
        Save(stream, compressToZip);
    }

    public void Save(Stream stream)
    {
      Save(stream, false);
    }

    public void Save(Stream stream, bool compressToZip)
    {
      if (compressToZip)
      {
        using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Compress))
        using (StreamWriter writer = new StreamWriter(zipStream))
          Save(writer);
      }
      else
      {
        using (StreamWriter writer = new StreamWriter(stream))
          Save(writer);
      }
    }

    public string RootName = "State";

    public void Save(TextWriter textWriter)
    {
      List<KeyValuePair<string, object>> items = new List<KeyValuePair<string, object>>();
      foreach (KeyValuePair<string, Getter<object>> pair in StateItemGetters)
      {
        try
        {
          items.Add(new KeyValuePair<string, object>(pair.Key, pair.Value()));
        }
        catch (Exception exc)
        {
          Logger.WriteException(exc);
        }
      }

      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.CheckCharacters = false;

      using (XmlWriter writer = XmlWriter.Create(textWriter, settings))
      {
        writer.WriteStartElement(RootName);

        foreach (KeyValuePair<string, object> pair in items)
        {
          if (pair.Value != null)
          {
            try
            {
              using (XmlWriter embedWriter = new XmlEmbeddingWriter(writer, pair.Key))
              {
                System.Xml.Serialization.XmlSerializerNamespaces ns =
                  new System.Xml.Serialization.XmlSerializerNamespaces();
                ns.Add("", "");
                System.Xml.Serialization.XmlSerializer serializer =
                  new System.Xml.Serialization.XmlSerializer(pair.Value.GetType());
                serializer.Serialize(embedWriter, pair.Value, ns);
              }
            }
            catch (Exception exc)
            {
              Logger.WriteException(exc);
            }
          }
        }
      }
    }

    List<KeyValuePair<string, Getter<object>>> StateItemGetters
    {
      get
      {
        lock (_stateItemGetters)
        {
          return new List<KeyValuePair<string, Getter<object>>>(_stateItemGetters);
        }
      }
    }
    List<KeyValuePair<string, Getter<object>>> _stateItemGetters = new List<KeyValuePair<string, Getter<object>>>();

    public object RegisterItem(string name, Getter<object> getter)
    {
      KeyValuePair<string, Getter<object>> itemGetter =
        new KeyValuePair<string, Getter<object>>(name, getter);
      lock (_stateItemGetters)
      {
        _stateItemGetters.Add(itemGetter);
      }
      return itemGetter;
    }

    public void UnregisterItem(object item)
    {
      lock (_stateItemGetters)
      {
        _stateItemGetters.Remove((KeyValuePair<string, Getter<object>>)item);
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      if (timer != null)
      {
        Stop();
        timer.Dispose();
        timer = null;
      }
    }

    #endregion

    ~StateSaver()
    {
      try { Dispose(); }
      catch { }
    }
  }
}
