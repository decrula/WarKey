using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public class WarKeyModelRepository : IWarKeyModelRepository
    {
        public void Create(string name, IWarKeyModel model)
        {
            RegistryKey hklm = Registry.LocalMachine;
        }

        public void Update(string name, IWarKeyModel model)
        {
            throw new NotImplementedException();
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public IWarKeyModel Read(string name)
        {
            throw new NotImplementedException();
        }
    }
}
