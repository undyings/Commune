using System;
using System.Collections.Generic;
using System.Text;
using Commune.Basis;
using System.Globalization;

namespace Commune.Data
{
  public interface IFieldConverter
  {
    object ToGridValue(object databaseValue);
    object ToDatabaseValue(object gridValue);
  }

  public class FieldConverterHlp
  {
    public static string DatabaseValueAsString(object databaseValue)
    {
      if (databaseValue == null)
        return null;
      string valueAsStr = databaseValue as string;
      if (valueAsStr == null)
      {
        Logger.AddMessage("Значение базы данных '{0}' не является string", databaseValue);
        return null;
      }
      return valueAsStr;
    }
  }

  public class TimeSpanStringConverter : IFieldConverter
  {
    public readonly static TimeSpanStringConverter Default = new TimeSpanStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      long gridValue;
      if (long.TryParse(valueAsStr, out gridValue))
        return new TimeSpan(gridValue);
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в TimeSpan", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      TimeSpan? valueAsTimeSpan = gridValue as TimeSpan?;
      if (valueAsTimeSpan == null)
        return null;

      return valueAsTimeSpan.Value.Ticks.ToString();
    }
  }

  public class DateTimeLongConverter : IFieldConverter
  {
    public readonly static DateTimeLongConverter Default = new DateTimeLongConverter(true);
    public readonly static DateTimeLongConverter NotNullable = new DateTimeLongConverter(false);

    readonly bool isNullable;
    public DateTimeLongConverter(bool isNullable)
    {
      this.isNullable = isNullable;
    }

    public object ToGridValue(object databaseValue)
    {
      if (databaseValue == null)
      {
        if (isNullable)
          return null;
        return new DateTime(1, 1, 1);
      }
      //hack Т.к. была ошибка, из-за которой в базу вместо null записались нули
      if (isNullable && (long)databaseValue == 0)
        return null;
      return new DateTime((long)databaseValue);
    }

    public object ToDatabaseValue(object gridValue)
    {
      DateTime? time = gridValue as DateTime?;
      if (time == null)
      {
        if (isNullable)
          return null;
        return new DateTime(1, 1, 1).Ticks;
      }
      return time.Value.Ticks;
    }
  }

  public class TimeSpanSecondConverter : IFieldConverter
  {
    public readonly static TimeSpanSecondConverter Nullable = new TimeSpanSecondConverter(true);
    public readonly static TimeSpanSecondConverter NotNullable = new TimeSpanSecondConverter(false);

    readonly bool isNullable;
    public TimeSpanSecondConverter(bool isNullable)
    {
      this.isNullable = isNullable;
    }

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
      {
        if (isNullable)
          return null;
        return TimeSpan.Zero;
      }

      int seconds;
      if (int.TryParse(valueAsStr, out seconds))
        return TimeSpan.FromSeconds(seconds);
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' из секунд в TimeSpan", databaseValue);
      if (isNullable)
        return null;
      return TimeSpan.Zero;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
      {
        if (isNullable)
          return null;
        return ((int)TimeSpan.Zero.TotalSeconds).ToString();
      }
      TimeSpan? valueAsTimeSpan = gridValue as TimeSpan?;
      if (valueAsTimeSpan == null)
      {
        if (isNullable)
          return null;
        else
          valueAsTimeSpan = TimeSpan.Zero;
      }

      return ((int)valueAsTimeSpan.Value.TotalSeconds).ToString();
    }
  }

  public class DateTimeStringConverter : IFieldConverter
  {
    public readonly static DateTimeStringConverter Default = new DateTimeStringConverter(true);
    public readonly static DateTimeStringConverter NotNullable = new DateTimeStringConverter(false);

    readonly bool isNullable;
    public DateTimeStringConverter(bool isNullable)
    {
      this.isNullable = isNullable;
    }

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr != null)
      {
        long gridValue;
        if (long.TryParse(valueAsStr, out gridValue))
          return new DateTime(gridValue);
        Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в DateTime", databaseValue);
      }
      if (isNullable)
        return null;
      return new DateTime(1, 1, 1);
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return isNullable ? null : new DateTime(1, 1, 1).Ticks.ToString();

      DateTime? valueAsDateTime = gridValue as DateTime?;
      if (valueAsDateTime == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является DateTime", gridValue);
        return isNullable ? null : new DateTime(1, 1, 1).Ticks.ToString();
      }
      return valueAsDateTime.Value.Ticks.ToString();
    }
  }

  public class BoolStringConverter : IFieldConverter
  {
    public readonly static BoolStringConverter Default = new BoolStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      if (valueAsStr == "1")
        return true;
      if (valueAsStr == "0")
        return false;
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в bool", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      bool? valueAsBool = gridValue as bool?;
      if (valueAsBool == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является bool", gridValue);
        return null;
      }
      if (valueAsBool == true)
        return "1";
      return "0";
    }
  }

  public class ByteLongConverter : IFieldConverter
  {
    public readonly static ByteLongConverter Default = new ByteLongConverter();

    public object ToGridValue(object databaseValue)
    {
      return (byte)(long)databaseValue;
    }

    public object ToDatabaseValue(object gridValue)
    {
      return (long)(byte)gridValue;
    }
  }

  public class ByteSByteConverter : IFieldConverter
  {
    public readonly static ByteSByteConverter Default = new ByteSByteConverter();

    public object ToGridValue(object databaseValue)
    {
      return (byte)(sbyte)databaseValue;
    }

    public object ToDatabaseValue(object gridValue)
    {
      return (sbyte)(byte)gridValue;
    }
  }

  public class IntByteConverter : IFieldConverter
  {
    public readonly static IntByteConverter Default = new IntByteConverter();

    public object ToGridValue(object databaseValue)
    {
      return (int)(byte)databaseValue;
    }

    public object ToDatabaseValue(object gridValue)
    {
      return (byte)(int)gridValue;
    }
  }

  public class IntLongConverter : IFieldConverter
  {
    public readonly static IntLongConverter Default = new IntLongConverter();

    public object ToGridValue(object databaseValue)
    {
      return Convert.ToInt32(databaseValue);
      //return (int)(long)databaseValue;
    }

    public object ToDatabaseValue(object gridValue)
    {
      return gridValue;
      //return (long)(int)gridValue;
    }
  }

  public class IntNullableLongConverter : IFieldConverter
  {
    public readonly static IntNullableLongConverter Default = new IntNullableLongConverter();

    public object ToGridValue(object databaseValue)
    {
      if (databaseValue == null)
        return null;

      return (int)(long)databaseValue;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;

      return (long)(int)gridValue;
    }
  }

  public class BoolLongConverter : IFieldConverter
  {
    public readonly static BoolLongConverter Default = new BoolLongConverter();

    public object ToGridValue(object databaseValue)
    {
      return (long)databaseValue > 0;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if ((bool)gridValue)
        return (long)1;
      return (long)0;
    }
  }

  public class IntStringConverter : IFieldConverter
  {
    public readonly static IntStringConverter Default = new IntStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      int gridValue;
      if (int.TryParse(valueAsStr, out gridValue))
        return gridValue;
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в int", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      int? valueAsInt = gridValue as int?;
      if (valueAsInt == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является int", gridValue);
        return null;
      }
      return valueAsInt.Value.ToString();
    }
  }

  public class LongStringConverter : IFieldConverter
  {
    public readonly static LongStringConverter Default = new LongStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      long gridValue;
      if (long.TryParse(valueAsStr, out gridValue))
        return gridValue;

      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в long", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;

      long? valueAsLong = gridValue as long?;
      if (valueAsLong == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является long", gridValue);
        return null;
      }
      return valueAsLong.Value.ToString();
    }
  }

  public class UIntStringConverter : IFieldConverter
  {
    public readonly static UIntStringConverter Default = new UIntStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      uint gridValue;
      if (uint.TryParse(valueAsStr, out gridValue))
        return gridValue;
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в uint", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      uint? valueAsInt = gridValue as uint?;
      if (valueAsInt == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является uint", gridValue);
        return null;
      }
      return valueAsInt.Value.ToString();
    }
  }

  public class DecimalStringConverter : IFieldConverter
  {
    public readonly static DecimalStringConverter Default = new DecimalStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      decimal gridValue;
      if (decimal.TryParse(valueAsStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out gridValue))
        return gridValue;
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в decimal", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;

      decimal? valueAsDecimal = gridValue as decimal?;
      if (valueAsDecimal == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является decimal", gridValue);
        return null;
      }
      return valueAsDecimal.Value.ToString(CultureInfo.InvariantCulture);
    }
  }

  public class DoubleStringConverter : IFieldConverter
  {
    public readonly static DoubleStringConverter Default = new DoubleStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      double gridValue;
      if (double.TryParse(valueAsStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out gridValue))
        return gridValue;
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в double", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      double? valueAsDouble = gridValue as double?;
      if (valueAsDouble == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является double", gridValue);
        return null;
      }
      return valueAsDouble.Value.ToString(CultureInfo.InvariantCulture);
    }
  }

  public class GuidStringConverter : IFieldConverter
  {
    public readonly static GuidStringConverter Default = new GuidStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;
      return ConvertHlp.ToGuid(valueAsStr);
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      Guid? valueAsGuid = gridValue as Guid?;
      if (valueAsGuid == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является Guid", gridValue);
        return null;
      }
      return valueAsGuid.Value.ToString("N");
    }
  }

  public class FloatStringConverter : IFieldConverter
  {
    public readonly static FloatStringConverter Default = new FloatStringConverter();

    public object ToGridValue(object databaseValue)
    {
      string valueAsStr = FieldConverterHlp.DatabaseValueAsString(databaseValue);
      if (valueAsStr == null)
        return null;

      float gridValue;
      if (float.TryParse(valueAsStr, NumberStyles.Float, CultureInfo.InvariantCulture, out gridValue))
        return gridValue;
      Logger.AddMessage("Ошибка преобразования значения базы данных '{0}' в float", databaseValue);
      return null;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if (gridValue == null)
        return null;
      float? valueAsFloat = gridValue as float?;
      if (valueAsFloat == null)
      {
        Logger.AddMessage("Значение грида '{0}' не является float", gridValue);
        return null;
      }
      return valueAsFloat.Value.ToString(CultureInfo.InvariantCulture);
    }
  }

  public class BoolByteConverter : IFieldConverter
  {
    public readonly static BoolByteConverter Default = new BoolByteConverter();

    public object ToGridValue(object databaseValue)
    {
      return ((byte)databaseValue) != 0;
    }

    public object ToDatabaseValue(object gridValue)
    {
      if ((bool)gridValue)
        return 1;
      return 0;
    }
  }
}
