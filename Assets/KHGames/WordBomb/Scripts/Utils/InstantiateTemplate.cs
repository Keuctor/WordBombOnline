using System;
using System.Collections;
using UnityEngine;

namespace ilasm.WordBomb
{
    [Serializable]
    public class InstantiateTemplate<T> where T : MonoBehaviour
    {
        public T Template;
        public Transform Container;

        public T Instantiate()
        {
            return GameObject.Instantiate<T>(Template, Container);
        }
    }
}