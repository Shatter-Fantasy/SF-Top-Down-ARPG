using Unity.Scripting.LifecycleManagement;
using UnityEngine;

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
    
    public abstract partial class ManagerBaseStaticCleanUp<T> : MonoBehaviour where T : Object
    {
        [AutoStaticsCleanup]
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<T>();
                
                return _instance;
            } 
            set => _instance = value;
        }
    }
}
