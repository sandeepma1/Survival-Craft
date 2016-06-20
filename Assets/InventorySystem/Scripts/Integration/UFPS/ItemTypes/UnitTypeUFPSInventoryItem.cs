#if UFPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [RequireComponent(typeof(ObjectTriggererItemUFPS))]
    public partial class UnitTypeUFPSInventoryItem : UFPSInventoryItemBase
    {
        public vp_UnitType unitType;
        public uint unitAmount = 1;
        public InventoryAudioClip pickupSound;

        public override uint currentStackSize
        {
            get { return unitAmount; }
            set { unitAmount = value; }
        }

        public override string name
        {
            get
            {
                if (useUFPSItemData && unitType != null)
                    return unitType.DisplayName;

                return base.name;
            }
            set { base.name = value; }
        }

        public override string description
        {
            get
            {
                if (useUFPSItemData && unitType != null)
                    return unitType.Description;
                
                return base.description;
            }
            set { base.description = value; }
        }
        
        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
#if UFPS_MULTIPLAYER

            if (vp_MPPickupManager.Instance != null)
            {
                //var dropObj = base.Drop(location, rotation);
                var dropPos = GetDropPosition(location, rotation);
                NotifyItemDropped(null);

                //gameObject.SetActive(false);
                vp_MPPickupManager.Instance.photonView.RPC("InventoryDroppedObject", PhotonTargets.AllBuffered, (int) ID,
                    objectTriggererItemUfps.ID, (int) unitAmount, dropPos, rotation);

                return null;
            }

            return base.Drop(location, rotation);
#else

            return base.Drop(location, rotation);

#endif
        }


        public override void EquippedItem(InventoryEquippableField equipSlot, uint amountEquipped)
        {
            base.EquippedItem(equipSlot, amountEquipped);

            AddAmmo(amountEquipped);
            ufpsEventHandler.Register(this); // Enable UFPS events
        }

        public override void UnEquippedItem(uint amountUnEquipped)
        {
            base.UnEquippedItem(amountUnEquipped);

            RemoveAmmo(currentStackSize);
            ufpsEventHandler.Unregister(this); // Disable UFPS events
        }


        //// UFPS EVENT
        protected virtual void OnStop_Reload()
        {
            Debug.Log("UFPS event after reload");
            UpdateAmmoAfterUFPSAction();
        }

        // UFPS Event
        protected virtual void OnStop_Attack()
        {
            Debug.Log("UFPS event after fired.");
            UpdateAmmoAfterUFPSAction();
        }

        protected virtual void AddAmmo(uint amount)
        {
            ufpsInventory.SetUnitCount(unitType, (int)amount);
//            UpdateAmmoAfterUFPSAction();
        }

        protected virtual void RemoveAmmo(uint amount)
        {
            int tempCurrentStackSize = (int)amount;
            int bankCount = ufpsInventory.GetUnitCount(unitType);
            if (bankCount > 0)
            {
                ufpsInventory.TryRemoveUnits(unitType, bankCount);
                tempCurrentStackSize -= bankCount;
            }

            foreach (var bankInstance in ufpsInventory.UnitBankInstances)
            {
                if (bankInstance.UnitType == unitType)
                {
                    if (bankInstance.Count >= tempCurrentStackSize)
                    {
                        bankInstance.TryRemoveUnits(tempCurrentStackSize);
                        tempCurrentStackSize = 0;
                    }
                    else if (bankInstance.Count < tempCurrentStackSize)
                    {
                        // Not enoguh for a full removal, but grab as much as possible
                        tempCurrentStackSize -= bankInstance.Count;
                        bankInstance.TryRemoveUnits(bankInstance.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Resyncs the Inventory Pro variables after an UFPS action.
        /// </summary>
        protected virtual void UpdateAmmoAfterUFPSAction()
        {
            int clipsAmmoCount = ufpsInventory.UnitBankInstances.Where(i => i.UnitType == unitType).Sum(i => i.Count);
            int inventoryAmmoCount = ufpsInventory.GetUnitCount(unitType);

//            Debug.Log("Updating clips count : " + clipsAmmoCount);
//            Debug.Log("Updating inventory count : " + inventoryAmmoCount);
            Assert.IsTrue(clipsAmmoCount + inventoryAmmoCount >= 0);

            currentStackSize = (uint)(clipsAmmoCount + inventoryAmmoCount);
            if (currentStackSize <= 0)
            {
                itemCollection[index].item = null;
            }

            itemCollection[index].Repaint();
        }

        public override bool PickupItem()
        {
//            currentStackSize = unitAmount;
            bool pickedup = base.PickupItem(); // Add to inventory instead.
            if (pickedup)
            {
                InventoryAudioManager.AudioPlayOneShot(pickupSound);
                return true;
            }

            return false;
        }
    }
}

#endif