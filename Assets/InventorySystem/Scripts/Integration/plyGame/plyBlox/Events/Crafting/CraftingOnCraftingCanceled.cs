﻿#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plyBloxKit;
using plyGame;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyEvent("Inventory Pro/Crafting", "Crafting OnCraftingCanceled", Description = "Called when the user cancels a crafting action." + "\n\n" +
        "<b>Temp variables:</b>\n\n" +
        "- itemID (int)\n" +
        "- category (InventoryCraftingCategory)\n" +
        "- categoryID (int)" +
        "- blueprint (InventoryCraftingBlueprint)\n" +
        "- blueprintID (int)" +
        "- progress (float) ( ranging from 0 - 1 )")]
    public class CraftingOnCraftingCanceled : plyEvent
    {
        public override void Run()
        {
            base.Run();
        }

        public override System.Type HandlerType()
        {
            // here the Event is linked to the correct handler
            return typeof(CraftingEventHandler);
        }
    }
}

#endif