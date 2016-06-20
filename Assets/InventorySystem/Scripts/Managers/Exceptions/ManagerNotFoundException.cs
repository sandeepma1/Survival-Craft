using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public class ManagerNotFoundException : Exception
    {

        public ManagerNotFoundException(string managerName)
            : base(managerName + " - Check the setup wizard to resolve the issue.")
        {
            
        }

    }
}
