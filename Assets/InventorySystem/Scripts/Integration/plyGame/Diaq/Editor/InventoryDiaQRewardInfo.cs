﻿#if PLY_GAME

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Editors;
using Devdog.InventorySystem.Models;
using plyCommon;
using plyGame;
using DiaQ;
using plyCommonEditor;
using plyCommonEditor.FontAwesome;
using plyGameEditor;
using UnityEditor;

namespace Devdog.InventorySystem.Integration.plyGame.DiaQ.Editors
{
    public class InventoryDiaQRewardInfo : plyDataProviderInfo
    {

        private InventoryItemPicker _itemPicker;
        private InventoryItemPicker itemPicker
        {
            get
            {
                if (_itemPicker == null)
                {
                    _itemPicker = InventoryItemPicker.Get();
                    _itemPicker.Close(true);
                }

                return _itemPicker;
            }
        }

        private InventoryItemBase selectedItem { get; set; }
        private InventoryCurrency selectedCurrency { get; set; }
        private static string[] Options = { "Currency", "Item" };
        private int selected = 0;
        private int selectedCurrencyID = 0;
        private int selectedIdent = -1;
        private DataAsset dataAsset;
        private ActorAttributesAsset attribsAsset;
        private ItemsAsset itemsAsset;
        private string[] attribNames = new string[0];



		/// <summary> Unique name to identify the provider by </summary>
		public override string ProviderName()
		{
			return "Inventory Pro";
		}

		/// <summary> Context is important to identify where this provider can be used.
		/// The default is "data", meaning it can provider data and is able to set 
		/// its data. That is, it implements DataProvider_GetValue and DataProvider_SetValue.
		/// Where a system, using DataProviders, are more specialised it will indicate what
		/// context it expects if it do not function purely on get/set of data. </summary>
		public override string ProviderContext()
		{
			return "DiaQReward";
		}

		/// <summary> Return a nice name that identifies the data. Shown in the button
		/// used to open the data provider editor window for setup. </summary>
		public override string PrettyName(plyDataObject data, string emptyText)
		{
			if (data.nfo[0] == "0")
                return "Currency: " + data.nfo[2];
            
            if (data.nfo[0] == "1")
                return "Inventory Pro item: " + data.nfo[2];

            return emptyText;
		}

		/// <summary> Init the target type with this when the provider is selected </summary>
		public override plyDataObject.TargetObjectType DefaultTargetType()
		{
			return plyDataObject.TargetObjectType.Name;
		}

		/// <summary> Init the target type data this when the provider is selected </summary>
		public override string DefaultTargetTypeData()
		{
			// At runtime "plyRPGDiaQRewardHandler" is placed onto the DiaQ GameObject because I use
			// EdGlobal.RegisterAutoComponent("DiaQ", "plyRPGDiaQRewardHandler"); to make sure it
			// will automatically attach to that GameObject
			return "DiaQ";
		}

		/// <summary> Init the component name with this when the provider is selected.
		/// It is the component that implements plyDataProviderInterface </summary>
		public override string DefaultComponent()
		{
			// this will be the component who's DataProvider_Callback() is called
            return typeof(InventoryProDiaQRewardHandler).Name;
		}

		/// <summary> Init the the nfo[] field with this when the provider is selected </summary>
		public override string[] InitNfo()
		{
			// nfo[0] = 0:Currency, 1:Item
			// nfo[1] = the ID of the item
			return new string[] { "0", "1", "" };
		}

		/// <summary> Return true if user may choose different settings for type and type data in editor </summary>
		public override bool CanChangeType()
		{
			return false;
		}

	
		/// <summary> Called when the data provider is selected </summary>
		public override void NfoFieldFocus(plyDataObject data, EditorWindow ed)
		{
			// make sure the Component that handles the Rewards is registered to be added to DiaQ
            EdGlobal.RemoveAutoComponent("DiaQ", typeof(plyRPGDiaQRewardHandler).AssemblyQualifiedName); // make sure old one is not present as it causes problems
            EdGlobal.RegisterAutoComponent("DiaQ", typeof(InventoryProDiaQRewardHandler).AssemblyQualifiedName);

			selected = 0;
			int.TryParse(data.nfo[0], out selected);

			if (attribsAsset == null)
			{
				if (dataAsset == null) dataAsset = EdGlobal.GetDataAsset();
				attribsAsset = (ActorAttributesAsset)dataAsset.GetAsset<ActorAttributesAsset>();
				if (attribsAsset == null) attribsAsset = (ActorAttributesAsset)EdGlobal.LoadOrCreateAsset<ActorAttributesAsset>(plyEdUtil.DATA_PATH_SYSTEM + "attributes.asset", null);
			}

			if (itemsAsset == null)
			{
				if (dataAsset == null) dataAsset = EdGlobal.GetDataAsset();
				itemsAsset = (ItemsAsset)dataAsset.GetAsset<ItemsAsset>();
				if (itemsAsset == null) itemsAsset = (ItemsAsset)EdGlobal.LoadOrCreateAsset<ItemsAsset>(plyEdUtil.DATA_PATH_SYSTEM + "items.asset", null);
				itemsAsset.UpdateItemCache();
			}

			selectedIdent = -1;
			attribNames = new string[attribsAsset.attributes.Count];
			for (int i = 0; i < attribsAsset.attributes.Count; i++)
			{
				attribNames[i] = attribsAsset.attributes[i].def.screenName;
				if (selected == 1 && selectedIdent < 0)
				{
					if (data.nfo[1].Equals(attribsAsset.attributes[i].id.ToString()))
					{
						selectedIdent = i;
						data.nfo[2] = attribsAsset.attributes[selectedIdent].ToString(); // update cached name just in case it has changed
						GUI.changed = true;
					}
				}
			}
		}

		/// <summary> Called when the nfo[] edit fields should be rendered </summary>
		public override void NfoField(plyDataObject data, EditorWindow ed)
		{
			// nfo[0] = 0:Currency, 1:Item
			// nfo[1] = the identifier of the attribute or item.(not used with currency selected)
			// nfo[2] = cached name of selected attribute or item

			EditorGUI.BeginChangeCheck();
			selected = EditorGUILayout.Popup("Reward Type", selected, Options);
			if (EditorGUI.EndChangeCheck())
			{
				data.nfo[0] = selected.ToString();
                data.nfo[1] = "-1";
                data.nfo[2] = "";
			}

            if (selected == 0)
            {
                selectedCurrencyID = EditorGUILayout.Popup("Currency type", selectedCurrencyID, ItemManager.database.singleCurrenciesStrings);

                selectedItem = null;
                selectedCurrency = ItemManager.database.currencies.FirstOrDefault(o => o.ID == selectedCurrencyID);

                data.nfo[0] = "0";
                data.nfo[1] = selectedCurrencyID.ToString();
                data.nfo[2] = selectedCurrency != null ? selectedCurrency.singleName : " (NOT FOUND)";
            }
            else if (selected == 1)
            {
                if (GUILayout.Button("Select item"))
                {
                    //itemPicker = InventoryItemPicker.Get();
                    itemPicker.Show(true);
                    itemPicker.OnPickObject += (item) =>
                    {
                        data.nfo[0] = "1";
                        data.nfo[1] = item.ID.ToString();
                        data.nfo[2] = item.name;
                        selectedItem = item;
                        selectedCurrency = null;
                        //data.nfo[2] = item.currentStackSize.ToString();

                        ed.Repaint();
                        GUI.changed = true;
                    };
                }
            }
		}
    }
}


#endif