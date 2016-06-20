using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public interface IItemsSerializer
    {
        object SerializeCollection(ItemCollectionBase collection);
        object SerializeItems(IEnumerable<InventoryItemBase> items);

        ItemCollectionSerializationModel DeserializeCollection(object serializedData);
        ItemContainerSerializationModel DeserializeItems(object serializedData);
    }
}
