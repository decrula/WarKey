using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public interface IWarKeyView
    {
        IWarKeyModel GetCurrent();
        void Update(IWarKeyModel model);
    }
}
