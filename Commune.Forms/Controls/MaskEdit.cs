using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Commune.Basis;

namespace Commune.Forms
{
  public class MaskEdit : TextBox
  {
    public static bool IsNumberMaskValue(string mask, string value)
    {
      if (value == null)
        return true;
      if (value.Length != mask.Length)
        return false;
      int index = -1;
      foreach (char ch in value)
      {
        index++;
        if (mask[index] == '|')
          continue;
        if (!char.IsDigit(ch))
          return false;
      }
      return true;
    }

    public static char NumberKeyDownHandler(char ch)
    {
      if (!char.IsDigit(ch))
        return ch;
      int value = (int)char.GetNumericValue(ch);
      return ((value + 9) % 10).ToString()[0];
    }

    public static char NumberKeyUpHandler(char ch)
    {
      if (!char.IsDigit(ch))
        return ch;
      int value = (int)char.GetNumericValue(ch);
      return ((value + 1) % 10).ToString()[0];
    }

    string mask;
    public string Mask
    {
      get { return mask; }
    }
    string defaultValue;
    public string DefaultText
    {
      get { return defaultValue; }
    }
    Getter<bool, string, string> validator;
    public override string Text
    {
      get { return base.Text; }
      set
      {
        if (Validate(value))
        {
          base.Text = value;
          Select(0, 1);
        }
      }
    }

    Getter<char, char> keyUpHandler;
    Getter<char, char> keyDownHandler;

    public void Initialize(string mask, string defaultValue, Getter<bool, string, string> validator,
      Getter<char, char> keyUpHandler, Getter<char, char> keyDownHandler)
    {
      this.mask = mask;
      this.defaultValue = defaultValue;
      this.validator = validator;
      this.keyUpHandler = keyUpHandler;
      this.keyDownHandler = keyDownHandler;
      this.Text = defaultValue;
    }

    public void InitializeAsNumerical(int digitCount)
    {
      Initialize(new string('x', digitCount), new string('0', digitCount),
        MaskEdit.IsNumberMaskValue, MaskEdit.NumberKeyUpHandler, MaskEdit.NumberKeyDownHandler);
    }

    public MaskEdit() :
      this("xx", "00", delegate { return true; })
    {
    }

    public MaskEdit(string mask, string defaultValue, Getter<bool, string, string> validator)
      : this(mask, defaultValue, validator, null, null)
    {
    }

    public MaskEdit(string mask, string defaultValue, Getter<bool, string, string> validator,
      Getter<char, char> keyUpHandler, Getter<char, char> keyDownHandler)
      : base()
    {
      if (mask.Length != defaultValue.Length)
        throw new ArgumentException(string.Format(
          "MaskEdit: Длина маски '{0}' должна быть равна длине дефолтного значения '{1}'",
          mask, defaultValue));

      Initialize(mask, defaultValue, validator, keyUpHandler, keyDownHandler);

      this.ContextMenu = new ContextMenu();
    }

    bool Validate(string newValue)
    {
      if (StringHlp.IsEmpty(newValue))
        return true;

      if (newValue.Length != Mask.Length)
        return false;

      return validator(Mask, newValue);
    }

    int CaretPosition
    {
      get
      {
        return Math.Max(0, SelectionStart + SelectionLength - 1);
      }
    }

    void NextPosition()
    {
      NextPosition(true);
    }

    void NextPosition(bool isRound)
    {
      if (!StringHlp.IsEmpty(Text))
      {
        for (int i = CaretPosition + 1; i < Text.Length; ++i)
        {
          if (Mask[i] != '|')
          {
            Select(i, 1);
            return;
          }
        }
        if (isRound)
        {
          for (int i = 0; i < Mask.Length; ++i)
          {
            if (Mask[i] != '|')
            {
              Select(i, i + 1);
              break;
            }
          }
        }
        return;
      }
      Select(0, 0);
    }

