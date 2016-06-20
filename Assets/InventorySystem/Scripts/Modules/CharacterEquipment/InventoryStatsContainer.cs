using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public class InventoryStatsContainer : IEnumerable<KeyValuePair<string, List<IInventoryCharacterStat>>>
    {
        public delegate void StatChanged(IInventoryCharacterStat stat);
        public event StatChanged OnStatChanged;


        private Dictionary<string, List<IInventoryCharacterStat>> _characterStats { get; set; }
        public List<IInventoryStatDataProvider> dataProviders { get; protected set; }


        public InventoryStatsContainer()
            : this(new List<IInventoryStatDataProvider>())
        { }

        public InventoryStatsContainer(List<IInventoryStatDataProvider> dataProviders)
        {
            Assert.IsNotNull(dataProviders, "Dataproviders object given is null!");

            _characterStats = new Dictionary<string, List<IInventoryCharacterStat>>();
            this.dataProviders = dataProviders;
        }

        public KeyValuePair<string, List<IInventoryCharacterStat>>? GetCategory(string category)
        {
            if (ContainsCategory(category) == false)
            {
                return null;
            }

            return _characterStats.FirstOrDefault(o => o.Key == category);
        }


        public bool ContainsCategory(string category)
        {
            return _characterStats.ContainsKey(category);
        }

        public bool ContainsStat(string category, string name)
        {
            return Get(category, name) != null;
        }

        /// <summary>
        /// Convenience method to grab a stat from this character.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IInventoryCharacterStat Get(string category, string name)
        {
            if (ContainsCategory(category) == false)
            {
                return null;
            }

            return _characterStats[category].FirstOrDefault(o => o.statName == name);
        }


        /// <summary>
        /// Add a stat to this character.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="category"></param>
        public void Add(string category, IInventoryCharacterStat stat)
        {
            if (ContainsCategory(category) == false)
            {
                _characterStats.Add(category, new List<IInventoryCharacterStat>());
            }

            if (_characterStats[category].Any(o => o.statName == stat.statName))
            {
                Debug.LogWarning("Stat with same name already exists in this characterUI - " + stat.statName + " will still be added. Use GetStat() == null to verify if it exists first.");
            }

            stat.OnStatChanged += NotifyStatChanged;
            _characterStats[category].Add(stat);
        }

        /// <summary>
        /// Remove all stats with the given category and name.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns>True if succeded, false if the stat couldn't be removed or found.</returns>
        public bool Remove(string category, string name)
        {
            if (_characterStats.ContainsKey(category) == false)
            {
                return false;
            }

            var stat = _characterStats[category].FirstOrDefault(o => o.statName == name);
            if (stat != null)
            {
                stat.OnStatChanged -= NotifyStatChanged;
                _characterStats[category].Remove(stat);
                return true;
            }

            return false;
        }


        public void Clear()
        {
            foreach (var cat in _characterStats)
            {
                foreach (var stat in cat.Value)
                {
                    stat.OnStatChanged -= NotifyStatChanged;
                }
            }

            _characterStats.Clear();
        }


        /// <summary>
        /// Clears all values and grabs new values from the data providers.
        /// </summary>
        public void Prepare()
        {
            Clear();

            foreach (var provider in dataProviders)
            {
                provider.Prepare(_characterStats);
            }

            foreach (var cat in _characterStats)
            {
                foreach (var stat in cat.Value)
                {
                    stat.OnStatChanged += NotifyStatChanged;
                }
            }
        }

        public void NotifyStatChanged(IInventoryCharacterStat stat)
        {
            if (OnStatChanged != null)
                OnStatChanged(stat);
        }

        public IEnumerator<KeyValuePair<string, List<IInventoryCharacterStat>>> GetEnumerator()
        {
            return _characterStats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
