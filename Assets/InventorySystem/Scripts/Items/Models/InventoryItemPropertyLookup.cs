using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryItemPropertyLookup
    {
        public enum ActionEffect
        {
            /// <summary>
            /// Add to the bonus, increases the maximum
            /// </summary>
            Add,

            ///// <summary>
            ///// Add to the maximum value
            ///// </summary>
            //IncreaseMax,

            /// <summary>
            /// Restore the value (for example consumables, when eating an apple, restore the health).
            /// </summary>
            Restore,

            /// <summary>
            /// Decrease the value by a set amount, if the user doesn't have enough of the property the action will be canceled.
            /// </summary>
            Decrease
        }


        public int _propertyID;
        public InventoryItemProperty property
        {
            get
            {
                return ItemManager.database.properties.FirstOrDefault(o => o.ID == _propertyID);
            }
        }

        public string value;
        /// <summary>
        /// (1 = value * 1.0f, 0.1f = value * 0.1f so 10%).
        /// </summary>
        public bool isFactor = false;

        //public bool increaseMax = false; // Increase max or add to?
        public ActionEffect actionEffect = ActionEffect.Restore;


        public int intValue
        {
            get
            {
                int v = 0;
                Int32.TryParse(value, out v);

                return v;
            }
        }

        public float floatValue
        {
            get
            {
                float v = 0.0f;
                Single.TryParse(value, out v);

                return v;
            }
        }

        public bool isSingleValue
        {
            get
            {
                float v;
                return Single.TryParse(value, out v);
            }
        }

        public string stringValue
        {
            get
            {
                return value;
            }
        }

        public bool boolValue
        {
            get
            {
                return Boolean.Parse(value);
            }
        }


        public InventoryItemPropertyLookup()
        {
            
        }

        public InventoryItemPropertyLookup(InventoryItemPropertyLookup copyFrom)
        {
            this._propertyID = copyFrom._propertyID;
            this.value = copyFrom.value;
            this.isFactor = copyFrom.isFactor;
            this.actionEffect = copyFrom.actionEffect;
        }

        public bool CanDoDecrease(InventoryPlayer player)
        {
            if (player.characterCollection != null)
            {
                var prop = player.characterCollection.stats.Get(property.category, property.name);
                if (prop != null)
                {
                    if (prop.currentValue >= floatValue)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            return property.ToString(value);
        }
    }
}
