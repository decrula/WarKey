using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public class WarKeyModel : IWarKeyModel
    {
        private string name;
        private bool displayEnemysHP;
        private bool displayAlliesHP;
        private IDictionary<int, int> keyMappers;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">方案名称</param>
        /// <param name="displayEnemysHP">显示敌方血条</param>
        /// <param name="displayAlliesHP">显示友方血条</param>
        /// <param name="keyMappers">键盘映射</param>
        public WarKeyModel(string name, bool displayEnemysHP, bool displayAlliesHP, IDictionary<int, int> keyMappers)
        {
            this.name = name;
            this.displayEnemysHP = displayEnemysHP;
            this.displayAlliesHP = displayAlliesHP;
            this.keyMappers = keyMappers;
        }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 显示敌方血条
        /// </summary>
        public bool DisplayEnemysHP
        {
            get { return this.displayEnemysHP; }
        }

        /// <summary>
        /// 显示友方血条
        /// </summary>
        public bool DisplayAlliesHP
        {
            get { return this.displayAlliesHP; }
        }

        /// <summary>
        /// 键盘映射
        /// </summary>
        public IDictionary<int, int> KeyMappers
        {
            get { return this.keyMappers; }
        }
    }
}