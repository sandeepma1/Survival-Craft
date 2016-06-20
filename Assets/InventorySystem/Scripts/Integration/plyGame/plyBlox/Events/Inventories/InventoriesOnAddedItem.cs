﻿#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plyBloxKit;
using plyGame;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyEvent("Inventory Pro/Inventories", "Inventories OnAddedItem", Description = "Called when an item is added to this collection. <b>Note that it can only be used on the player.</b>" + "\n\n" +
        "<b>Temp variables:</b>\n\n" +
        "- item (InventoryItemBase)\n" +
        "- itemID (int)\n" +
        "- amount (int)\n" +
        "- isLoot (bool)")]
    public class InventoriesOnAddedItem : plyEvent
    {
        public override void Run()
        {
            base.Run();
        }

        public override System.Type HandlerType()
        {
            // here the Event is linked to the correct handler
            return typeof(InventoriesEventHandler);
        }
    }
}

#endif