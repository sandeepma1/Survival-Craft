using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem
{
    public class CollectionAvoidRebuildLock : IDisposable
    {
        public ItemCollectionBase[] collections { get; set; }

        public CollectionAvoidRebuildLock(params ItemCollectionBase[] collections)
        {
            this.collections = collections;
            foreach (var col in this.collections)
            {
                col.disableCounterRebuildBlocks++;
            }
        }

        public void Dispose()
        {
            foreach (var col in this.collections)
            {
                col.disableCounterRebuildBlocks--;
            }
        }
    }
}
