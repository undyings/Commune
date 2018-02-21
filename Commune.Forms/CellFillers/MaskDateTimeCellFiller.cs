using System;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public class MaskDateTimeCellFiller : IGridCellFiller
  {
    readonly string mask;
    readonly string dateTimeFormat;
    readonly bool isNullableDate;
    readonly Getter<Control> buttonCreator;
    readonly Getter<MonthCalendar> monthCalendarCreator;

    public MaskDateTimeCellFiller(string mask, string dateTimeFormat, bool isNullableDate,
      Getter<Control> buttonCreator, Getter<MonthCalendar> monthCalendarCreator)
    {
      this.mask = mask;
      this.dateTimeFormat = dateTimeFormat;
      this.isNullableDate = isNullableDate;
      this.buttonCreator = buttonCreator;
      this.monthCalendarCreator = monthCalendarCreator;
    }
    public MaskDateTimeCellFiller(string mask, string dateTimeFormat, bool isNullableDate) :
      this(mask, dateTimeFormat, isNullableDate, delegate
      {
        Button button = new Button();
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.FlatStyle = FlatStyle.Popup;
        button.Font = new Font("Arial", 8);
        button.Text = "▼";
        button.Width = 16;
        return button;
      }, delegate { return new MonthCalendar(); })
    {
    }

    public Control CreateEditControl(Rectangle cellRectagle, IGridColumn column, object row, object value)
    {
      Control button = buttonCreator();
      MaskDateTimeEdit dateTimeEdit = new MaskDateTimeEdit(mask, dateTimeFormat, button, isNullableDate,
        monthCalendarCreator);
      dateTimeEdit.Bounds = cellRectagle;
      dateTimeEdit.Value = value as DateTime?;
      return dateTimeEdit;
    }

    public void PaintCell(Graphics graphics, Rectangle cellRectangle, IGridColumn column, object row, object value)
    {
      DateTime? dateTime = value as DateTime?;
      SynchronizerHlp.PaintTextCell(graphics, cellRectangle, column, row,
        dateTime != null ? dateTime.Value.ToString(dateTimeFormat) : "");
    }

    public void PushedValue(Control control, IGridColumn column, object row)
    {
      MaskDateTimeEdit maskEdit = control as MaskDateTimeEdit;
      if (maskEdit == null)
      {
        Logger.AddMessage("MaskDateTimeCellFiller работает только с MaskDateTimeEdit");
        return;
      }

      if (maskEdit.ApplyEditing())
        column.SetValue(row, maskEdit.Value);
      else
      {
        //column.SetValue(row, DateTime.MinValue);
        if (!SynchronizerHlp.MakeParseError(column, row, maskEdit.Control.Text))
          Logger.AddMessage("MaskDateTimeCellFiller: Для колонки '{0}' не найден обработчик ошибок парсинга",
            column.Name);
      }
    }
  }
}
