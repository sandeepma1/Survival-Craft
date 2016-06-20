#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Get the item's name.")]
    public class GetItemName : FsmStateAction
    {
        public InventoryItemBase item;
        public FsmString result;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            result.Value = item.name;

            Finish();
        }
    }
}

#endif