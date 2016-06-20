using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryPlayerEquipTypeBinder
    {
        /// <summary>
        /// The equip field this is attached to
        /// </summary>
        public InventoryEquippableField associatedField;

        /// <summary>
        /// Use dynamic search for this field?
        /// </summary>
        public bool findDynamic = false;

        /// <summary>
        /// The path / gameObject name of the object to look for.
        /// </summary>
        public string equipTransformPath;

        /// <summary>
        /// The transform the item should be equipped to
        /// </summary>
        public Transform equipTransform;

        /// <summary>
        /// How should the item be equipped?
        /// </summary>
        public InventoryPlayerEquipHelper.EquipHandlerType equipHandlerType;
        
        /// <summary>
        /// The item that is currently in this binder
        /// </summary>
        public GameObject currentItem { get; set; }


        public InventoryPlayerEquipTypeBinder(InventoryEquippableField associatedField, Transform equipTransform, InventoryPlayerEquipHelper.EquipHandlerType equipHandlerType)
        {
            this.associatedField = associatedField;
            this.equipTransform = equipTransform;
            this.equipHandlerType = equipHandlerType;
        }
    }
}
