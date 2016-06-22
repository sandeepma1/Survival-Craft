using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.InventorySystem.UI
{
	/// <summary>
	/// Any window that you want to hide or show through key combination or a helper.
	/// </summary>
	[RequireComponent (typeof(Animator))]
	[AddComponentMenu ("InventorySystem/UI Helpers/UI Interactive Window")]
	public partial class UIWindowInteractive : UIWindow
	{
		#region Variables


		/// <summary>
		/// Keys to toggle this window
		/// </summary>
		public KeyCode[] keyCombination;


		public virtual bool keysDown {
			get {
				if (keyCombination.Length == 0)
					return false;

				foreach (var keyCode in keyCombination) {
					if (Input.GetKeyDown (keyCode))
						return true;
				}

				return false;
			}
		}


		#endregion


		public virtual void Update ()
		{
			if (keysDown) {
				if (InventoryUIUtility.CanReceiveInput (gameObject))
					Toggle ();

			}
		}

		[Header ("Actions")]
		public UIWindowActionEvent TrigggerUI = new UIWindowActionEvent ();

		public void NotifyTrigggerUI ()
		{
			TrigggerUI.Invoke ();

			if (InventoryUIUtility.CanReceiveInput (gameObject))
				Toggle ();

		}
		/*public virtual void Update()
        {
            if (keysDown)
            {
                if (InventoryUIUtility.CanReceiveInput(gameObject))
                    Toggle();

            }
        }*/
	}
}