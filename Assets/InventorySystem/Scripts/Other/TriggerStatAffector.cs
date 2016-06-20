using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;


namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Actions/Trigger Stat Affector")]
    public class TriggerStatAffector : MonoBehaviour
    {
        public delegate void Change(InventoryPlayer player, IInventoryCharacterStat stat);
        public event Change OnEnter;
        public event Change OnStay;
        public event Change OnExit;


        [Header("Stat")]
        public InventoryItemPropertyReference property;

        [Header("Enter")]
        public bool changeOnEnter = true;
        public float enterChangeAmount = 10f;
        
        [Header("Stay")]
        public bool changeOnStay = true;
        public float onStayChangeInterval = 1.0f; // Deal damage every N seconds
        public float onStayChangeAmount = 2f;

        [Header("Exit")]
        public bool changeOnExit = true;
        public float onExitChangeAmount = 2f;
        
        [Header("Audio & Visuals")]
        [SerializeField]
        private InventoryAudioClip _audioClipOnDamage;

        [SerializeField]
        private GameObject _particleEffect;

        [SerializeField]
        private Vector3 _particleOffset;
        

        private Collider _inTrigger;
        private float _timer;

        protected void Update()
        {
            if (_inTrigger == null || changeOnStay == false)
                return;


            _timer += Time.deltaTime;
            if (_timer >= onStayChangeInterval)
            {
                _timer = 0f;
                var player = InventoryPlayerManager.instance.currentPlayer;

                if (player.characterCollection != null)
                {
                    ChangeStat(player, onStayChangeAmount);

                    if (OnStay != null)
                    {
                        var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
                        OnStay(player, stat);
                    }
                }
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (changeOnEnter == false)
                return;

            var player = InventoryPlayerManager.instance.currentPlayer;
            if (other.gameObject == player.gameObject)
            {
                _inTrigger = other;
                
                if (player.characterCollection != null)
                {
                    ChangeStat(player, enterChangeAmount);

                    if (OnEnter != null)
                    {
                        var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
                        OnEnter(player, stat);
                    }
                }               
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var player = InventoryPlayerManager.instance.currentPlayer;
            if (other.gameObject == player.gameObject)
            {
                _inTrigger = null;
                _timer = 0f;

                if (player.characterCollection != null)
                {
                    if (changeOnExit)
                    {
                        ChangeStat(player, onExitChangeAmount);
                    }

                    if (OnExit != null)
                    {
                        var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
                        OnExit(player, stat);
                    }
                }
            }
        }

        private void ChangeStat(InventoryPlayer player, float amount)
        {
            var stat = player.characterCollection.stats.Get(property.property.category, property.property.name);
            stat.ChangeCurrentValueRaw(amount);

            InventoryAudioManager.AudioPlayOneShot(_audioClipOnDamage);

            if (_particleEffect != null)
            {
                var particles = Instantiate(_particleEffect);
                particles.transform.position = player.transform.position + _particleOffset;

                Destroy(particles.gameObject, 1.0f);
            }
        }
    }
}