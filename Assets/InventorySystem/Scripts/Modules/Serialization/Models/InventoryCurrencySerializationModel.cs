using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem.Models
{
    public partial class InventoryCurrencySerializationModel
    {
        public uint currencyID;
        public float amount;

        public InventoryCurrencySerializationModel()
        {
            
        }

        public InventoryCurrencySerializationModel(uint currencyID, float amount)
        {
            this.currencyID = currencyID;
            this.amount = amount;
        }
    }
}
