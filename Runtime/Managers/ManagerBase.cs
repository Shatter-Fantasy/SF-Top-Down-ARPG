using UnityEngine;

namespace SF
{
    public abstract class ManagerBase<T> : MonoBehaviour
    {
        protected static T _instance;

        public static T  Manager
        {
            get => _instance;
            set => _instance = value;
        }
        
    }
}
