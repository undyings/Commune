using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Commune.Basis;
using System.Drawing.Drawing2D;
using System;

namespace Commune.Forms
{
  public partial class ControlWithButton<TControl, TButton> : UserControl
    where TControl : Control
    where TButton : Control
  {
    public readonly TControl Control;
    public readonly TButton Button;
    public ControlWithButton(TControl control, TButton button)
    {
      InitializeComponent();

      this.Control = control;
      this.Button = button;
      Button.Dock = DockStyle.Right;
      this.Controls.Add(Button);
      Control.Dock = DockStyle.Fill;
      this.Controls.Add(Control);
      Control.BringToFront();

      Control.KeyDown += delegate (object sender, KeyEventArgs e)
      {
        OnKeyDown(e);
      };
      Button.KeyDown += delegate (object sender, KeyEventArgs e)
      {
        OnKeyDown(e);
      };
      Button.GotFocus += delegate (object sender, EventArgs e)
      {
        control.Focus();
      };
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      // 
      // ControlWithButton
      // 
      this.Name = "ControlWithButton";
      this.Size = new System.Drawing.Size(150, 30);
      this.ResumeLayout(false);

    }
  }
}
