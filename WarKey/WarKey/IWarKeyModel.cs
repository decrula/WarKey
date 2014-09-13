using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public interface IWarKeyModel
    {
        string Name { get; }

        bool DisplayEnemysHP { get; }
        bool DisplayAlliesHP { get; }

        IDictionary<int, int> KeyMappers { get; }
    }
}
