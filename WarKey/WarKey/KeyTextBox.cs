using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WarKey
{
    public class KeyTextBox : TextBox
    {
        public KeyTextBox()
        {
            this.TextAlign = HorizontalAlignment.Center;
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        public int KeyValue
        {
            get
            {
                return KeyboardDescription.GetKey(this.Text);
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                this.Text = "";
            }
            else
            {
                this.Text = KeyboardDescription.GetDescription(e.KeyValue);
                e.Handled = true;
            }

            base.OnKeyUp(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;

            base.OnKeyPress(e);
        }
    }
}
