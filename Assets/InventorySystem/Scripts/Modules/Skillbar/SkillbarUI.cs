﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/skillbar/")]
    [AddComponentMenu("InventorySystem/Windows/Skillbar")]
    public partial class SkillbarUI : ItemCollectionBase
    {
        [System.Serializable]
        public class SkillbarSlot
        {
            public KeyCode[] keyCombination;
            public string name;
        }



        [Header("General")]
        public SkillbarSlot[] keys;

        /// <summary>
        /// When a consumable stack (or any other type) is depleted remove the stack and clear the slot.
        /// </summary>
        public bool clearSlotWhenStackDepleted = false;

        public override uint initialCollectionSize
        {
            get
            {
                return (uint)keys.Length;
            }
        }


        public override void Awake()
        {
            base.Awake();

            InitSlots();
        }

        protected virtual void InitSlots()
        {
            if (manuallyDefineCollection)
            {
                var wrappers = container.GetComponentsInChildren<InventoryUIItemWrapperKeyTrigger>();
                Assert.IsTrue(wrappers.Length == keys.Length, "More / less wrappers than there are keys defined in the SkillbarUI component!");
                for (int i = 0; i < wrappers.Length; i++)
                {
                    wrappers[i].keyCombination = keys[i].name;
                    wrappers[i].Repaint();
                    items[i] = wrappers[i];
                }
            }
            else
            {
                // Fill the container on startup, can add / remove later on
                for (int i = 0; i < initialCollectionSize; i++)
                {
                    ((InventoryUIItemWrapperKeyTrigger)items[i]).keyCombination = keys[i].name;
                    items[i].Repaint(); // First time
                }
            }
        }

        public override void Start()
        {
            base.Start();


            foreach (var i in InventoryManager.GetLootToCollections())
            {
                // Item added to inventory
                i.OnAddedItem += (items, amount, cameFromCollection) =>
                {
                    RepaintOfID(items.First().ID);
                };
                i.OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                {
                    RepaintOfID(itemID);
                };
                i.OnUsedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                {
                    RepaintOfID(itemID);
                };
            }
        }

        protected virtual void Update()
        {
            if (InventoryUIUtility.isFocusedOnInput)
                return;

            for (int i = 0; i < keys.Length; i++)
            {
                uint keysDown = 0;
                foreach (var k in keys[i].keyCombination)
                {
                    if(Input.GetKeyDown(k))
                        keysDown++;
                }

                if(keysDown == keys[i].keyCombination.Length && keys[i].keyCombination.Length > 0)
                {
                    // All keys down
                    items[i].TriggerUse();
                    items[i].Repaint();
                }
            }
        }

        public override bool SetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.SetItem(slot, item);
            if (set == false)
                return false;

            //item.On

            return true;
        }

        private void RepaintOfID(uint id)
        {
            foreach (var item in items)
            {
                if(item.item != null && item.item.ID == id)
                {
                    if (item.item.currentStackSize == 0 && clearSlotWhenStackDepleted)
                        item.item = null;

                    item.Repaint();
                }
            }
        }


        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            bool can = base.CanMergeSlots(slot1, collection2, slot2);
            if (can == false)
                return false;

            return useReferences == false;
        }
    }
}