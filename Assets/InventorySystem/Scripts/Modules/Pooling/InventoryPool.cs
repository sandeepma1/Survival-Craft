using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// Only supports GameObjects and no unique types, just to speed some things up.
    /// It's not ideal and I advice you to only use it on small collections.
    /// </summary>
    public struct InventoryPool<T> : IEnumerable<T> where T : UnityEngine.Component, IPoolableObject, new()
    {
        private readonly List<T> _itemPool;
        private readonly Transform _poolParent;
        private readonly T _baseObject;

        public InventoryPool(T baseObject, int startSize = 64)
        {
            _poolParent = new GameObject("_PoolParent").transform;
            _poolParent.SetParent(InventoryManager.instance.collectionObjectsParent);
            _baseObject = baseObject;
            Assert.IsNotNull(_baseObject, "Pool couldn't instantiate item, baseObject is null!");

            _itemPool = new List<T>(startSize);
            for (int i = 0; i < startSize; i++)
            {
                Instantiate();
            }
        }


        /// <summary>
        /// Create an object inside the pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public T Instantiate()
        {
            Assert.IsNotNull(_baseObject, "Pool couldn't instantiate item, baseObject is null!");

            var a = GameObject.Instantiate<T>(_baseObject);
            a.transform.SetParent(_poolParent);
            a.gameObject.SetActive(false); // Start disabled

            _itemPool.Add(a);
            return a;
        }
    

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Get(bool createWhenNoneLeft = true)
        {
            foreach (var item in _itemPool)
            {
                if(item.gameObject.activeSelf == false)
                {
                    item.gameObject.SetActive(true);
                    return item;
                }
            }

            if (createWhenNoneLeft)
            {
                Debug.Log("New object created, considering increasing the pool size if this is logged often");
                return Instantiate();
            }

            return null;
        }
    

        /// <summary>
        /// Mark an object as inactive so it can be recycled.
        /// </summary>
        /// <param name="item"></param>
        public void Destroy(T item)
        {
            item.Reset(); // Resets the item state
            item.transform.SetParent(_poolParent);
            item.gameObject.SetActive(false); // Up for reuse
        }

        public void DestroyAll()
        {
            foreach (var item in _itemPool)
                Destroy(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _itemPool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// InventoryPool only good for gameObjects
    /// </summary>
    public struct InventoryPool
    {
        private readonly List<GameObject> _itemPool;
        private readonly Transform _poolParent;
        private readonly GameObject _baseObject;

        public InventoryPool(GameObject baseObject, int startSize = 64)
        {
            _poolParent = new GameObject("_PoolParent").transform;
            _poolParent.SetParent(InventoryManager.instance.collectionObjectsParent);
            _baseObject = baseObject;
            Assert.IsNotNull(_baseObject, "Pool couldn't instantiate item, baseObject is null!");

            _itemPool = new List<GameObject>(startSize);
            for (int i = 0; i < startSize; i++)
            {
                Instantiate();
            }
        }


        /// <summary>
        /// Create an object inside the pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public GameObject Instantiate()
        {
            Assert.IsNotNull(_baseObject, "Pool couldn't instantiate item, baseObject is null!");

            GameObject a = null;
            if (_baseObject != null)
                a = GameObject.Instantiate<GameObject>(_baseObject);
            else
                a = new GameObject();

            a.transform.SetParent(_poolParent);
            a.gameObject.SetActive(false); // Start disabled

            _itemPool.Add(a);
            return a;
        }


        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Get(bool createWhenNoneLeft = true)
        {
            foreach (var item in _itemPool)
            {
                if (item.gameObject.activeInHierarchy == false)
                {
                    item.gameObject.SetActive(true);
                    return item;
                }
            }

            if (createWhenNoneLeft)
            {
                Debug.Log("New object created, considering increasing the pool size if this is logged often");
                return Instantiate();
            }

            return null;
        }


        /// <summary>
        /// Mark an object as inactive so it can be recycled.
        /// </summary>
        /// <param name="item"></param>
        public void Destroy(GameObject item)
        {
            item.transform.SetParent(_poolParent);
            item.gameObject.SetActive(false); // Up for reuse
        }


        public void DestroyAll()
        {
            foreach (var item in _itemPool)
                Destroy(item);
        }
    }
}