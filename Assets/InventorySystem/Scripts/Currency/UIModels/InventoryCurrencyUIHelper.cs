using UnityEngine;
using System.Collections;

using Devdog.InventorySystem;

namespace Devdog.InventorySystem.UI
{
    using UnityEngine.UI;

    public class InventoryCurrencyUIHelper : MonoBehaviour
    {
        public uint currencyID;
        public bool allowCurrencyConversions = true;

        public ItemCollectionBase toCollection;
        public Button button;


        public void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    TriggerAddCurrencyToCollection(toCollection);
                });
            }
        }

        public void TriggerAddCurrencyToCollection(ItemCollectionBase collection)
        {
            InventoryManager.instance.intValDialog.ShowDialog(transform, "Amount", "", 1, 9999, value =>
            {
                // Yes callback
                if (InventoryManager.CanRemoveCurrency((float)value, currencyID, allowCurrencyConversions))
                {
                    InventoryManager.RemoveCurrency(value, currencyID);
                    toCollection.AddCurrency(value, currencyID);
                }

            }, value =>
            {
                // No callback

            }
            );
        }
    }
}
