using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Devdog.InventorySystem
{
    public class SkillExampleInventoryItemType : InventoryItemBase
    {

        public InventoryAudioClip audioClipWhenUsed = new InventoryAudioClip();


        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var info = base.GetInfo();
            info.Remove(info.First.Next);

            return info;
        }


        public override void NotifyItemUsed(uint amount, bool alsoNotifyCollection)
        {
            base.NotifyItemUsed(amount, alsoNotifyCollection);

            InventoryItemUtility.SetItemProperties(InventoryPlayerManager.instance.currentPlayer, properties, InventoryItemUtility.SetItemPropertiesAction.Use);
        }

        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;

            NotifyItemUsed(1, true);
            InventoryAudioManager.AudioPlayOneShot(audioClipWhenUsed);

            return 1; // 1 item used
        }
    }
}