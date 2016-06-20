#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Get the item count from a given collection.")]
    public class GetItemCount : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public InventoryItemBasePlaymakerWrapper item;

        [UIHint(UIHint.Variable)]
        public ItemCollectionBase collection;


        public FsmInt result;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            result.Value = (int)collection.GetItemCount(item.item.ID);

            Finish();
        }
    }
}

#endif