using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{

    public delegate void IInventoryCharacterStatChanged(IInventoryCharacterStat stat);
    public interface IInventoryCharacterStat
    {
        /// <summary>
        /// Name of this stat
        /// </summary>
        string statName { get; set; }

        string valueStringFormat { get; set; }

        /// <summary>
        /// The factor by which the value is multiplied.
        /// </summary>
        float currentFactor { get; }

        /// <summary>
        /// The factor by which the max value is multiplied.
        /// </summary>
        float currentFactorMax { get; }

        /// <summary>
        /// The current value of this stat. (baseValue + currentValueRaw) * currentFactor * currentFactorMaxValue
        /// </summary>
        float currentValue { get; }
        
        /// <summary>
        /// The current value without the factor and base value applied.
        /// </summary>
        float currentValueRaw { get; set; }
        float currentValueNormalized { get; }

        float maxValue { get; }


        /// <summary>
        /// The base / starting value of this property.
        /// </summary>
        float baseValue { get; }

        /// <summary>
        /// Should the item be shown in the UI?
        /// </summary>
        bool showInUI { get; }

        Sprite icon { get; }
        Color color { get; }

        event IInventoryCharacterStatChanged OnStatChanged;


        void Reset();


        /// <summary>
        /// The raw value is the value before any other transmutations ( base value and factors ).
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        void SetCurrentValueRaw(float value, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        void ChangeCurrentValueRaw(float value, bool fireEvents = true);

        /// <summary>
        ///  Factor allows you to set a multiplier for the actual value. Default is 1.0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        void SetFactor(float value, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        void ChangeFactor(float value, bool fireEvents = true);


        /// <summary>
        /// Factor max allows you to set a multiplier for the maximum value. Default is 1.0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        void SetFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        void ChangeFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// The max value raw is the value before any transmutations ( base value and factors )
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        void SetMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="andIncreaseCurrentValue"></param>
        /// <param name="fireEvents"></param>
        void ChangeMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// The base value is the "starting value". For example, start with 100 health.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        void SetBaseValue(float value, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fireEvents"></param>
        void ChangeBaseValue(float value, bool fireEvents = true);
    }
}
