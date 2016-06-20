using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/player-spawner/")]
    [AddComponentMenu("InventorySystem/Player/Inventory Player spawner")]
    public class InventoryPlayerSpawner : InventoryPlayerBase
    {

        [InventoryRequired]
        public GameObject playerPrefab;

        public bool spawnOnAwake = true;
        public bool dontDestroyPlayerOnLoad = true;

        /// <summary>
        /// When a character is already found in the scene, should spawning be aborted?
        /// </summary>
        public bool forceSingleCharacter = true;



        protected override void Awake()
        {
            base.Awake();
            
            if (spawnOnAwake)
            {
                Spawn();
            }
        }

        public virtual void Spawn()
        {
            var foundPlayer = FindObjectOfType<InventoryPlayer>();
            if (forceSingleCharacter && foundPlayer != null)
            {
//                Debug.Log("Inventory Pro player already found in scene, enforcing singel player.");
                if (foundPlayer.isInitialized == false)
                {
                    foundPlayer.Init();
                }
                return;
            }

            var playerObj = Object.Instantiate<GameObject>(playerPrefab);
            var player = playerObj.GetComponentInChildren<InventoryPlayer>();
            if (dontDestroyPlayerOnLoad)
            {
                DontDestroyOnLoad(playerObj);
            }

            player.transform.root.gameObject.name = playerPrefab.name;
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            player.characterCollection = this.characterCollection;
            player.inventoryCollections = this.inventoryCollections;
            player.skillbarCollection = this.skillbarCollection;
            player.equipLocations = this.equipLocations;

            player.Init();

            transform.DetachChildren();
            Destroy(gameObject); // No longer need spawner.
        }
    }
}
