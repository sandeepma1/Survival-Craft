using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public class SerializedObjectNotFoundException : Exception
    {

        public SerializedObjectNotFoundException(string message)
            : base(message)
        {
            
        }
    }
}
