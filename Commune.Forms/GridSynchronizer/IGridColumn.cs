using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Forms
{
  /// <summary>
  /// Интерфейс колонки.
  /// Стандартные реализации - SimpleColumn, SimpleImageColumn.
  /// </summary>
  public interface IGridColumn
  {
    /// <summary>
    /// Имя колонки
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Возвращает значение в колонке для данного ряда
    /// </summary>
    /// <param name="row">Ряд</param>
    /// <returns></returns>
    object GetValue(object row);

    /// <summary>
    /// Устанавливает значение в колонке для данного ряда
    /// </summary>
    /// <param name="row">Ряд</param>
    /// <param name="value">Значение</param>
    void SetValue(object row, object value);

    /// <summary>
    /// Возвращает дополнительную информацию о колонке
    /// </summary>
    /// <param name="ext">Идентификатор информации.
    /// На данный момент поддерживаются "DisplayName", "ColumnType", "Alignment", "IsReadOnly", "InitialWidth"</param>
    /// <returns></returns>
    object GetExtended(string ext);
  }

  [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
  public class ColumnExtensionAttribute : Attribute
  {
    public ColumnExtensionAttribute()
    {
    }
    public ColumnExtensionAttribute(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    public string Name;

    public object Value;
  }

  public interface ISupportDefaultExtensionColumn
  {
    void WithExtension(ColumnExtensionAttribute extension);
  }
}
