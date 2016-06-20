using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Models
{
    /// <summary>
    /// A helper class that is used to verify if a set of items can be added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemCollectionBaseAddCounter
    {
        public sealed class Tuple
        {
            private uint? _itemID;

            public uint? itemID
            {
                get { return _itemID; }
                set
                {
                    _itemID = value;
                    if (_itemID == null)
                    {
                        _amount = 0;
                    }
                }
            }

            private uint _amount;
            public uint amount
            {
                get { return _amount; }
                set
                {
                    _amount = value;
                    if (_amount == 0)
                    {
                        _itemID = null;
                    }
                }
            }

            public uint? blockedBy { get; set; }


            public Tuple(uint? itemID, uint amount)
            {
                this.itemID = itemID;
                this.amount = amount;
                this.blockedBy = null;
            }

            public void Reset()
            {
                itemID = null;
                amount = 0;
                blockedBy = null;
            }
        }

        public class CollectionLookup : InventoryCollectionLookup<Tuple[]>
        {

            public ItemCollectionBase collectionRef { get; set; }

            public CollectionLookup(Tuple[] collection, int priority, ItemCollectionBase collectionRef)
                : base(collection, priority)
            {
                this.collectionRef = collectionRef;
            }
        }



        private readonly List<CollectionLookup> _collections = new List<CollectionLookup>();
//        private Tuple[] _tuples = new Tuple[0];

        public List<CollectionLookup> collections
        {
            get
            {
                return _collections;
            }
        } 


        public ItemCollectionBaseAddCounter()
        {
            
        }

        public ItemCollectionBaseAddCounter(params InventoryCollectionLookup<ItemCollectionBase>[] collection)
        {
            LoadFrom(collection);
        }

        public void LoadFrom(params InventoryCollectionLookup<ItemCollectionBase>[] collectionsToLoadFrom)
        {
            _collections.Clear();
            foreach (var col in collectionsToLoadFrom)
            {
                var lookup = new CollectionLookup(null, col.priority, col.collection);
                LoadCollectionTuples(lookup);
                _collections.Add(lookup);
            }
        }

        public void LoadFrom(params ItemCollectionBase[] collections)
        {
            LoadFrom(collections.Select(o => new InventoryCollectionLookup<ItemCollectionBase>(o, 50)).ToArray());
        }

        private void LoadCollectionTuples(CollectionLookup lookup)
        {
            var items = lookup.collectionRef.items.Select(o => o.item).ToArray();
            
            lookup.collection = new Tuple[items.Length];
            for (int i = 0; i < lookup.collection.Length; i++)
            {
                lookup.collection[i] = new Tuple(null, 0);
            }

            for (uint i = 0; i < lookup.collection.Length; i++)
            {
                // Only set those with an item to avoid overwriting old changes.
                if (items[i] != null)
                {
                    var t = lookup.collection[i];
                    t.itemID = items[i].ID;
                    t.amount = items[i].currentStackSize;

                    SetItem(lookup, i, t);
                }
            }
        }


        public void SetItem(CollectionLookup lookup, uint i, Tuple t)
        {
            Assert.IsTrue(i < lookup.collection.Length, "Index out of bounds.");

            lookup.collection[i] = t;
            if (t.itemID != null)
            {
                var item = ItemManager.database.items[t.itemID.Value];
                if (item.layoutSize > 1)
                {
                    for (int col = 0; col < item.layoutSizeCols; col++)
                    {
                        for (int row = 0; row < item.layoutSizeRows; row++)
                        {
                            if (row == 0 && col == 0)
                                continue;

                            var iTemp = (uint)(i + col + (row * lookup.collectionRef.colsCount));
                            if (iTemp >= lookup.collection.Length)
                            {
                                continue;
                            }

                            lookup.collection[iTemp].blockedBy = i;
                        }
                    }
                }
            }
        }
        
        public CollectionLookup GetBestCollectionForItem(InventoryItemBase item)
        {
            CollectionLookup best = null;

            foreach (var lookup in _collections)
            {
                if (CanAddItem(lookup, item))
                {
                    if (best == null)
                        best = lookup;
                    else if (lookup.priority > best.priority)
                        best = lookup;
                }
            }

            return best;
        }

        /// <summary>
        /// Get the total weight of all items inside this collection
        /// </summary>
        /// <returns></returns>
        public float GetWeight(CollectionLookup lookup)
        {
            return lookup.collection.Sum(o => o.itemID == null ? 0.0f : ItemManager.database.items[o.itemID.Value].weight * o.amount);
        }


        public bool CanAddItem(CollectionLookup lookup, InventoryItemBase item)
        {
            return CanAddItemCount(lookup, item, item.currentStackSize) >= item.currentStackSize;
        }

        public uint CanAddItemCount(CollectionLookup lookup, InventoryItemBase itemToAdd, uint earlyBailAmount)
        {
            if (lookup.collectionRef.canPutItemsInCollection == false)
                return 0;

            if (lookup.collectionRef.useReferences)
                return 0;

            if (lookup.collectionRef.VerifyFilters(itemToAdd) == false)
                return 0;

            if (lookup.collectionRef.VerifyCustomConditionals(itemToAdd) == false)
                return 0;

            int weightLimit = 99999;
            if (lookup.collectionRef.restrictByWeight && itemToAdd.weight > 0.0f) // avoid dividing by 0.0f
            {
                float weightSpace = lookup.collectionRef.restrictMaxWeight - GetWeight(lookup);
                weightLimit = Mathf.FloorToInt(weightSpace / itemToAdd.weight);
            }

            StartTest(lookup);
            float amount = 0f;
            for(uint i = 0; i < lookup.collection.Length; i++)
            {
                if (amount >= earlyBailAmount)
                {
                    break; // Got enough to pass.
                }

                var item = lookup.collection[i];
                if (item.itemID != null && item.itemID == itemToAdd.ID)
                {
                    amount += Mathf.Clamp((int)itemToAdd.maxStackSize - (int)item.amount, 0, 999999);
                }
                else if (item.itemID == null)
                {
                    if (CanSetItem(i, itemToAdd, lookup))
                    {
                        var t = lookup.collection[i];
                        t.itemID = itemToAdd.ID;
                        t.amount = itemToAdd.currentStackSize;

                        SetItem(lookup, i, t); // Set state -> Test is active, which will revert changes.

                        amount += itemToAdd.maxStackSize;
                    }
                }
            }
            EndTest(lookup);

            return (uint)Mathf.Min(amount, weightLimit);
        }

        private Tuple[] testTuples = new Tuple[0];
        public void StartTest(CollectionLookup lookup)
        {
            // Copy all data so that it can be restored later.
            testTuples = new Tuple[lookup.collection.Length];
            for (int i = 0; i < testTuples.Length; i++)
            {
                testTuples[i] = new Tuple(lookup.collection[i].itemID, lookup.collection[i].amount)
                {
                    blockedBy = lookup.collection[i].blockedBy
                };
            }
        }

        public void EndTest(CollectionLookup lookup)
        {
            lookup.collection = new Tuple[testTuples.Length];
            testTuples.CopyTo(lookup.collection, 0);

            testTuples = new Tuple[0];
        }



        public bool CanSetItem(uint toSlot, InventoryItemBase item, CollectionLookup collection)
        {
            if (item == null)
            {
                return true;
            }

            if (collection.collectionRef.ignoreItemLayoutSizes == false)
            {
                #region Blocked by other object

                if (collection.collection[toSlot].blockedBy.HasValue)
                {
                    // Item has no collection, so can't validate if it blocks itself. This field is blocked, abort..
                    if (item.itemCollection != collection.collectionRef)
                    {
                        return false;
                    }

                    // Not blocked by self?
                    if (collection.collection[toSlot].blockedBy != item.index)
                    {
                        return false;
                    }
                }

                for (int col = 0; col < item.layoutSizeCols; col++)
                {
                    for (int row = 0; row < item.layoutSizeRows; row++)
                    {
                        if (row == 0 && col == 0)
                            continue;

                        var checkSlot = (uint)(toSlot + col + (row * collection.collectionRef.colsCount));
                        if (checkSlot >= collection.collectionRef.items.Length)
                        {
                            return false; // Out of bounds
                                            // break;
                        }

                        if (collection.collection[checkSlot].itemID.HasValue)
                        {
                            return false;
                        }
                        
                        if (collection.collection[checkSlot].blockedBy.HasValue)
                        {
                            return false;
                        }
                    }
                }

                #endregion


                #region Layout out of bounds

                var col2 = (int)(toSlot % collection.collectionRef.colsCount);
                var row2 = Mathf.FloorToInt((float)toSlot / collection.collectionRef.colsCount);
                if (row2 + item.layoutSizeRows - 1 >= collection.collectionRef.rowsCount)
                    return false;

                if (col2 + item.layoutSizeCols - 1 >= collection.collectionRef.colsCount)
                    return false;

                #endregion
            }


            if (collection.collectionRef.canPutItemsInCollection == false)
                return false;

            if (collection.collectionRef.restrictByWeight && collection.collectionRef.GetWeight() + item.weight * item.currentStackSize > collection.collectionRef.restrictMaxWeight)
                return false; // To much weight

            if (collection.collectionRef.VerifyFilters(item) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Try adding a list of items.
        /// </summary>
        /// <param name="itemsToAdd"></param>
        /// <returns>All items that couldn't be added. If returnValue.Length == 0 the action was sucesful.</returns>
        public InventoryItemAmountRow[] TryAdd(IList<InventoryItemAmountRow> itemsToAdd)
        {
            Assert.IsFalse(itemsToAdd.Any(o => o.amount > o.item.maxStackSize), "Item amount is exceeding max stack size.");
            Assert.IsFalse(itemsToAdd.Any(o => o.item == null), "Given array contains an empty item! (NULL)");

            itemsToAdd = itemsToAdd.OrderByDescending(o => o.item.layoutSize).ToArray(); // Order from large to small to make sure large items can always be placed.

            var unAddedItems = new List<InventoryItemAmountRow>(itemsToAdd.Count);
            unAddedItems.AddRange(itemsToAdd);

            for (int j = 0; j < itemsToAdd.Count; j++)
            {
                bool added = false;
                var collection = GetBestCollectionForItem(itemsToAdd[j].item);
                if (collection == null)
                {
                    break;
                }

                for (int i = 0; i < collection.collection.Length; i++)
                {
                    if (collection.collection[i].itemID == itemsToAdd[j].item.ID)
                    {
                        if (collection.collection[i].amount + itemsToAdd[j].amount <= itemsToAdd[j].item.maxStackSize)
                        {
                            // Doesn't exceed stack size.

                            collection.collection[i].amount += itemsToAdd[j].amount;

                            unAddedItems[j] = new InventoryItemAmountRow(null, 0);
                            added = true;
                            break;
                        }

                        // Exceeds stack size, try to add as much as possible.
                        uint canAddAmount = itemsToAdd[j].item.maxStackSize - collection.collection[i].amount;
                        unAddedItems[j].SetAmount(unAddedItems[j].amount - canAddAmount);
                        collection.collection[i].amount += canAddAmount;
                    }
                }

                if (added == false)
                {
                    for (uint i = 0; i < collection.collection.Length; i++)
                    {
                        if (collection.collection[i].itemID == null)
                        {
                            if (CanSetItem(i, itemsToAdd[j].item, collection))
                            {
                                var t = collection.collection[i];
                                t.itemID = itemsToAdd[j].item.ID;
                                t.amount = itemsToAdd[j].item.currentStackSize;

                                SetItem(collection, i, t);

                                unAddedItems[j] = new InventoryItemAmountRow(null, 0);
                                break;
                            }
                        }
                    }
                }
            }

            unAddedItems.RemoveAll(o => o.item == null || o.amount == 0);
            return unAddedItems.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsToAdd"></param>
        /// <returns>Returns all items that couldn't be added. AkA failed action is returnArr.Length > 0</returns>
        public InventoryItemAmountRow[] TryAdd(IList<InventoryItemBase> itemsToAdd)
        {
            return TryAdd(InventoryItemUtility.ItemsToRows(itemsToAdd));
        }

        public bool TryRemoveItems(IList<InventoryItemBase> itemsToRemove)
        {
            return TryRemoveItems(InventoryItemUtility.ItemsToRows(itemsToRemove));
        }

        public bool TryRemoveItems(IList<InventoryItemAmountRow> itemsToRemove)
        {
            return TryRemoveItems(itemsToRemove.Select(o => new Tuple(o.item.ID, o.amount)).ToArray());
        }

        private bool TryRemoveItems(params Tuple[] tuples)
        {
            int itemsRemoved = 0;
            for (int i = 0; i < tuples.Length; i++)
            {
                var toRemove = tuples[i].amount;
                foreach (var collection in _collections)
                {
                    if (toRemove <= 0)
                        break;

                    foreach (var tuple in GetStacksSmallestToLargest(collection, tuples[i].itemID.Value))
                    {
                        if (tuple.itemID != null && tuple.itemID == tuples[i].itemID)
                        {
                            if (tuple.amount >= toRemove)
                            {
                                // Remove all at once
                                tuple.amount -= toRemove;
                                toRemove = 0;
                                itemsRemoved++;

                                if (tuple.amount == 0)
                                {
                                    tuple.Reset();
                                }

                                break;
                            }

                            if (tuple.amount < toRemove)
                            {
                                toRemove -= tuple.amount;
                                tuple.Reset();
                            }
                        }
                    }
                }
            }
            return tuples.Length == itemsRemoved;
        }

        public void RemoveSlot(CollectionLookup lookup, uint slot)
        {
            lookup.collection[slot].Reset();
            foreach (var source in lookup.collection.Where(o => o.blockedBy == slot))
            {
                source.Reset();
            }
        }

        public Tuple[] GetStacksSmallestToLargest(CollectionLookup lookup, uint itemID)
        {
            return lookup.collection.Where(o => o.itemID == itemID).OrderBy(o => o.amount).ToArray();
        }

        public CollectionLookup FindLookup(ItemCollectionBase collection)
        {
            return _collections.FirstOrDefault(o => o.collectionRef == collection);
        }

        public bool CanRemoveSlots(CollectionLookup lookup, uint amount)
        {
            if (lookup.collection.Length - (int) amount < 0)
            {
                return false;
            }

            uint oldSize = (uint)lookup.collection.Length;
            uint newSize = oldSize - amount;

            for (uint i = newSize; i < oldSize; i++)
            {
                if (lookup.collection[i].itemID != null || lookup.collection[i].blockedBy.HasValue)
                {
                    return false;
                }
            }

            return true;
        }

        public uint GetEmptySlotCount()
        {
            uint count = 0;
            foreach (var col in _collections)
            {
                count += (uint)col.collection.Sum(o => o.itemID == null && o.blockedBy == null ? 1 : 0);
            }

            return count;
        }
    }
}
