using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public interface IWarKeyModelRepository
    {
        void Create(string name, IWarKeyModel model);
        void Update(string name, IWarKeyModel model);
        void Delete(string name);

        IWarKeyModel Read(string name);
    }
}
