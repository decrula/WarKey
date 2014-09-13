using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WarKey
{
    public class WarKeyController : IWarKeyControl, IKeyDownEventHandler
    {
        private IWarKeyModel model;
        private IWarKeyView view;
        private Keyboard keyboard;

        public WarKeyController(IWarKeyView view)
        {
            this.view = view;
            this.model = view.GetCurrent();

            keyboard = new Keyboard(this);
        }

        public void Save(string name, IWarKeyModel model)
        {
            this.model = model;

        }

        public void Load(string name)
        {
            //throw new NotImplementedException();
        }

        public bool Handle(KeyEventArgs e)
        {
            if (WarcraftWindow.IsForeground == false)
                return true;

            if (model.DisplayEnemysHP)
                WarcraftWindow.DisplayEnemysHP();
            if (model.DisplayAlliesHP)
                WarcraftWindow.DisplayAlliesHP();

            if (model.KeyMappers.ContainsKey(e.KeyValue))
            {
                WarcraftWindow.Send(model.KeyMappers[e.KeyValue]);

                // 当空格被映射时，且不处于聊天状态，屏蔽WAR的转至最近战争点。
                if (e.KeyValue == 32 && WarcraftWindow.IsChating == false)
                    return false;
            }

            return true;
        }
    }
}
