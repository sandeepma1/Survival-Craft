using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public interface IInventoryItemContainer
    {
        string uniqueName { get; }

        InventoryItemBase[] items { get; set; }
    }
}
