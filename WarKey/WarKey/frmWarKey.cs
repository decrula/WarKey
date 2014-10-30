using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WarKey
{
    public partial class frmWarKey : Form, IWarKeyView
    {
        private static readonly string[] NUMPAD = { "D7", "D8", "D4", "D5", "D1", "D2" };

        private IWarKeyControl controller;
        private readonly IList<KeyTextBox> numpadTextBoxs = new List<KeyTextBox>();
        /// <summary>
        /// TKey: 映射后的键值, TValue: 原始的键值
        /// </summary>
        private readonly IDictionary<int, int> keyMappers = new Dictionary<int, int>();

        public frmWarKey()
        {
            InitializeComponent();
            // 初始化约束验证
            InitializeConstraintValidator();

            InitializeWarKey();
        }

        private void InitializeWarKey()
        {
            controller = new WarKeyController(this);
            // 启动默认方案
            optSolution.SelectedIndex = optSolution.Items.IndexOf("默认方案");
        }

        /// <summary>
        /// 初始化约束验证
        /// </summary>
        private void InitializeConstraintValidator()
        {
            foreach (Control control in grpNumpad.Controls)
            {
                if (control is KeyTextBox)
                {
                    ((KeyTextBox)control).TextChanged += txtNumPad_TextChanged;
                    numpadTextBoxs.Add((KeyTextBox)control);
                }
            }
        }

        // 数字键盘映射验证
        private void txtNumPad_TextChanged(object sender, EventArgs e)
        {
            foreach (KeyTextBox tb in numpadTextBoxs)
            {
                if (tb == sender)
                    continue;

                if (tb.Text == ((KeyTextBox)sender).Text)
                    tb.Text = "";
            }
        }

        /// <summary>
        /// 添加数字键盘映射
        /// </summary>
        private void AddNumpadKeyMappers()
        {
            foreach (KeyTextBox tb in numpadTextBoxs)
            {
                if (string.IsNullOrEmpty(tb.Text))
                    continue;

                string keyDescription = tb.Name.Replace("txtNumPad", "D");
                int originalKey = KeyboardDescription.GetKey(keyDescription);
                int mappedKey = tb.KeyValue;

                if (keyMappers.ContainsKey(mappedKey))
                    continue;

                keyMappers.Add(mappedKey, originalKey);
            }
        }

        private void AddCommonKeyMappers()
        {
            for (int i = 0; i <= 5; i++)
            {
                KeyTextBox original = this.grpMainpad.Controls["txtOriginal" + i] as KeyTextBox;
                KeyTextBox mapped = this.grpMainpad.Controls["txtMapped" + i] as KeyTextBox;

                if (string.IsNullOrEmpty(original.Text) || string.IsNullOrEmpty(mapped.Text))
                    continue;

                int originalKey = original.KeyValue;
                int mappedKey = mapped.KeyValue;

                if (keyMappers.ContainsKey(mappedKey))
                    continue;

                keyMappers.Add(mappedKey, originalKey);
            }
        }

        public IWarKeyModel GetCurrent()
        {
            keyMappers.Clear();
            AddNumpadKeyMappers();
            AddCommonKeyMappers();
            return new WarKeyModel(optSolution.Text, cbDisplayEnemysHP.Checked, cbDisplayAlliesHP.Checked, this.keyMappers);
        }

        public void Update(IWarKeyModel model)
        {
            throw new NotImplementedException();
        }

        #region 后台最小化

        // 关闭窗口时隐藏
        private void frmWarKey_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void niNotify_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                cmMenu.Show();

            if (e.Button == MouseButtons.Left)
                this.Show();
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.controller.Save(optSolution.Text, this.GetCurrent());
            MessageBox.Show("success.");
        }

        private void optSolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            string solutionName = optSolution.Text;
            IWarKeyModel model = this.controller.Load(solutionName);
            this.UpdateUI(model);
        }

        private void UpdateUI(IWarKeyModel model)
        {
            this.cbDisplayEnemysHP.Checked = model.DisplayEnemysHP;
            this.cbDisplayAlliesHP.Checked = model.DisplayAlliesHP;

            foreach (KeyValuePair<int, int> pair in model.KeyMappers)
            {
                this.UpdateTextBox(pair);
            }
        }

        private void UpdateTextBox(KeyValuePair<int, int> pair)
        {
            string originalKeyDescription = KeyboardDescription.GetDescription(pair.Value);
            if (NUMPAD.Contains(originalKeyDescription))
            {
                string textboxID = originalKeyDescription.Replace("D", "txtNumPad");
                (this.grpNumpad.Controls[textboxID] as KeyTextBox).KeyValue = pair.Key;
            }
            else
            {
                KeyValuePair<KeyTextBox, KeyTextBox> textboxPair = GetAvailableKeyTextBoxPair();
                textboxPair.Key.Text = KeyboardDescription.GetDescription(pair.Key);
                textboxPair.Value.Text = originalKeyDescription;
            }
        }

        private KeyValuePair<KeyTextBox, KeyTextBox> GetAvailableKeyTextBoxPair()
        {
            KeyValuePair<KeyTextBox, KeyTextBox> textboxPair;

            for (int i = 0; i < 6; i++)
            {
                KeyTextBox txtOriginal = this.grpMainpad.Controls["txtOriginal" + i] as KeyTextBox;

                if (string.IsNullOrWhiteSpace(txtOriginal.Text) == false && txtOriginal.Text != "\0")
                    continue;

                KeyTextBox txtMapped = this.grpMainpad.Controls[txtOriginal.Name.Replace("txtOriginal", "txtMapped")] as KeyTextBox;
                textboxPair = new KeyValuePair<KeyTextBox, KeyTextBox>(txtMapped, txtOriginal);
                return textboxPair;
            }
            return new KeyValuePair<KeyTextBox, KeyTextBox>();
        }
    }
}