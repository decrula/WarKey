using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WarKey
{
    public class WarKeyModelRepository : IWarKeyModelRepository
    {
        public void Create(string name, IWarKeyModel model)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, model);
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftware = hklm.OpenSubKey("Software", true);
            RegistryKey hkWarKey = hkSoftware.CreateSubKey("WarKey");
            hkWarKey.SetValue(name, stream.ToArray());

            hkWarKey.Close();
        }

        public void Update(string name, IWarKeyModel model)
        {
            Create(name, model);
        }

        public void Delete(string name)
        {
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftware = hklm.OpenSubKey("Software", true);
            RegistryKey hkWarKey = hkSoftware.CreateSubKey("WarKey");
            hkWarKey.DeleteValue(name, false);

            hkSoftware.Close();
        }

        public IWarKeyModel Read(string name)
        {
            IWarKeyModel model;
            byte[] bytesSaved;

            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey hkSoftware = hklm.OpenSubKey("Software", true);
            RegistryKey hkWarKey = hkSoftware.CreateSubKey("WarKey");
            if (hkWarKey.GetValue(name) == null)
                bytesSaved = (byte[])hkWarKey.GetValue("默认方案");
            else
                bytesSaved = (byte[])hkWarKey.GetValue(name);

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(bytesSaved);
            model = (IWarKeyModel)formatter.Deserialize(stream);

            return model;
        }
    }
}
