using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper reference sum")]
    public partial class InventoryUIItemWrapperKeyTrigger : InventoryUIItemWrapper
    {
        public UnityEngine.UI.Text keyCombinationText;
        public string keyCombination;
        
        public override void Repaint()
        {
            base.Repaint();

            if (item != null)
            {
                if (keyCombinationText != null)
                    keyCombinationText.text = keyCombination;
            }
            else
            {
                if (keyCombinationText != null)
                    keyCombinationText.text = keyCombination;
            }
        }

        public override void TriggerUse()
        {
            if (item == null)
                return;

            if (itemCollection.canUseFromCollection == false)
                return;

            if (item != null)
            {
                item.Use();
            }
//            var found = InventoryManager.Find(item.ID, false);
//            if (found != null)
//            {
//                int used = found.Use();
//                if (used >= 0)
//                    found.itemCollection[found.index].Repaint();
//            }
        }
    }
}