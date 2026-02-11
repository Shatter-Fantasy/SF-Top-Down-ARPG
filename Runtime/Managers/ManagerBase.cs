using UnityEngine;

namespace SF
{
    public abstract class ManagerBase<T> : MonoBehaviour
    {
        protected static T _manager;

        public static T  Manager
        {
            get => _manager;
            set => _manager = value;
        }
    }
}
