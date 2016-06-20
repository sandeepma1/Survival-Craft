#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Toggle a triggerer.")]
    public class ToggleTriggerer : FsmStateAction
    {
        public ObjectTriggererBase trigger;
        public FsmBool useOrUnUse;
        
        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            trigger.Toggle();
            Finish();
        }
    }
}

#endif