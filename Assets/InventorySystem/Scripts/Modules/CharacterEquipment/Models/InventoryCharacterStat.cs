using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    /// <summary>
    /// Holds the final stat as well as all the affectors changing the item's behaviour.
    /// </summary>
    public partial class InventoryCharacterStat : IInventoryCharacterStat
    {
        /// <summary>
        /// Name of this stat
        /// </summary>
        public string statName { get; set; }

        public string valueStringFormat { get; set; }

        /// <summary>
        /// The factor by which the value is multiplied.
        /// </summary>
        public float currentFactor { get; protected set; }

        /// <summary>
        /// The factor by which the max value is multiplied.
        /// </summary>
        public float currentFactorMax { get; protected set; }

        /// <summary>
        /// The current value of this stat. (baseValue + currentValueRaw) * currentFactor * currentFactorMaxValue
        /// </summary>
        public float currentValue
        {
            get
            {
                return Mathf.Clamp(currentValueNotClamped, float.MinValue, maxValue);
            }
        }

        protected float currentValueNotClamped
        {
            get
            {
                return (baseValue + currentValueRaw) * currentFactor;
            }
        }


        /// <summary>
        /// The current value without the factor and base value applied.
        /// </summary>
        public float currentValueRaw { get; set; }


        public float currentValueNormalized
        {
            get { return currentValue / maxValue; }
        }



        public float maxValue
        {
            get
            {
                return maxValueRaw * currentFactorMax;
            }
        }

        private float maxValueRaw;
        

        /// <summary>
        /// The base / starting value of this property.
        /// </summary>
        public float baseValue { get; protected set; }

        /// <summary>
        /// Should the item be shown in the UI?
        /// </summary>
        public bool showInUI { get; protected set; }

        public Sprite icon { get; set; }
        public Color color { get; set; }

        public event IInventoryCharacterStatChanged OnStatChanged;


        public InventoryCharacterStat(string statName, float baseValue, float maxValue, bool showInUI = true)
            : this(statName, "{0}", baseValue, maxValue, showInUI, Color.white, null)
        { }

        public InventoryCharacterStat(string statName, string valueStringFormat, float baseValue, float maxValue, bool showInUI, Color color, Sprite icon)
        {
            this.statName = statName;
            this.showInUI = showInUI;
            this.valueStringFormat = valueStringFormat;
            this.color = color;
            this.icon = icon;

            currentFactor = 1.0f;
            currentFactorMax = 1.0f;

            SetMaxValueRaw(maxValue, false);
            SetBaseValue(baseValue, false);
        }

        public InventoryCharacterStat(InventoryItemProperty property)
            : this(property.name, property.valueStringFormat, property.baseValue, property.maxValue, property.showInUI, property.color, property.icon)
        { }


        public virtual void Reset()
        {
            SetCurrentValueRaw(0, false);
        }

        protected void ClamClampCurrentValueRaw()
        {
            float over = currentValueNotClamped - maxValue;
            if (over > 0.0f)
            {
                currentValueRaw -= over; // "clamp" the currentValue raw, so that currentValueRaw + all other stats hit the maxValueRaw.
            }
        }

        /// <summary>
        /// The raw value is the value before any other transmutations ( base value and factors ).
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        public void SetCurrentValueRaw(float value, bool fireEvents = true)
        {
            currentValueRaw = value;
            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        public void ChangeCurrentValueRaw(float value, bool fireEvents = true)
        {
            SetCurrentValueRaw(currentValueRaw + value, fireEvents);
        }

        /// <summary>
        ///  Factor allows you to set a multiplier for the actual value. Default is 1.0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        public void SetFactor(float value, bool fireEvents = true)
        {
            currentFactor = value;
            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        public void ChangeFactor(float value, bool fireEvents = true)
        {
            SetFactor(currentFactor + value, fireEvents);
        }


        /// <summary>
        /// Factor max allows you to set a multiplier for the maximum value. Default is 1.0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        public void SetFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            float prevMax = maxValue;
            currentFactorMax = value;
            if (andIncreaseCurrentValue)
            {
                float increase = maxValue - prevMax;
                ChangeCurrentValueRaw(increase, false); // Updating below..
            }

            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        public void ChangeFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            SetFactorMax(currentFactorMax + value, andIncreaseCurrentValue, fireEvents);
        }

        /// <summary>
        /// The max value raw is the value before any transmutations ( base value and factors )
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        public void SetMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            float prevMax = maxValue;
            maxValueRaw = value;
            if (andIncreaseCurrentValue)
            {
                float increase = maxValue - prevMax;
                ChangeCurrentValueRaw(increase, false); // Updating below..
            }

            ClamClampCurrentValueRaw();


            if (fireEvents)
                NotifyCharacterCollection();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        public void ChangeMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            SetMaxValueRaw(maxValueRaw + value, andIncreaseCurrentValue, fireEvents);
        }

        /// <summary>
        /// The base value is the "starting value". For example, start with 100 health.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        public void SetBaseValue(float value, bool fireEvents = true)
        {
            baseValue = value;
            ClamClampCurrentValueRaw();

            if (fireEvents)
                NotifyCharacterCollection();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        public void ChangeBaseValue(float value, bool fireEvents = true)
        {
            SetBaseValue(baseValue + value, fireEvents);
        }
        

        private void NotifyCharacterCollection()
        {
            if (OnStatChanged != null)
                OnStatChanged(this);
        }

        public override string ToString()
        {
            return string.Format(valueStringFormat, System.Math.Round(currentValue, 2), maxValue, statName);
        }
    }
}