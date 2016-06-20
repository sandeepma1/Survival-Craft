#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Use a triggerer.")]
    public class UseTriggerer : FsmStateAction
    {
        public ObjectTriggererBase trigger;
        public FsmBool useOrUnUse;
        
        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            if (useOrUnUse.Value)
            {
                trigger.Use();
            }
            else
            {
                trigger.UnUse();
            }

            Finish();
        }
    }
}

#endif