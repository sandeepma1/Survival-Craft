using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper reference sum")]
    public partial class InventoryUIItemWrapperReferenceSum : InventoryUIItemWrapperKeyTrigger
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Repaint()
        {
            base.Repaint();

            if (item != null)
            {
                uint count = InventoryManager.GetItemCount(item.ID, false);
                amountText.text = count.ToString();
                
                if (count == 0)
                    icon.material = InventorySettingsManager.instance.iconDepletedMaterial;
                else
                    icon.material = InventorySettingsManager.instance.iconDefaultMaterial;
            }
            else
            {
                amountText.text = string.Empty;
                icon.material = InventorySettingsManager.instance.iconDefaultMaterial;
            }
        }

        public override void TriggerUse()
        {
            base.TriggerUse();
        }
    }
}