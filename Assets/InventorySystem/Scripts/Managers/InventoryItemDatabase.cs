using Devdog.InventorySystem.Models;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ItemDatabase.asset", menuName = "InventoryPro/Item Database")]
    public partial class InventoryItemDatabase : ScriptableObject
    {
        [Header("Items")]
        public InventoryItemBase[] items = new InventoryItemBase[0];
        public InventoryItemRarity[] itemRarities = new InventoryItemRarity[] { new InventoryItemRarity(0, "Normal", Color.white, null) };
        public InventoryItemCategory[] itemCategories = new InventoryItemCategory[] { new InventoryItemCategory() { ID = 0, name = "None", cooldownTime = 0.0f } };
        public InventoryItemProperty[] properties = new InventoryItemProperty[0];
        
        [Header("Equipment")]
        public InventoryEquipStat[] equipStats = new InventoryEquipStat[0];
        public string[] equipStatTypes = new string[0];
        public InventoryEquipType[] equipTypes = new InventoryEquipType[0];
        
        [Header("Crafting")]
        public InventoryCraftingCategory[] craftingCategories = new InventoryCraftingCategory[0];

        [Header("Currencies")]
        public InventoryCurrency[] currencies = new InventoryCurrency[] { new InventoryCurrency() { singleName = "Gold", pluralName = "Gold" } };


        // Convenience methods:
        public string[] pluralCurrenciesStrings
        {
            get
            {
                return currencies.Select(o => o.pluralName).ToArray();
            }
        }
        public string[] singleCurrenciesStrings
        {
            get
            {
                return currencies.Select(o => o.singleName).ToArray();
            }
        }

        public string[] craftingCategoriesStrings
        {
            get
            {
                return craftingCategories.Select(o => o.name).ToArray();
            }
        }

        public string[] propertiesStrings
        {
            get
            {
                return properties.Select(o => o.name).ToArray();
            }
        }
        public string[] itemRarityStrings
        {
            get
            {
                return itemRarities.Select(o => o.name).ToArray();
            }
        }

        public Color[] itemRaritiesColors
        {
            get
            {
                return itemRarities.Select(o => o.color).ToArray();
            }
        }
        public string[] itemCategoriesStrings
        {
            get
            {
                return itemCategories.Select(o => o.name).ToArray();
            }
        }

        public string[] equipTypesStrings
        {
            get
            {
                return equipTypes.Select(o => o.name).ToArray();
            }
        }
    }
}
