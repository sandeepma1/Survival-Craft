using System;
using UnityEngine;
using System.Collections;
using System.IO;
using Devdog.InventorySystem.Models;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Managers/Item manager")]
    [RequireComponent(typeof(InventorySettingsManager))]
    [RequireComponent(typeof(InventoryManager))]
    [RequireComponent(typeof(InventoryTriggererManager))]
    [RequireComponent(typeof(InventoryPlayerManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    public partial class ItemManager : MonoBehaviour
    {
        [InventoryRequired]
        [SerializeField]
        [FormerlySerializedAs("itemDatabase")]
        private InventoryItemDatabase _sceneItemDatabase;
        public InventoryItemDatabase sceneItemDatabase
        {
            get { return _sceneItemDatabase; }
        }

        private static InventoryDatabaseLookup<InventoryItemDatabase> _itemDatabaseLookup;
        public static InventoryDatabaseLookup<InventoryItemDatabase> itemDatabaseLookup
        {
            get
            {
                if (_itemDatabaseLookup == null)
                {
                    _itemDatabaseLookup = new InventoryDatabaseLookup<InventoryItemDatabase>(instance != null ? instance._sceneItemDatabase : null, CurrentItemDBPathKey);
                }

                return _itemDatabaseLookup;
            }
        }

        private static string CurrentDBPrefixName
        {
            get
            {
                var path = Application.dataPath;
                if (path.Length > 0)
                {
                    var pathElems = path.Split('/');
                    return pathElems[pathElems.Length - 2];
                }

                return "";
            }
        }

        private static string CurrentItemDBPathKey
        {
            get { return CurrentDBPrefixName + "InventorySystem_CurrentItemDatabasePath"; }
        }

        public static InventoryItemDatabase database
        {
            get
            {
                return itemDatabaseLookup.GetDatabase();
            }
            private set
            {
                itemDatabaseLookup.defaultDatabase = value;
            }
        }

        #region Convenience properties

        [Obsolete("Use ItemManager.database.items instead.")]
        public InventoryItemBase[] items { get { return _sceneItemDatabase.items; } set { _sceneItemDatabase.items = value; }}
        [Obsolete("Use ItemManager.database.currencies instead.")]
        public InventoryCurrency[] currencies { get { return _sceneItemDatabase.currencies; } set { _sceneItemDatabase.currencies = value; }}
        [Obsolete("Use ItemManager.database.itemRarities instead.")]
        public InventoryItemRarity[] itemRarities { get { return _sceneItemDatabase.itemRarities; } set { _sceneItemDatabase.itemRarities = value; } }
        [Obsolete("Use ItemManager.database.itemCategories instead.")]
        public InventoryItemCategory[] itemCategories { get { return _sceneItemDatabase.itemCategories; } set { _sceneItemDatabase.itemCategories = value; } }
        [Obsolete("Use ItemManager.database.properties instead.")]
        public InventoryItemProperty[] properties { get { return _sceneItemDatabase.properties; } set { _sceneItemDatabase.properties = value; } }
        [Obsolete("Use ItemManager.database.equipStats instead.")]
        public InventoryEquipStat[] equipStats { get { return _sceneItemDatabase.equipStats; } set { _sceneItemDatabase.equipStats = value; } }
        [Obsolete("Use ItemManager.database.equipStatTypes instead.")]
        public string[] equipStatTypes { get { return _sceneItemDatabase.equipStatTypes; } set { _sceneItemDatabase.equipStatTypes = value; } }
        [Obsolete("Use ItemManager.database.equipTypes instead.")]
        public InventoryEquipType[] equipTypes { get { return _sceneItemDatabase.equipTypes; } set { _sceneItemDatabase.equipTypes = value; } }
        [Obsolete("Use ItemManager.database.craftingCategories instead.")]
        public InventoryCraftingCategory[] craftingCategories { get { return _sceneItemDatabase.craftingCategories; } set { _sceneItemDatabase.craftingCategories = value; } }

        #endregion


        private static ItemManager _instance;
        public static ItemManager instance {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ItemManager>();
                }

//                if (_instance == null)
//                {
//                    throw new ManagerNotFoundException("ItemManager");    
//                }

                return _instance;
            }
        }


        public void Awake()
        {
            _instance = this;

#if UNITY_EDITOR
            if (itemDatabaseLookup == null)
                Debug.LogError("Item Database is not assigned!", transform);

#endif
        }

    }
}

// using UnityEditor;