using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public partial class InventoryAudioManager
    {
        private static AudioSource[] audioSources;
        private static GameObject audioSourceGameObject;

        private static List<InventoryAudioClip> audioQueue; 


        static InventoryAudioManager()
        {
            audioSources = new AudioSource[16];
            audioQueue = new List<InventoryAudioClip>(audioSources.Length);
            audioSourceGameObject = new GameObject("INVENTORY_AUDIO_SOURCES");
            UnityEngine.Object.DontDestroyOnLoad(audioSourceGameObject);

            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i] = audioSourceGameObject.AddComponent<AudioSource>();
                audioSources[i].outputAudioMixerGroup = InventoryManager.instance.uiAudioMixerGroup;
            }
        }

        public static void Update()
        {
            if (audioQueue.Count > 0)
            {
                var source = GetNextAudioSource();
                var clip = audioQueue[audioQueue.Count - 1];

                source.clip = clip.audioClip;
                source.pitch = clip.pitch;
                source.volume = clip.volume;
                source.loop = clip.loop;
                source.Play();

                audioQueue.RemoveAt(audioQueue.Count - 1);
            }
        }


        private static AudioSource GetNextAudioSource()
        {
            foreach (var audioSource in audioSources)
            {
                if (audioSource.isPlaying == false)
                    return audioSource;
            }

            Debug.LogWarning("All sources taken, can't play audio clip...");
            return null;
        }


        /// <summary>
        /// Plays an audio clip, only use this for the UI, it is not pooled so performance isn't superb.
        /// TODO: Pool this
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        public static void AudioPlayOneShot(InventoryAudioClip clip)
        {
            if (clip == null || clip.audioClip == null)
                return;

            audioQueue.Add(clip);
            audioQueue = audioQueue.GroupBy(o => o.audioClip).Select(o => o.First()).ToList(); // Remove duplicates.
        }
    }
}
