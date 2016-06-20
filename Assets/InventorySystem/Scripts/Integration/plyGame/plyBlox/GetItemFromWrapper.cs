#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Items", "Get item from wrapper", BlockType.Variable, Description = "Get the item from a wrapper.")]
    public class GetItemFromWrapper : SystemObject_Value
    {
        [plyBlockField("Wrapper", ShowName = true, ShowValue = true, DefaultObject = typeof(InventoryUIItemWrapper), EmptyValueName = "-error-", Description = "The wrapper to get the item from.")]
        public InventoryUIItemWrapper wrapper;

        public override void Created()
        {

        }

        public override BlockReturn Run(BlockReturn param)
        {
            value = wrapper.item;

            return BlockReturn.OK;
        }
    }
}

#endif