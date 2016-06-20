using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public partial interface IInventoryItemContainerGenerator
    {
        IInventoryItemContainer container { get; }

        IItemGenerator generator { get; }
    }
}