using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Integration.SimpleJson;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public class EasySave2ItemsSerializer : IItemsSerializer
    {
        
        public object SerializeCollection(ItemCollectionBase collection)
        {
            // Easy save 2 handles all this for us..
            return new ItemCollectionSerializationModel(collection);
        }

        public object SerializeItems(IEnumerable<InventoryItemBase> items)
        {
            // Easy save 2 handles all this for us..
            return new ItemContainerSerializationModel(items);
        }

        public ItemCollectionSerializationModel DeserializeCollection(object serializedData)
        {
            // Easy save 2 handles all this for us..
            return (ItemCollectionSerializationModel) serializedData;
        }

        public ItemContainerSerializationModel DeserializeItems(object serializedData)
        {
            // Easy save 2 handles all this for us..
            return (ItemContainerSerializationModel) serializedData;
        }
    }
}
