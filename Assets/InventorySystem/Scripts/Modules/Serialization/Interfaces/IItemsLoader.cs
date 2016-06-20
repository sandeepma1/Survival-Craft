using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public interface IItemsLoader
    {
        void LoadItems(Action<object> callback);
    }
}