using System;
using UnityEngine;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
	[HelpURL ("http://devdog.nl/documentation/infoboxui/")]
	[RequireComponent (typeof(UIWindow))]
	[AddComponentMenu ("InventorySystem/Windows Other/Infobox")]
	public partial class InfoBoxUI : MonoBehaviour
	{
		public delegate void Repainted (InventoryItemBase item, LinkedList<InventoryItemInfoRow[]> rows);

		public event Repainted OnRepainted;


		/// <summary>
		/// All information will be appended to the container.
		/// </summary>
		[Header ("UI")]
		public RectTransform container;

		// Default fields
		public Image uiIcon;
		public Text uiName;
		public Text uiDescription;



		/// <summary>
		/// Show the dialog window when hovering a lootable item.
		/// </summary>
		[Header ("UI behavior")]
		public bool showWhenHoveringLootableObject = true;

		public bool showWhenHoveringWrapper = true;

		/// <summary>
		/// Should the window be shown / hidden when a new item becomes visible / invisible.
		/// </summary>
		public bool showHideWindow = true;

		/// <summary>
		/// Show the dialog for out of range items?
		/// </summary>
		public bool showOutOfRangeLootables = false;

		/// <summary>
		/// Position the info box at the cursor's position?
		/// </summary>
		public bool positionAtMouse = true;

		/// <summary>
		/// Show the info box for the best triggerer ( not the hovering one )
		/// </summary>
		public bool showForBestTriggerer = true;

		/// <summary>
		/// When the InfoBoxUI hits the right or left part of the screen it will move to the other side.
		/// </summary>
		[Header ("Borders")]
		public bool moveWhenHitBorderHorizontal = true;

		/// <summary>
		/// When the InfoBoxUI hits the top or bottom part of the screen it will move to the other side.
		/// </summary>
		public bool moveWhenHitBorderVertical = true;

		/// <summary>
		/// Used to define extra margin on the corners of the screen.
		/// If the item falls of the screen it will be shown on the other side of the cursor.
		/// </summary>
		public Vector2 borderMargins;

		[Header ("UI element prefabs")]
		[InventoryRequired]
		public GameObject infoBoxCategory;
		public GameObject separatorPrefab;
		[InventoryRequired]
		public InfoBoxRowUI infoBoxRowPrefab;
		// 1 item (row) inside the infobox



		private RectTransform rectTransform { get; set; }

		private Vector2 defaultPivot { get; set; }

		/// <summary>
		/// Used to avoid continuous repainting.
		/// </summary>
		protected InventoryItemBase currentItem { get; set; }

		private UIWindow window { get; set; }

		private InventoryPool<InfoBoxRowUI> poolRows { get; set; }

		private InventoryPool poolSeparators { get; set; }

		private InventoryPool poolCategoryBoxes { get; set; }


		private bool isHoveringWrapperItem {
			get {
				return InventoryUIUtility.currentlyHoveringWrapper != null && InventoryUIUtility.currentlyHoveringWrapper.item != null;
			}
		}

		protected virtual void Awake ()
		{
			rectTransform = GetComponent<RectTransform> ();
			defaultPivot = rectTransform.pivot;
			window = GetComponent<UIWindow> ();

			poolRows = new InventoryPool<InfoBoxRowUI> (infoBoxRowPrefab, 32);
			if (separatorPrefab != null)
				poolSeparators = new InventoryPool (separatorPrefab, 8);

			poolCategoryBoxes = new InventoryPool (infoBoxCategory, 8);


			// Safety checks
			if (GetComponent<InventoryUIItemWrapperBase> () != null && GetComponent<InventoryUIItemWrapperStatic> () == null) {
				Debug.LogError ("Using a InventoryUIItemWrapperBase in the InfoBoxUI, use an InventoryUIItemWrapperStatic instead.", transform);
			}
		}

		protected virtual void Start ()
		{
			if (showWhenHoveringLootableObject) {
				InventoryTriggererManager.instance.OnCursorEnterTriggerer += HandleItemTriggerer;
				InventoryTriggererManager.instance.OnCursorExitTriggerer += triggerer => {
					Hide ();
				};
			}
			if (showForBestTriggerer) {
				InventoryPlayerManager.instance.OnPlayerChanged += InstanceOnOnPlayerChanged;
			}
			if (showWhenHoveringWrapper) {
				InventoryInputManager.instance.OnCursorEnterWrapper += wrapper => HandleInfoBox (wrapper.item);
				InventoryInputManager.instance.OnCursorExitWrapper += wrapper => {
					Hide ();
				};
			}
		}

		private void InstanceOnOnPlayerChanged (InventoryPlayer oldPlayer, InventoryPlayer newPlayer)
		{
			if (oldPlayer != null) {
				try {
					oldPlayer.rangeHelper.OnChangedBestTriggerer -= RangeHelperOnOnChangedBestTriggerer;
				} catch (Exception) {
					// ignored
				}
			}

			newPlayer.rangeHelper.OnChangedBestTriggerer += RangeHelperOnOnChangedBestTriggerer;
		}

		private void RangeHelperOnOnChangedBestTriggerer (ObjectTriggererBase old, ObjectTriggererBase newBest)
		{
			HandleItemTriggerer (newBest);
		}

		protected virtual void HandleItemTriggerer (ObjectTriggererBase triggerer)
		{
			if (triggerer != null) {
				if (triggerer.inRange == false && showOutOfRangeLootables == false)
					return;

				var a = triggerer as ObjectTriggererItem;
				if (a != null) {
					bool shouldDestroySource;
					HandleInfoBox (a.GetItem (out shouldDestroySource));

					return;
				}
			}

			if (isHoveringWrapperItem == false)
				Hide ();
		}

		public virtual void HandleInfoBox (InventoryItemBase forItem)
		{
			if (forItem == null) {
				Hide ();
				return;
			}

			if (forItem != currentItem) {
				currentItem = forItem;
				Repaint (currentItem, currentItem.GetInfo ());
			}

			if (showHideWindow) {
				window.Show ();
			}
		}

		protected virtual void HandleBorders ()
		{
			if (InventorySettingsManager.instance.guiRoot.renderMode == RenderMode.WorldSpace)
				return;


			if (moveWhenHitBorderHorizontal) {
				// Change the box if its about to fall of the screen
				if (Input.mousePosition.x + rectTransform.sizeDelta.x > Screen.width - borderMargins.x) {
					// Falls of the right
					rectTransform.pivot = new Vector2 (defaultPivot.y, rectTransform.pivot.x); // Swap
				} else {
					rectTransform.pivot = new Vector2 (defaultPivot.x, rectTransform.pivot.y); // Swap
				}
			}

			if (moveWhenHitBorderVertical) {
				if (Input.mousePosition.y - rectTransform.sizeDelta.y < 0.0f - borderMargins.y) {
					// Falls of the bottom
					rectTransform.pivot = new Vector2 (rectTransform.pivot.x, defaultPivot.x); // Swap                
				} else {
					rectTransform.pivot = new Vector2 (rectTransform.pivot.x, defaultPivot.y); // Swap
				}
			}
		}

		protected virtual void PositionInfoBox ()
		{
			if (positionAtMouse) {
				rectTransform.anchoredPosition = Input.mousePosition;
				HandleBorders ();
			}
		}

		protected virtual void Hide ()
		{
			currentItem = null;
			if (showHideWindow)
				window.Hide ();

		}

		protected virtual void LateUpdate ()
		{
			if (window.isVisible)
				PositionInfoBox ();
		}

		/// <summary>
		/// Repaint the infobox with the given data.
		/// </summary>
		/// <param name="item">The item we're going to display</param>
		/// <param name="rows">The rows of data we're displaying</param>
		protected virtual void Repaint (InventoryItemBase item, LinkedList<InventoryItemInfoRow[]> rows)
		{
			poolRows.DestroyAll ();
			poolSeparators.DestroyAll ();
			poolCategoryBoxes.DestroyAll ();

			// The usual stuff
			if (uiIcon != null)
				uiIcon.sprite = item.icon;
            
			if (uiName != null) {
				uiName.text = item.name;
				uiName.color = (item.rarity != null) ? item.rarity.color : uiName.color;
			}
			if (uiDescription != null)
				uiDescription.text = item.description;

			int i = 0;        
			foreach (var box in rows) {
				i++;

				var boxObj = poolCategoryBoxes.Get ();

				foreach (var row in box) {
					var rowObj = poolRows.Get ();
					rowObj.transform.SetParent (boxObj.transform);
					InventoryUtility.ResetTransform (rowObj.transform);

					rowObj.title.text = row.title;
					rowObj.title.color = row.titleColor;

					rowObj.message.text = row.text;
					rowObj.message.color = row.textColor;
				}

				boxObj.transform.SetParent (container);
				InventoryUtility.ResetTransform (boxObj.transform);

				if (i < rows.Count && separatorPrefab != null) {
					// Add a separator
					if (separatorPrefab != null) {
						var separator = poolSeparators.Get ();
						separator.transform.SetParent (container);
						InventoryUtility.ResetTransform (separator.transform);
					}
				}
			}

			if (OnRepainted != null)
				OnRepainted (item, rows);
		}
	}
}