using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commune.Basis;
using System.Windows.Forms;
using System.Drawing;

namespace Commune.Forms
{
  /// <summary>
  /// Класс простой колонки, задающей делегат для получения (и если нужно установки) значения
  /// для данного ряда типа T. Подразумевает, что в GetValue и SetValue передается row типа T.
  /// Можно также задать имя колонки, DisplayName, начальную ширину и выравнивание.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SimpleColumn<T> : IGridColumn, ISupportDefaultExtensionColumn
  {
    /// <summary>
    /// Конструктор простой колонки.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="getter">Делегат, возвращающий значение колонки для данного ряда</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleColumn(string name, Getter<object, T> getter, params ColumnExtensionAttribute[] extensions)
    {
      this.Name = name;
      this.Getter = getter;

      AddExtensions(extensions);
      //this.extensions.AddRange(extensions);
    }

    void AddExtensions(params ColumnExtensionAttribute[] extensions)
    {
      foreach (ColumnExtensionAttribute extension in extensions)
      {
        if (extensionsByName.ContainsKey(extension.Name))
          Logger.AddMessage("В колонке '{0}' расширение '{1}' задается повторно", Name, extension.Name);
        extensionsByName[extension.Name] = extension;
      }
    }

    /// <summary>
    /// Конструктор простой колонки.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="getter">Делегат, возвращающий значение колонки для данного ряда</param>
    /// <param name="setter">Делегат, устанавливающий значение колонки для данного ряда</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleColumn(string name, Getter<object, T> getter, Executter<T, object> setter,
      params ColumnExtensionAttribute[] extensions)
    {
      this.Name = name;
      this.Getter = getter;
      this.Setter = setter;

      AddExtensions(extensions);
      //this.extensions.AddRange(extensions);
    }

    /// <summary>
    /// Констркутор простой колонки
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="displayName">Имя колонки, которое будет отображаться в DataGridView</param>
    /// <param name="getter">Делегат, возвращающий значение колонки для данного ряда</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleColumn(string name, string displayName, Getter<object, T> getter, params ColumnExtensionAttribute[] extensions)
      : this(name, getter, extensions)
    {
      this.DisplayName = displayName;
    }

    /// <summary>
    /// Конструктор простой колонки.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="getter">Делегат, возвращающий значение колонки для данного ряда</param>
    /// <param name="initialWidth">Начальная ширина колонки (-1 означает умолчательная для DataGridView)</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleColumn(string name, Getter<object, T> getter, int initialWidth, params ColumnExtensionAttribute[] extensions)
      : this(name, getter, extensions)
    {
      this.InitialWidth = initialWidth;
    }

