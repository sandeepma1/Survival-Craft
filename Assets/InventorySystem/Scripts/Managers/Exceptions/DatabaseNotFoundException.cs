using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public class DatabaseNotFoundException : Exception
    {

        public DatabaseNotFoundException(string msg)
            : base(msg)
        {
            
        }

    }
}
