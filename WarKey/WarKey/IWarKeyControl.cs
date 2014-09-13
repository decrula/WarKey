using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public interface IWarKeyControl
    {
        void Save(string name, IWarKeyModel model);
        void Load(string name);
    }
}
