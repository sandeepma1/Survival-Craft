using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.UI;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Other/Sync collection")]
    public class SyncCollection : MonoBehaviour
    {
        [InventoryRequired]
        public ItemCollectionBase toSyncFrom;

        [InventoryRequired]
        public ItemCollectionBase toSyncTo;

        [InventoryRequired]
        public UIWindow toSyncFromWindow;

        [InventoryRequired]
        public UIWindow toSyncToWindow;
        

        private CollectionToCollectionSyncer _syncer;

        protected void Awake()
        {
            StartSyncing();
        }
        
        protected void OnDestroy()
        {
            StopSyncing();
        }
        
        public void StartSyncing()
        {
            _syncer = new CollectionToCollectionSyncer(toSyncFrom, toSyncTo);
            _syncer.StartSyncing();

            toSyncFromWindow.OnShow += CopyToOriginal;
            toSyncToWindow.OnShow += CopyToSynced;
        }

        private void CopyToOriginal()
        {
            foreach (var wrapper in toSyncTo.items)
            {
                wrapper.transform.SetParent(toSyncFrom.container);
            }
        }

        private void CopyToSynced()
        {
            foreach (var wrapper in toSyncFrom.items)
            {
                wrapper.transform.SetParent(toSyncTo.container);
            }
        }

        public void StopSyncing()
        {
            _syncer.StopSyncing();
            toSyncFromWindow.OnShow -= CopyToOriginal;
            toSyncToWindow.OnShow -= CopyToSynced;
        }
    }
}
