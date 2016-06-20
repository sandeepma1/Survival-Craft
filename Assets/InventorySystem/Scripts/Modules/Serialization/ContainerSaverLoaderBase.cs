using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public abstract class ContainerSaverLoaderBase : SaverLoaderBase
    {
        private IInventoryItemContainer _container;
        protected IInventoryItemContainer container
        {
            get
            {
                if (_container == null)
                {
                    _container = (IInventoryItemContainer)GetComponent(typeof(IInventoryItemContainer));
                }

                return _container;
            }
        }

        public override string saveName
        {
            get
            {
                return SaveNamePrefix + "Container_" + container.uniqueName.ToLower().Replace(" ", "_");
            }
        }

        public override void Save()
        {
            try
            {
                SaveItems(serializer.SerializeItems(container.items), (bool saved) =>
                {
                    Debug.Log("Saved container " + saveName);
                });
            }
            catch (SerializedObjectNotFoundException e)
            {
                Debug.LogWarning(e.Message + e.StackTrace);
            }
            //catch(Exception e)
            //{
            //    Debug.LogWarning(e.Message + e.StackTrace);                    
            //}
        }

        public override void Load()
        {
            try
            {
                LoadItems((object data) =>
                {
                    Debug.Log("Loaded container " + saveName);

                    var model = serializer.DeserializeItems(data);
                    var items = model.SerializationModelsToItems(model.items, false);
                    container.items = items.ToArray();
                    foreach (var item in container.items)
                    {
                        if (item != null)
                        {
                            item.gameObject.SetActive(false);
                            item.transform.SetParent(transform);
                            item.transform.localPosition = Vector3.zero;
                        }
                    }
                });
            }
            catch (SerializedObjectNotFoundException e)
            {
                Debug.LogWarning(e.Message + e.StackTrace);
            }
            //catch(Exception e)
            //{
            //    Debug.LogWarning(e.Message + e.StackTrace);
            //}
        }
    }
}
