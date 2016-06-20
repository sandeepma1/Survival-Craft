using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public class InventoryStatsPropertiesDataProvider : IInventoryStatDataProvider
    {
        public Dictionary<string, List<IInventoryCharacterStat>> Prepare(Dictionary<string, List<IInventoryCharacterStat>> appendTo)
        {
            // Get the properties
            foreach (var property in ItemManager.database.properties)
            {
                if (property.useInStats == false)
                    continue;

                if (appendTo.ContainsKey(property.category) == false)
                    appendTo.Add(property.category, new List<IInventoryCharacterStat>());

                // Check if it's already in the list
                if (appendTo[property.category].FirstOrDefault(o => o.statName == property.name) != null)
                    continue;

                appendTo[property.category].Add(new InventoryCharacterStat(property));
            }

            return appendTo;
        }
    }
}
