using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem.UI
{
    [HelpURL("http://devdog.nl/documentation/uishowstat/")]
    public class UIShowStat : MonoBehaviour
    {
        [Header("Stat")]
        public InventoryItemPropertyReference property;

        [Header("Player")]
        public bool useCurrentPlayer = true;
        public InventoryPlayer player;

        [Header("Interpolation")]
        public bool useValueInterpolation = false;
        public AnimationCurve interpolationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float interpolationSpeed = 1f;

        [Header("Visuals")]
        public UIShowValueModel visualizer = new UIShowValueModel();


        /// <summary>
        /// The aim value used for interpolation.
        /// </summary>
        private float _aimStatValue;
        private float _deltaStatValue;

        public void Start()
        {
            if (useCurrentPlayer)
            {
                InventoryPlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            }

            // Force a repaint.
            OnPlayerChanged(null, InventoryPlayerManager.instance.currentPlayer);
        }

        private void OnPlayerChanged(InventoryPlayer oldPlayer, InventoryPlayer newPlayer)
        {
            // Remove the old
            if (oldPlayer != null && oldPlayer.characterCollection != null)
                oldPlayer.characterCollection.stats.OnStatChanged -= Repaint;

            player = newPlayer;

            // Add the new
            if (player != null && player.characterCollection != null)
            {
                player.characterCollection.stats.OnStatChanged += Repaint;
                Repaint(player.characterCollection.stats.Get(property.property.category, property.property.name));
            }
        }

        protected virtual void Repaint(IInventoryCharacterStat stat)
        {
            if (stat == null || stat != player.characterCollection.stats.Get(property.property.category, property.property.name))
                return;

            if (useValueInterpolation)
            {
                StartCoroutine(_RepaintInterpolated(stat));
            }
            else
            {
                visualizer.Repaint(stat.currentValue, stat.maxValue);
            }
        }

        private IEnumerator _RepaintInterpolated(IInventoryCharacterStat stat)
        {
            _aimStatValue = stat.currentValue;
            float timer = 0f;
            while (timer < 1f)
            {
                float val = Mathf.Lerp(_deltaStatValue, _aimStatValue, interpolationCurve.Evaluate(timer));
                visualizer.Repaint(val, stat.maxValue);

                timer += Time.deltaTime * interpolationSpeed;
                yield return null;
            }

            _deltaStatValue = _aimStatValue;
        }
    }
}
