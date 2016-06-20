#if PLY_GAME

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Integration.plyGame.plyBlox;
using Devdog.InventorySystem.Models;
using plyBloxKit;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    [AddComponentMenu("InventorySystem/Integration/plyGame/ply Inventory player")]
    public partial class plyInventoryPlayer : InventoryPlayer
    {
        protected virtual List<ActorAttribute> plyAttributes
        {
            get
            {
                if (InventoryPlayerManager.instance.currentPlayer == null)
                    return new List<ActorAttribute>();

                var actor = InventoryPlayerManager.instance.currentPlayer.GetComponent<Actor>();
                if (actor == null || actor.actorClass == null)
                    return new List<ActorAttribute>();

                return actor.actorClass.attributes;
            }
        }

        private Actor _actor;
        public Actor actor
        {
            get
            {
                if (_actor == null)
                    _actor = gameObject.GetComponent<Actor>();

                return _actor;
            }
        }



        protected override void Awake()
        {
            base.Awake();

            // Pass the data to plyBlox
            gameObject.AddComponent<InventoriesCollectionEventsProxy>();
            gameObject.AddComponent<CharactersCollectionEventsProxy>();
            gameObject.AddComponent<VendorCollectionEventsProxy>();
            gameObject.AddComponent<CraftingCollectionEventsProxy>();

            StartCoroutine(RegisterPlyAttributeListeners());
        }

        private IEnumerator RegisterPlyAttributeListeners()
        {
            yield return null; // Wait for plyGame to initialize.
            yield return null;

            var attributes = this.plyAttributes;
            foreach (var attr in attributes)
            {
                var a = actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == attr.id.Value);
                if (a != null)
                {
                    a.RegisterChangeListener(AttributeChangeCallback);
                }
            }
        }

        private void AttributeChangeCallback(object sender, object[] args)
        {
            var attr = (ActorAttribute) sender;
            var invProAttr = ItemManager.database.plyAttributes.FirstOrDefault(o => o.ID == attr.id);

            var player = InventoryPlayerManager.instance.currentPlayer;
            if (player != null && player.characterCollection != null && invProAttr != null)
            {
                var stat = player.characterCollection.stats.Get(invProAttr.category, attr.def.screenName);
                if (stat != null)
                {
                    var equippable = attr.lastInfluence.GetComponent<plyGameEquippableInventoryItem>();
                    var consumable = attr.lastInfluence.GetComponent<plyGameConsumableInventoryItem>();
                    if (equippable != null)
                    {
//                        var plyAttr = equippable.plyAttributes.First(o => o.id.Value == attr.id.Value);
                        stat.ChangeCurrentValueRaw(equippable.lastAddedUseValue);
//                        stat.SetMaxValueRaw(plyAttr.toAdd, true);
                    }
                    else if (consumable != null)
                    {
//                        var plyAttr = consumable.plyAttributes.First(o => o.id.Value == attr.id.Value);
                        stat.ChangeCurrentValueRaw(consumable.lastAddedUseValue);
                    }
                }
            }
        }
    }
}

#endif