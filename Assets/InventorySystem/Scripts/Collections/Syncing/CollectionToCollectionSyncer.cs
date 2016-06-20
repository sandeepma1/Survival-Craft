using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public partial class CollectionToCollectionSyncer
    {
        public ItemCollectionBase fromCollection { get; protected set; }
        public ItemCollectionBase toCollection { get; protected set; }

        private bool _registered;

        public CollectionToCollectionSyncer(ItemCollectionBase fromCollection, ItemCollectionBase toCollection)
        {
            this.fromCollection = fromCollection;
            this.toCollection = toCollection;

            CopyAll();
//            StartSyncing();
        }

        ~CollectionToCollectionSyncer()
        {
            StopSyncing();
        }

        public void StartSyncing()
        {
            RegisterCollectionEvents();
        }

        public void StopSyncing()
        {
            UnRegisterCollectionEvents();
        }

        public void CopyAll()
        {
            toCollection.items = fromCollection.items;
            toCollection.currenciesContainer = fromCollection.currenciesContainer;
        }

        private void RegisterCollectionEvents()
        {
            if (_registered == false)
            {
                _registered = true;
                fromCollection.OnResized += OnResized;
//                fromCollection.OnCurrencyChanged += OnCurrencyChanged;
            }
        }

        private void UnRegisterCollectionEvents()
        {
            if (_registered)
            {
                _registered = false;
                fromCollection.OnResized -= OnResized;
//                fromCollection.OnCurrencyChanged -= OnCurrencyChanged;
            }
        }

        
//        private void OnCurrencyChanged(float amountBefore, InventoryCurrencyLookup lookup)
//        {
//            CopyAllCurrencies();
//        }

        private void OnResized(uint fromsize, uint tosize)
        {
            CopyAll();
        }
    }
}