    void PrevPosition()
    {
      if (!StringHlp.IsEmpty(Text))
      {
        for (int i = CaretPosition - 1; i >= 0; --i)
        {
          if (Mask[i] != '|')
          {
            Select(i, 1);
            return;
          }
        }
        for (int i = Mask.Length - 1; i >= 0; --i)
        {
          if (Mask[i] != '|')
          {
            Select(i, 1);
            return;
          }
        }
        return;
      }
      Select(0, 0);
    }

    bool MouseSelect(MouseEventArgs e, int oldCaretPosition)
    {
      int index = GetCharIndexFromPosition(e.Location);
      if (index != -1)
      {
        Point position = GetPositionFromCharIndex(index);
        if (position.X > e.Location.X)
          index = Math.Max(0, index - 1);
        if (Mask[index] != '|')
        {
          Select(index, 1);
          return true;
        }
      }
      Select(oldCaretPosition, 1);
      if (Mask[oldCaretPosition] == '|')
        NextPosition();
      return false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;

      int caretPosition = CaretPosition;
      base.OnMouseMove(e);
      MouseSelect(e, caretPosition);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      int caretPosition = CaretPosition;
      base.OnMouseDown(e);
      MouseSelect(e, caretPosition);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      int caretPosition = CaretPosition;
      base.OnMouseUp(e);
      MouseSelect(e, caretPosition);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      int caretPosition = CaretPosition;
      base.OnMouseClick(e);
      MouseSelect(e, caretPosition);
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
      int caretPosition = CaretPosition;
      base.OnMouseDoubleClick(e);
      MouseSelect(e, caretPosition);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (StringHlp.IsEmpty(Text) && e.KeyCode != Keys.Enter)
      {
        base.Text = DefaultText;
        Select(0, 1);
      }

      switch (e.KeyCode)
      {
        case Keys.Left:
          PrevPosition();
          e.Handled = true;
          return;
        case Keys.Back:
          {
            //Select(Math.Max(0, CaretPosition - 1), 1);
            PrevPosition();
            int caretPosition = CaretPosition;
            if (Mask[caretPosition] != '|')
            {
              string newValue = ReplaceChar(Text, caretPosition, DefaultText[caretPosition]);
              ValidateAndSet(newValue, caretPosition);
            }
            e.Handled = true;
            return;
          }
        case Keys.Right:
        case Keys.Space:
          NextPosition();
          e.Handled = true;
          return;
        case Keys.Delete:
          {
            int caretPosition = CaretPosition;
            if (Mask[caretPosition] != '|')
            {
              string newValue = ReplaceChar(Text, caretPosition, DefaultText[caretPosition]);
              ValidateAndSet(newValue, caretPosition);
            }
            e.Handled = true;
            return;
          }
        case Keys.Down:
        case Keys.Up:
          {
            Getter<char, char> handler = e.KeyCode == Keys.Down ? keyDownHandler : keyUpHandler;

            int caretPosition = CaretPosition;
            if (handler != null && Mask[caretPosition] != '|')
            {
              string newValue = ReplaceChar(Text, caretPosition, handler(Text[caretPosition]));
              ValidateAndSet(newValue, caretPosition);
            }
            e.Handled = true;
            return;
          }
        case Keys.Home:
        case Keys.End:
          e.Handled = true;
          return;
      }
      base.OnKeyDown(e);
    }

    static string ReplaceChar(string text, int caretPosition, char newChar)
    {
      char[] chars = text.ToCharArray();
      chars[caretPosition] = newChar;
      return new string(chars);
    }

    bool ValidateAndSet(string newValue, int caretPosition)
    {
      if (Validate(newValue))
      {
        base.Text = newValue;
        Select(caretPosition, 1);
        return true;
      }
      return false;
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)13 || e.KeyChar == (char)8)
      {
        e.Handled = true;
        return;
      }
      if (StringHlp.IsEmpty(Text))
      {
        base.Text = DefaultText;
        Select(0, 1);
      }
      int caretPosition = CaretPosition;
      if (Mask[caretPosition] != '|')
      {
        string newValue = ReplaceChar(Text, caretPosition, e.KeyChar);
        if (ValidateAndSet(newValue, caretPosition))
          NextPosition(false);
      }
      e.Handled = true;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      e.Handled = true;
    }
  }
}
