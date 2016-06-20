using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryItemPropertyReference
    {
        [SerializeField]
        private int _propertyID;
        public int propertyID
        {
            get { return _propertyID; }
        }

        public InventoryItemProperty property
        {
            get { return ItemManager.database.properties.FirstOrDefault(o => o.ID == _propertyID); }
        }

    }
}