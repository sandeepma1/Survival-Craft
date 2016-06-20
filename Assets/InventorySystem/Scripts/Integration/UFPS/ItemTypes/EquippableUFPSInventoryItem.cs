#if UFPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [RequireComponent(typeof(ObjectTriggererItemUFPS))]
    public partial class EquippableUFPSInventoryItem : UFPSInventoryItemBase
    {
        public vp_ItemType itemType;


        public uint currentClipCount { get; set; }
        //public override uint currentStackSize
        //{
        //    get { return currentClipCount; }
        //    set { currentClipCount = value; }
        //}

        public override string name
        {
            get
            {
                if (useUFPSItemData && itemType != null)
                    return itemType.DisplayName;
                else
                    return base.name;
            }
            set { base.name = value; }
        }

        public override string description
        {
            get
            {
                if (useUFPSItemData && itemType != null)
                    return itemType.Description;
                else
                    return base.description;
            }
            set { base.description = value; }
        }

        protected override void Awake()
        {
            base.Awake();

//            currentClipCount = 0;
        }

        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
#if UFPS_MULTIPLAYER

            //var dropObj = base.Drop(location, rotation);
            var dropPos = GetDropPosition(location, rotation);
            NotifyItemDropped(null);

            //gameObject.SetActive(false);
            if(vp_MPPickupManager.Instance != null)
                vp_MPPickupManager.Instance.photonView.RPC("InventoryDroppedObject", PhotonTargets.AllBuffered, (int)ID, objectTriggererItemUfps.ID, (int)currentClipCount, dropPos, rotation);

            return null;

#else

            return base.Drop(location, rotation);

#endif
        }


        public override void EquippedItem(InventoryEquippableField equipSlot, uint amountEquipped)
        {
            base.EquippedItem(equipSlot, amountEquipped);
            
            objectTriggererItemUfps.TryGiveToPlayer(InventoryPlayerManager.instance.currentPlayer.GetComponentInChildren<Collider>(), (int)currentClipCount);
        }

        public override void UnEquippedItem(uint amountUnEquipped)
        {
            base.UnEquippedItem(amountUnEquipped);

            var item = ufpsInventory.GetItem(itemType) as vp_UnitBankInstance;
            if (item != null)
            {
                int unitCount = item.Count;
                if (unitCount > 0)
                {
                    // Remove from weapon clip
                    item.DoRemoveUnits(9999);
                }

                ufpsInventory.TryRemoveItem(itemType, 0);
                currentClipCount = 0;

                if (unitCount > 0)
                {
                    // Give to inventory
                    ufpsInventory.TryGiveUnits(item.UnitType, unitCount);
                }

                return;
            }

            ufpsInventory.TryRemoveItem(itemType, 0);
            currentClipCount = 0;
        }


        public override bool PickupItem()
        {
            bool pickedUp = base.PickupItem();
            if (pickedUp)
                transform.position = Vector3.zero; // Reset position to avoid the user from looting it twice when reloading (reloading temp. enables the item)

            return pickedUp;
        }
    }
}

#endif