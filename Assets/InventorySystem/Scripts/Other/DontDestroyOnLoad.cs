using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}