#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using plyCommon;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    public partial class plyGameConsumableInventoryItem : InventoryItemBase
    {
        public plyGameAttributeModifierModel[] plyAttributes = new plyGameAttributeModifierModel[0];
        public InventoryAudioClip audioClipWhenUsed;
        public float lastAddedUseValue { get; set; }


        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var info = base.GetInfo();

            var attributes = new InventoryItemInfoRow[plyAttributes.Length];
            for (int i = 0; i < plyAttributes.Length; i++)
            {
                var a = Player.Instance.actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == plyAttributes[i].id.Value);
                if(a != null)
                    attributes[i] = new InventoryItemInfoRow(a.def.screenName, plyAttributes[i].toAdd.ToString(), plyAttributes[i].color, plyAttributes[i].color);
            }

            info.AddAfter(info.First, attributes.ToArray());

            return info;
        }

        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;

            if (Player.Instance.actor.actorClass.currLevel < requiredLevel)
            {
                InventoryManager.langDatabase.itemCannotBeUsedLevelToLow.Show(name, description, requiredLevel);
                return -1;
            }
            
            SetItemProperties(InventoryPlayerManager.instance.currentPlayer, InventoryItemUtility.SetItemPropertiesAction.Use);

            InventoryAudioManager.AudioPlayOneShot(audioClipWhenUsed);

            currentStackSize--;
            NotifyItemUsed(1, true);
            return 1;
        }

        protected virtual void SetItemProperties(InventoryPlayer player, InventoryItemUtility.SetItemPropertiesAction action)
        {
            InventoryItemUtility.SetItemProperties(player, properties, action);
            SetPlyGameValues(player);
        }

        protected virtual void SetPlyGameValues(InventoryPlayer player)
        {
            foreach (var attr in plyAttributes)
            {
                var a = Player.Instance.actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == attr.id.Value);
                if (a != null)
                {
                    if (attr.addToBonus)
                    {
                        a.lastInfluence = gameObject;
                        lastAddedUseValue = attr.toAdd;
                        a.ChangeSimpleBonus(attr.toAdd);
                    }
                    else
                    {
                        a.lastInfluence = gameObject;
                        lastAddedUseValue = attr.toAdd;
                        a.SetConsumableValue(a.ConsumableValue + attr.toAdd, gameObject);
                    }
                }
            }
        }
    }
}

#endif