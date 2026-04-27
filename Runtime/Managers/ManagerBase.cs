using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SF
{
    public abstract class ManagerBase<T> : MonoBehaviour
    {
        [NoAutoStaticsCleanup]
        protected static T _instance;

        public static T  Manager
        {
            get => _instance;
            set => _instance = value;
        }
        
    }
    
    public abstract partial class ManagerBaseStaticCleanUp<T> : MonoBehaviour
    {
        [AutoStaticsCleanup]
        protected static T _instance;

        public static T  Manager
        {
            get => _instance;
            set => _instance = value;
        }
    }
}
