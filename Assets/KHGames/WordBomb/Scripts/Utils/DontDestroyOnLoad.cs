using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ilasm.WordBomb.Utils
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
