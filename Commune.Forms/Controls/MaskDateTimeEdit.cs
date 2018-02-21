using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;
using System.ComponentModel;

namespace Commune.Forms
{
  public partial class MaskDateTimeEdit : ControlWithButton<MaskEdit, Control>
  {
    readonly string dateTimeFormat;
    readonly MonthCalendar monthCalendar;
    readonly bool isNullableDate;
    readonly Form form;
    DateTime? dateTime = null;
    public DateTime? Value
    {
      get { return dateTime; }
      set
      {
        dateTime = value;
        base.Control.Text = value != null ? value.Value.ToString(dateTimeFormat) : "";
      }
    }

    public MaskDateTimeEdit(string mask, string dateTimeFormat, Control button, bool isNullableDate,
      Getter<MonthCalendar> monthCalendarCreator) :
      base(new MaskEdit(mask, DateTime.Now.ToString(dateTimeFormat), MaskEdit.IsNumberMaskValue,
        MaskEdit.NumberKeyUpHandler, MaskEdit.NumberKeyDownHandler), button)
    {
      //InitializeComponent();

      this.dateTimeFormat = dateTimeFormat;
      this.isNullableDate = isNullableDate;

      this.monthCalendar = monthCalendarCreator();
      monthCalendar.Margin = new Padding(0);
      monthCalendar.MaxSelectionCount = 1;
      monthCalendar.Location = new Point(0, 0);

      this.form = new Form();
      form.StartPosition = FormStartPosition.Manual;
      form.AutoSize = true;
      form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

      form.FormBorderStyle = FormBorderStyle.None;
      Button resetDateButton = null;
      if (isNullableDate)
      {
        resetDateButton = new Button();
        resetDateButton.Text = "Сбросить дату";
        resetDateButton.Click += delegate
        {
          this.Value = null;
          form.Hide();
        };
        form.Controls.Add(resetDateButton);
      }
      form.Controls.Add(monthCalendar);
      form.ShowInTaskbar = false;
      monthCalendar.DateSelected += delegate
      {
        form.Hide();
      };
      monthCalendar.DateChanged += delegate
      {
        this.Value = monthCalendar.SelectionStart;
      };
      form.Deactivate += delegate
      {
        form.Hide();
        //form.Close();
      };

      button.Click += delegate
      {
        monthCalendar.SelectionStart = dateTime ?? DateTime.Now;
        monthCalendar.TodayDate = DateTime.Now;
        Point screenPoint = PointToScreen(new Point(Width, Height));
        //form.Location = new Point(screenPoint.X - form.Width, screenPoint.Y);
        form.Show();
        form.Location = new Point(screenPoint.X - monthCalendar.Width, screenPoint.Y);
        if (resetDateButton != null)
        {
          resetDateButton.Location = new Point(0, monthCalendar.Height);
          resetDateButton.Width = monthCalendar.Width;
        }
        //form.ShowDialog();
      };
    }

    protected override void Dispose(bool disposing)
    {
      form.Dispose();
      base.Dispose(disposing);
    }

    public bool ApplyEditing()
    {
      if (StringHlp.IsEmpty(Control.Text))
      {
        this.Value = null;
        return true;
      }
      else
      {
        DateTime result;
        if (DateTime.TryParse(Control.Text, out result))
        {
          this.Value = result;
          return true;
        }
      }
      return false;
    }

    public bool IsValidEditing()
    {
      if (StringHlp.IsEmpty(Control.Text))
        return true;
      DateTime result;
      return DateTime.TryParse(Control.Text, out result);
    }

    public void CancelEditing()
    {
      this.Value = dateTime;
    }
  }
}
