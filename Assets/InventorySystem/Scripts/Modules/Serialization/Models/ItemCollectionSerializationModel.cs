using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Devdog.InventorySystem.Models
{
    public class ItemCollectionSerializationModel : ItemContainerSerializationModel
    {

        public InventoryCurrencySerializationModel[] currencies = new InventoryCurrencySerializationModel[0];
        

        public ItemCollectionSerializationModel()
            : base()
        { }

        public ItemCollectionSerializationModel(ItemCollectionBase fillUsingCollection)
            : base(fillUsingCollection)
        {

        }

        /// <summary>
        /// Fill this data model with a collection reference.
        /// Gets the collection data from the collection and stores it in this serializable model.
        /// </summary>
        /// <param name="collection"></param>
        public override void FillUsingCollection(ItemCollectionBase collection)
        {
            base.FillUsingCollection(collection);

            FillCurrenciesUsingCollection(collection);
        }

        public virtual void FillCurrenciesUsingCollection(ItemCollectionBase collection)
        {
            var currencyModelsList = new List<InventoryCurrencySerializationModel>(collection.currenciesContainer.lookups.Count);
            foreach(var currency in collection.currenciesContainer.lookups)
            {
                currencyModelsList.Add(new InventoryCurrencySerializationModel(currency._currencyID, currency.amount));
            }
            currencies = currencyModelsList.ToArray();
        }
        
        
        /// <summary>
        /// Fill a collection using this data object.
        /// </summary>
        /// <param name="collection">The collection to fill using this (ItemCollectionSerializationModel) object.</param>
        public override void FillCollectionUsingThis(ItemCollectionBase collection)
        {
            base.FillCollectionUsingThis(collection);

            foreach (var currency in currencies)
            {
                var c = collection.currenciesContainer.lookups.FirstOrDefault(o => o._currencyID == currency.currencyID);
                Assert.IsNotNull(c, "Couldn't find currency on this collection, but is in save file!");

                c.amount += currency.amount;
            }
        }
    }
}
