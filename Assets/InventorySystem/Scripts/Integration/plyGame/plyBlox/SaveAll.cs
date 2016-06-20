#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Actions", "Save all inventory pro data", BlockType.Action, Description = "Save all of the inventory pro data.", ShowName = "Save all inventory pro data.")]
    public class SaveAll : plyBlock
    {

        public override void Created()
        {

        }

        public override BlockReturn Run(BlockReturn param)
        {
            var allCollections = UnityEngine.Object.FindObjectsOfType<CollectionSaverLoaderBase>();
            foreach (var col in allCollections)
            {
                col.Save();
            }

            return BlockReturn.OK;
        }
    }
}

#endif