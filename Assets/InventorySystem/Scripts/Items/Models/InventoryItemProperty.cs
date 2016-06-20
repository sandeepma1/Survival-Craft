using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryItemProperty
    {
        [HideInInspector]
        public int ID;

        public string name;
        public string category = "Default";
        

        public bool showInUI = true;
        [Tooltip("How the value is shown.\n{0} = Current amount\n{1} = Max amount")]
        public string valueStringFormat = "{0}";

        public bool useInStats = true;
        public Color color = Color.white;
        public Sprite icon;

        /// <summary>
        /// The base value is the start value of this property.
        /// </summary>
        [Tooltip("The base value is the start value of this property")]
        public float baseValue;


        public float maxValue = 100.0f;
        
       
        

        public string ToString(object currentValue)
        {
            try
            {
                return string.Format(valueStringFormat, currentValue, maxValue, name);
            }
            catch (Exception)
            { }

            return "(Formatting not valid)";
        }
    }
}