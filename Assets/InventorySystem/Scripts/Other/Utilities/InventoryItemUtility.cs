using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public class InventoryItemUtility
    {
        public enum SetItemPropertiesAction
        {
            Use,
            UnUse
        }


        public static void SetItemProperties(InventoryPlayer player, InventoryItemPropertyLookup[] properties, SetItemPropertiesAction action, bool fireEvents = true)
        {
            // Use the item's properties.
            if (player != null)
            {
                foreach (var property in properties)
                {
                    SetItemProperty(player, property, action, fireEvents);
                }
            }
        }

        public static void SetItemProperty(InventoryPlayer player, InventoryItemPropertyLookup property, SetItemPropertiesAction action, bool fireEvents = true)
        {
            Assert.IsNotNull(player, "Null player object passed, make sure the InventoryPlayerManager.instance.currentPlayer is set!");
            if (player.characterCollection == null)
            {
                Debug.LogWarning("Character collection is not set on player. Can't set item properties (stats). Assign a Character collection to your player to fix this.", player.transform);
                return;
            }

            float multiplier = GetMultiplier(action);
            var prop = property.property;
            if (prop == null)
            {
                Debug.LogWarning("Property couldn't be found #" + property._propertyID);
                return;
            }

            var stat = player.characterCollection.stats.Get(prop.category, prop.name);
            if (stat != null)
            {
                switch (property.actionEffect)
                {
                    case InventoryItemPropertyLookup.ActionEffect.Add:

                        if (property.isFactor)
                        {
                            //if (property.increaseMax)
                            //    stat.ChangeFactorMax((property.floatValue - 1.0f) * multiplier, true);
                            //else
                            stat.ChangeFactorMax((property.floatValue - 1.0f) * multiplier, true, fireEvents);
                        }
                        else
                        {
                            //if(property.increaseMax)
                            //    stat.ChangeMaxValueRaw(property.floatValue * multiplier, true);
                            //else
                            stat.ChangeMaxValueRaw(property.floatValue * multiplier, true, fireEvents);
                        }

                        break;
                    case InventoryItemPropertyLookup.ActionEffect.Restore:

                        if (property.isFactor)
                            stat.ChangeCurrentValueRaw((stat.currentValue * (property.floatValue - 1.0f)) * multiplier, fireEvents);
                        else
                            stat.ChangeCurrentValueRaw(property.floatValue * multiplier, fireEvents);

                        break;

                    case InventoryItemPropertyLookup.ActionEffect.Decrease:

                        if (property.isFactor)
                            stat.ChangeCurrentValueRaw(-((stat.currentValue * (property.floatValue - 1.0f)) * multiplier), fireEvents);
                        else
                            stat.ChangeCurrentValueRaw(-(property.floatValue * multiplier), fireEvents);

                        break;
                    default:
                        Debug.LogWarning("Action effect" + property.actionEffect + " not found.");
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Stat based on property: " + prop.name + " not found on player.");
            }
        }

        private static float GetMultiplier(SetItemPropertiesAction action)
        {
            float multiplier = 1.0f;
            switch (action)
            {
                case SetItemPropertiesAction.Use:
                    multiplier = 1.0f;
                    break;
                case SetItemPropertiesAction.UnUse:
                    multiplier = -1f;
                    break;
                default:
                    Debug.LogWarning("Action " + action + " not found (Going with default use)");
                    break;
            }

            return multiplier;
        }

        public static InventoryItemAmountRow[] ItemsToRows(IList<InventoryItemBase> itemsToAdd)
        {
            var list = new List<InventoryItemAmountRow>(itemsToAdd.Count);
            for (int i = 0; i < itemsToAdd.Count; i++)
            {
                uint stackCount = itemsToAdd[i].currentStackSize;
                while (stackCount > 0)
                {
                    var row = new InventoryItemAmountRow(itemsToAdd[i], stackCount);
                    if (stackCount > itemsToAdd[i].maxStackSize)
                    {
                        stackCount -= itemsToAdd[i].maxStackSize;
                        row.SetAmount(itemsToAdd[i].maxStackSize);
                    }
                    else
                    {
                        stackCount = 0;
                    }

                    list.Add(row);
                }
            }

            return list.ToArray();
        }

        public static InventoryItemBase[] RowsToItems(InventoryItemAmountRow[] items, bool abideMaxStackSize)
        {
            var l = new List<InventoryItemBase>(items.Length);
            foreach (var row in items)
            {
                if (abideMaxStackSize)
                {
                    uint counter = row.amount;
                    var stackCount = Mathf.CeilToInt((float)row.amount / row.item.maxStackSize);

                    for (int i = 0; i < stackCount; i++)
                    {
                        var pickAmount = (uint)Mathf.Min(counter, row.item.maxStackSize);

                        var item = UnityEngine.Object.Instantiate<InventoryItemBase>(row.item);
                        item.currentStackSize = pickAmount;
                        l.Add(item);

                        counter -= pickAmount;
                    }
                }
                else
                {
                    var item = UnityEngine.Object.Instantiate<InventoryItemBase>(row.item);
                    item.currentStackSize = row.amount;
                    l.Add(item);
                }
            }

            return l.ToArray();
        }

        public static InventoryItemBase[] EnforceMaxStackSize(InventoryItemBase item)
        {
            if (item.currentStackSize <= item.maxStackSize)
            {
                return new [] { item };
            }

            uint counter = item.currentStackSize;
            var stackCount = Mathf.CeilToInt((float)item.currentStackSize / item.maxStackSize);
            var l = new InventoryItemBase[stackCount];

            for (int i = 0; i < stackCount; i++)
            {
                var pickAmount = (uint)Mathf.Min(counter, item.maxStackSize);

                var itemInst = UnityEngine.Object.Instantiate<InventoryItemBase>(item);
                itemInst.currentStackSize = pickAmount;
                l[i] = itemInst;

                counter -= pickAmount;
            }

            UnityEngine.Object.Destroy(item.gameObject); // Get rid of original
            return l.ToArray();
        }
    }
}
