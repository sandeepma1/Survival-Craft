using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/player/")]
    public abstract class InventoryPlayerBase : MonoBehaviour
    {

//          TODO: Unity doesn't handle generics, maybe in the future??
//        [System.Serializable]
//        public struct DynamicField<T> where T : ItemCollectionBase
//        {
//            private T _reference;
//            public T reference
//            {
//                get
//                {
//                    if (_reference != null)
//                    {
//                        return _reference;
//                    }
//
//                    _reference = GameObject.Find(path).GetComponent<T>();
//                    return _reference;
//                }
//            }
//            public string path { get; private set; }
//
//            public DynamicField(string path)
//            {
//                this.path = path;
//                this._reference = null;
//            }
//        }
//
//        public DynamicField<CharacterUI> characterCols;
//        public DynamicField<ItemCollectionBase>[] inventories;
//        public DynamicField<SkillbarUI> skillbars;




        // Reference based stuff
        public CharacterUI characterCollection;
        public ItemCollectionBase[] inventoryCollections = new ItemCollectionBase[0];
        public SkillbarUI skillbarCollection;


        ///////// Instantiation stuff
        public bool dynamicallyFindUIElements = false;

        public string characterCollectionName = "Character";
        public string[] inventoryCollectionNames = new string[] { "Inventory" };
        public string skillbarCollectionName = "Skillbar";



        public InventoryPlayerEquipTypeBinder[] equipLocations = new InventoryPlayerEquipTypeBinder[0];



        protected virtual void Awake()
        {

        }

        protected virtual void UpdateEquipLocations()
        {
            foreach (var equipLoc in equipLocations)
            {
                if (equipLoc.findDynamic)
                {
                    Transform equipTransform = null;
                    InventoryUtility.FindChildTransform(transform, equipLoc.equipTransformPath, ref equipTransform);
                    equipLoc.equipTransform = equipTransform;

                    Assert.IsNotNull(equipLoc.equipTransform, "Equip transform path is not valid (" + equipLoc.equipTransformPath + ")");
                }
            }
        }


        public virtual void FindUIElements(bool warnWhenNotFound = true)
        {
            characterCollection = FindElement<CharacterUI>(characterCollectionName, warnWhenNotFound);
            inventoryCollections = FindUIElements<ItemCollectionBase>(inventoryCollectionNames, warnWhenNotFound);
            skillbarCollection = FindElement<SkillbarUI>(skillbarCollectionName, warnWhenNotFound);
        }

        public T[] FindUIElements<T>(string[] collectionNames, bool warnWhenNotFound) where T : ItemCollectionBase
        {
            T[] comps = new T[collectionNames.Length];
            for (int i = 0; i < collectionNames.Length; i++)
            {
                comps[i] = FindElement<T>(collectionNames[i], warnWhenNotFound);
            }

            return comps;
        }

        public T FindElement<T>(string collectionName, bool warnWhenNotFound) where T : ItemCollectionBase
        {
            var f = FindObjectsOfType<T>();
            foreach (var col in f)
            {
                if (col.collectionName == collectionName)
                {
                    return col;
                }
            }

            if (warnWhenNotFound)
            {
                Debug.LogWarning("Player instantiation :: Collection with name (" + collectionName + ") not found!");
            }

            return null;
        }



    }
}