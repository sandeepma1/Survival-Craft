using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/serialization-saving-loading/")]
    public abstract class SaverLoaderBase : MonoBehaviour, IItemsSaver, IItemsLoader
    {
        protected IItemsSerializer serializer { get; set; }
        protected IItemsSaver saver { get; set; }
        protected IItemsLoader loader { get; set; }

        protected const string SaveNamePrefix = "InventoryPro_";

        public abstract string saveName { get; }

        public bool loadOnStart = true;
        public bool loadOnLevelLoad = false;
        public bool saveOnApplicationQuit = true;

        /// <summary>
        /// How many frames to wait before loading the data.
        /// This can be useful for 3rd party assets that take longer to initialize.
        /// </summary>
        public int waitFramesBeforeLoading = 0;

        protected virtual void Awake()
        {
            SetSerializer();
            SetSaverLoader();
        }

        protected virtual IEnumerator Start()
        {
            for (int i = 0; i < waitFramesBeforeLoading; i++)
            {
                yield return null;
            }

            if (loadOnStart)
            {
                Load();
            }
        }

        protected virtual IEnumerator OnLevelWasLoaded(int level)
        {
            SetSerializer();
            SetSaverLoader();

            for (int i = 0; i < waitFramesBeforeLoading; i++)
            {
                yield return null;
            }

            if (loadOnLevelLoad)
            {
                Load();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (saveOnApplicationQuit)
            {
                Save();
            }
        }

        public abstract void Save();
        public abstract void Load();

        protected virtual void SetSerializer()
        {
            serializer = new JsonItemsSerializer();
        }

        protected virtual void SetSaverLoader()
        {
            saver = this;
            loader = this;
        }

        public virtual void SaveItems(object serializedData)
        {
            SaveItems(serializedData, b => { });
        }

        public abstract void SaveItems(object serializedData, Action<bool> callback);
        public abstract void LoadItems(Action<object> callback);
    }
}
