using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace Commune.Basis
{
  public class ApplicationHlp
  {
    public static Process ProcessHideStart(string processPath)
    {
      return ProcessHideStart(processPath, "");
    }

    public static Process ProcessHideStart(string processPath, string args)
    {
      ProcessStartInfo info = new ProcessStartInfo(processPath);
      if (!StringHlp.IsEmpty(args))
        info.Arguments = args;
      info.CreateNoWindow = true;
      info.UseShellExecute = false;
      return Process.Start(info);
    }

    public static string MapPath(string filename)
    {
      return Path.Combine(System.Windows.Forms.Application.StartupPath, filename);
    }

    public static string CheckAndCreateFolderPath(string root, params string[] folders)
    {
      string path = root;
      foreach (string folder in folders)
      {
        path = Path.Combine(path, folder);
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
      }
      return path;
    }

    public static ComponentInfo[] GetComponentsInfo()
    {
      List<ComponentInfo> components = new List<ComponentInfo>();

      string[] versionFiles = Directory.GetFiles(MapPath("."), "*.version");
      foreach (string file in versionFiles)
      {
        Match m = Regex.Match(Path.GetFileName(file), @"^(?<progName>.*)\.version$");
        ComponentInfo info = new ComponentInfo();
        info.Name = m.Groups["progName"].Value;
        info.Version = File.ReadAllText(file);

        string historyFile = ApplicationHlp.MapPath(info.Name + ".history.html");
        if (File.Exists(historyFile))
        {
          info.History = File.ReadAllText(historyFile);
          try
          {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(info.History);
            info.History = doc.InnerText;
          }
          catch (Exception exc)
          {
            //Это не ошибка, просто не Xml - формат
            Logger.WriteException(exc);
          }
        }
        else
          info.History = string.Empty;

        components.Add(info);
      }
      return components.ToArray();
    }
    public static string[] GetVersions()
    {
      List<string> versions = new List<string>();

      string[] versionFiles = Directory.GetFiles(MapPath("."), "*.version");
      foreach (string file in versionFiles)
      {
        Match m = Regex.Match(Path.GetFileName(file), @"^(?<progName>.*)\.version$");
        string version = File.ReadAllText(file);
        versions.Add(version);
      }
      return versions.ToArray();
    }
    public struct ComponentInfo
    {
      public string Name;
      public string Version;
      public string History;

      public override string ToString()
      {
        return string.Format("Программа '{0}'\r\nВерсия '{1}'\r\nИстория изменений:\r\n{2}\r\n", Name, Version, History);
      }
    }

    public static void RemoveOldFiles(string dir, string searchPattern, int maxFiles)
    {
      string[] files = Directory.GetFiles(dir, searchPattern);
      if (files.Length > maxFiles)
      {
        Array.Sort(files);

        for (int i = 0; i < files.Length - maxFiles; i++)
        {
          try { File.Delete(files[i]); }
          catch (Exception exc) { Logger.WriteException(exc); }
        }
      }
    }
  }
}
