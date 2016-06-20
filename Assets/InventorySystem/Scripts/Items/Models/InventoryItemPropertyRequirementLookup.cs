using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryItemPropertyRequirementLookup
    {
        public enum FilterType
        {
            Equal,
            NotEqual,
            GreatherThan,
            LessThan
        }


        public int _propertyID;
        public InventoryItemProperty property
        {
            get
            {
                return ItemManager.database.properties.FirstOrDefault(o => o.ID == _propertyID);
            }
        }

        public float value;
        public FilterType filterType = FilterType.GreatherThan;



        public InventoryItemPropertyRequirementLookup()
        {
            
        }

        public InventoryItemPropertyRequirementLookup(InventoryItemPropertyRequirementLookup copyFrom)
        {
            this._propertyID = copyFrom._propertyID;
            this.value = copyFrom.value;
            this.filterType = copyFrom.filterType;
        }



        public bool CanUse(InventoryPlayer player)
        {
            Assert.IsNotNull(player, "Player object given is null");

            if (player.characterCollection == null)
            {
#if UNITY_EDITOR
                Debug.Log("Player character collection is null, can't verify filter stats - Default accepted");
#endif
                return true;
            }

            var stat = player.characterCollection.stats.Get(property.category, property.name);
            if (stat != null)
            {
                if (IsAbbidingFilter(stat.currentValue, value, filterType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAbbidingFilter(float statValue, float requiredValue, FilterType filterType)
        {

            switch (filterType)
            {
                case FilterType.GreatherThan:
                    return statValue > requiredValue;

                case FilterType.LessThan:
                    return statValue < requiredValue;

                case FilterType.Equal:
                    return requiredValue == statValue;

                case FilterType.NotEqual:
                    return requiredValue != statValue;
            }

            return false;
        }

        public override string ToString()
        {
            return property.ToString(value);
        }
    }
}
