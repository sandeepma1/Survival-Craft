using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public partial class InventoryUtility
    {
        /// <summary>
        /// Plays an audio clip, only use this for the UI, it is not pooled so performance isn't superb.
        /// TODO: Pool this
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        [Obsolete("Use the new InventoryAudioManager.AudioPlayOneShot instead.", true)]
        public static void AudioPlayOneShot(AudioClip clip, float volume = 1.0f)
        {
            Assert.IsNotNull(clip, "AudioClip is null, not allowed.");

            var obj = new GameObject("TEMP_AUDIO_SOURCE_UI");
            var source = obj.AddComponent<AudioSource>();

            source.PlayOneShot(clip, volume);
            UnityEngine.Object.Destroy(obj, clip.length + 0.1f);
        }

        public static int FindIndex<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return retVal;
                }

                retVal++;
            }

            return -1;
        }

        public static FieldInfo FindFieldInherited(System.Type startType, string fieldName)
        {
            if (startType == typeof(UnityEngine.MonoBehaviour) || startType == null)
                return null;

            // Copied fields can be restricted with BindingFlags
            var field = startType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;

            // Keep going untill we hit UnityEngine.MonoBehaviour type.
            return FindFieldInherited(startType.BaseType, fieldName);
        }

        public static void GetAllFieldsInherited(System.Type startType, List<FieldInfo> appendList)
        {
            if (startType == typeof(MonoBehaviour) || startType == null || startType == typeof(object))
                return;

            // Copied fields can be restricted with BindingFlags
            var fields = startType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields)
            {
                if (appendList.Any(o => o.Name == fieldInfo.Name) == false)
                {
                    appendList.Add(fieldInfo);
                }
            }

            // Keep going untill we hit UnityEngine.MonoBehaviour type or null.
            GetAllFieldsInherited(startType.BaseType, appendList);
        }

        public static void SetLayerRecursive(GameObject obj, int layer)
        {
            Assert.IsNotNull(obj, "Cannot set layers, gameObject given is null! (or object was destroyed while setting layers)");

            obj.layer = layer;
            foreach (Transform t in obj.transform)
            {
                SetLayerRecursive(t.gameObject, layer);
            }
        }

        public static void ResetTransform(Transform transform)
        {
            Assert.IsNotNull(transform, "Transform given is null");

            transform.localPosition = Vector3.zero;
            var rectTransform = transform.gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Regular GameObject find doesn't handle in-active objects...
        /// </summary>
        /// <param name="startObject"></param>
        /// <param name="path"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static void FindChildTransform(Transform startObject, string path, ref Transform result)
        {
            // Early bailing if it's already found
            if (result != null)
            {
                return;
            }

            var p = path.Split('/');
            Assert.IsTrue(p.Length > 0, "Not a valid path given...");

            foreach (Transform child in startObject.transform)
            {
                if (child.name == p[p.Length - 1])
                {
                    // Found name of object, check parent names
                    if (p.Length == 1)
                    {
                        result = child;
                        return;
                    }

                    bool isMatch = true;
                    var parent = child.parent;
                    for (int i = p.Length - 2; i >= 0; i--)
                    {
                        if (parent.name != p[i])
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        result = child;
                        return;
                    }
                }

                if (child.transform.childCount > 0)
                {
                    FindChildTransform(child, path, ref result);
                }
            }
        }
    }
}
