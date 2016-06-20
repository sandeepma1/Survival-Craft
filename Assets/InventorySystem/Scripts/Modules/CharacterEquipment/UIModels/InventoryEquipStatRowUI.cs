using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Used to define a row of stats.
    /// </summary>
    public partial class InventoryEquipStatRowUI : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// Name of the stat
        /// </summary>
        public UnityEngine.UI.Text statName;

        /// <summary>
        /// Status result
        /// </summary>
        public UnityEngine.UI.Text stat;

        public Image icon;
        public bool hideStatNameIfIconIsPresent = false;

        public virtual void Repaint(string name, string stat, Color color, Sprite icon)
        {
            if (this.statName != null)
            {
                this.statName.gameObject.SetActive(true);
                this.statName.text = name;
                this.statName.color = color;
            }

            this.stat.text = stat;
            this.stat.color = color;

            if (this.icon != null)
            {
                this.icon.sprite = icon;
                this.icon.gameObject.SetActive(true);
                this.icon.color = color;
                if (icon == null)
                {
                    this.icon.gameObject.SetActive(false);
                }
                else if (hideStatNameIfIconIsPresent && statName != null)
                {
                    statName.gameObject.SetActive(false);
                }
            }
        }

        public void Reset()
        {
            Repaint(string.Empty, string.Empty, Color.white, null);
        }
    }
}