    /// <summary>
    /// Конструктор простой колонки.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="displayName">DisplayName колонки</param>
    /// <param name="getter">Делегат, возвращающий значение колонки для данного ряда</param>
    /// <param name="initialWidth">Начальная ширина колонки (-1 означает умолчательная для DataGridView)</param>
    /// <param name="alignment">Выравнивание колонки (по умолчанию NotSet)</param>
    /// <param name="setter">Делегат, устанавливающий значение колонки для данного ряда</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleColumn(string name, string displayName, Getter<object, T> getter,
      int initialWidth, DataGridViewContentAlignment alignment, Executter<T, object> setter,
      params ColumnExtensionAttribute[] extensions)
    {
      this.Name = name;
      this.DisplayName = displayName;
      this.Getter = getter;
      this.Setter = setter;
      this.Alignment = alignment;
      this.InitialWidth = initialWidth;

      AddExtensions(extensions);
      //this.extensions.AddRange(extensions);
    }

    /// <summary>
    /// DisplayName колонки
    /// </summary>
    public string DisplayName;
    /// <summary>
    /// Делегат, возвращающий значение колонки для данного ряда
    /// </summary>
    public Getter<object, T> Getter;
    /// <summary>
    /// Делегат, устанавливающий значение колонки для данного ряда
    /// </summary>
    public Executter<T, object> Setter;
    /// <summary>
    /// Выравнивание колонки
    /// </summary>
    public DataGridViewContentAlignment Alignment
    {
      get
      {
        ColumnExtensionAttribute extensionAttr;
        if (extensionsByName.TryGetValue("Alignment", out extensionAttr))
          return (DataGridViewContentAlignment)extensionAttr.Value;
        return DataGridViewContentAlignment.NotSet;
      }
      set
      {
        ColumnExtensionAttribute extensionAttr;
        if (extensionsByName.TryGetValue("Alignment", out extensionAttr))
          extensionAttr.Value = value;
        else
          AddExtensions(new ColumnExtensionAttribute("Alignment", value));
      }
    }
    /// <summary>
    /// Начальная ширина колонки
    /// </summary>
    public int InitialWidth = -1;
    /// <summary>
    /// Расширения для колонки
    /// </summary>
    public ColumnExtensionAttribute[] Extensions
    {
      get
      {
        return _.ToArray<ColumnExtensionAttribute>(extensionsByName.Values);
        //return extensions.ToArray();
      }
    }

    //readonly List<ColumnExtensionAttribute> extensions = new List<ColumnExtensionAttribute>();
    readonly Dictionary<string, ColumnExtensionAttribute> extensionsByName = new Dictionary<string, ColumnExtensionAttribute>();

    public SimpleColumn<T> WithExtension(string extensionName, object value)
    {
      AddExtensions(new ColumnExtensionAttribute(extensionName, value));
      //extensions.Add(new ColumnExtensionAttribute(extensionName, value));
      return this;
    }
    public SimpleColumn<T> WithExtension(params ColumnExtensionAttribute[] extensions)
    {
      AddExtensions(extensions);
      return this;
    }

    void ISupportDefaultExtensionColumn.WithExtension(ColumnExtensionAttribute extension)
    {
      AddExtensions(extension);
      //extensions.Add(extension);
    }

    public SimpleColumn<T> WithCharWidth2(int charWidth)
    {
      return WithWidthes(charWidth, null);
    }

    public SimpleColumn<T> WithPatternWidth(string pattern)
    {
      AddExtensions(new ColumnExtensionAttribute("MinWidth", string.Format("pat:{0}", pattern ?? "")));
      //extensions.Add(new ColumnExtensionAttribute("MinWidth", string.Format("pat:{0}", pattern ?? "")));
      return this;
    }

    [Obsolete("Устаревшая, т.к. ширина символа всегда 7. Используйте WithCharWidth2")]
    public SimpleColumn<T> WithCharWidth(double charWidth)
    {
      this.InitialWidth = (int)(charWidth * 7);
      return this;
    }

    [Obsolete("Устаревшая, т.к. шрифт всегда Microsoft Sans Serif размер 8. Используйте WithPatternWidth")]
    public SimpleColumn<T> WithTemplateWidth(string template)
    {
      this.InitialWidth =
        (TextRenderer.MeasureText(template, new Font("Microsoft Sans Serif", 8)).Width * 12) / 10;
      return this;
    }

    public SimpleColumn<T> WithAlignment(DataGridViewContentAlignment alignment)
    {
      this.Alignment = alignment;
      return this;
    }

    public SimpleColumn<T> WithFormat(string format)
    {
      AddExtensions(new ColumnExtensionAttribute("Format", format));
      //extensions.Add(new ColumnExtensionAttribute("Format", format));
      return this;
    }

    public SimpleColumn<T> WithBorder(int borderWidth)
    {
      AddExtensions(new ColumnExtensionAttribute("Border", borderWidth));
      //extensions.Add(new ColumnExtensionAttribute("Border", borderWidth));
      return this;
    }

    public SimpleColumn<T> WithWidthes(int? minCharWidth, int? maxCharWidth)
    {
      if (minCharWidth != null)
      {
        AddExtensions(new ColumnExtensionAttribute("MinWidth", string.Format("char:{0}", minCharWidth.Value)));
        //extensions.Add(new ColumnExtensionAttribute("MinWidth", string.Format("char:{0}", minCharWidth.Value)));
      }
      if (maxCharWidth != null)
      {
        AddExtensions(new ColumnExtensionAttribute("MaxWidth", string.Format("char:{0}", maxCharWidth.Value)));
        //extensions.Add(new ColumnExtensionAttribute("MaxWidth", string.Format("char:{0}", maxCharWidth.Value)));
      }
      return this;
    }


    #region IGridColumn Members

    string name;
    /// <summary>
    /// Имя колонки
    /// </summary>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// Реализация метода IGridColumn.GetExtended
    /// </summary>
    /// <param name="ext"></param>
    /// <returns></returns>
    public virtual object GetExtended(string ext)
    {
      ColumnExtensionAttribute extensionAttr;
      if (extensionsByName.TryGetValue(ext, out extensionAttr))
        return extensionAttr.Value;

      //if (Extensions != null)
      //{
      //  foreach (ColumnExtensionAttribute extension in Extensions)
      //  {
      //    if (extension.Name == ext)
      //      return extension.Value;
      //  }
      //}

      switch (ext)
      {
        case "Alignment":
          return Alignment;
        case "IsReadOnly":
          return Setter == null;
        case "DisplayName":
          return DisplayName;
        case "InitialWidth":
          return InitialWidth;
      }
      return null;
    }

    /// <summary>
    /// Реализация метода IGridColumn.GetValue
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public virtual object GetValue(object row)
    {
      if (Getter != null && row is T)
        return Getter((T)row);
      return null;
    }

    /// <summary>
    /// Реализация метода IGridColumn.SetValue
    /// </summary>
    /// <param name="row"></param>
    /// <param name="value"></param>
    public virtual void SetValue(object row, object value)
    {
      if (Setter != null && row is T)
        Setter((T)row, value);
    }

    #endregion
  }

  public interface IGridColumnNumbered : IGridColumn
  {
    object GetValue(int rowIndex, object row);
  }

  /// <summary>
  /// Специальный класс для колонки с номером ряда. DataGridViewSynchronizer обрабатывает такие колонки специальным образом.
  /// </summary>
  public class SimpleNumberColumn : SimpleColumn<object>, IGridColumnNumbered
  {
    /// <summary>
    /// Конструктор колонки с номером. По умолчанию имя колонки - "№"
    /// </summary>
    public SimpleNumberColumn()
      : this("№")
    {
      this.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.InitialWidth = 35;
    }

    /// <summary>
    /// Конструктор колонки с номером.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleNumberColumn(string name, params ColumnExtensionAttribute[] extensions)
      : base(name, null, extensions)
    {
      this.Alignment = DataGridViewContentAlignment.MiddleRight;
      this.InitialWidth = 35;
    }

    /// <summary>
    /// Конструктор колонки с номером.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="initialWidth">Начальная ширина колонки (-1 означает умолчательная для DataGridView)</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleNumberColumn(string name, int initialWidth, params ColumnExtensionAttribute[] extensions)
      : base(name, null, initialWidth, extensions)
    {
      this.Alignment = DataGridViewContentAlignment.MiddleRight;
    }

    /// <summary>
    /// Конструктор колонки с номером.
    /// </summary>
    /// <param name="name">Имя колонки</param>
    /// <param name="displayName">DisplayName колонки</param>
    /// <param name="initialWidth">Начальная ширина колонки (-1 означает умолчательная для DataGridView)</param>
    /// <param name="alignment">Выравнивание колонки (по умолчанию NotSet)</param>
    /// <param name="extensions">Расширения для колонки</param>
    public SimpleNumberColumn(string name, string displayName, int initialWidth, DataGridViewContentAlignment alignment, params ColumnExtensionAttribute[] extensions)
      : base(name, displayName, null, initialWidth, alignment, null, extensions)
    {
    }

    public object GetValue(int rowIndex, object row)
    {
      return rowIndex + 1;
    }
  }
}
