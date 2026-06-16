#if !SF_DATABASES
using UnityEngine;

namespace SF.DataModule
{

    /// <summary>
    /// This is a DTOBase that can be used for normal class object where needed.
    /// </summary>
    [System.Serializable]
    public class DTOBase
    {
        public int ID = 0;
    }
    
    /// <summary>
    /// This is a DTOBase that can be used as a scriptable object where needed.
    /// </summary>
    public class DTOAssetBase : ScriptableObject
    {
        // Temp hiding this till I am ready for full implementation.
        [HideInInspector] public DTOBase BaseData;
        
        // TODO: Replace the below with either ItemData or Item
        //  I might replace the use of IDs with EntityID since each scriptable object now has an assigned EntityID built in.
        public int ID = 0;
        public string Name;
        public string Description;

        public DTOAssetBase() 
        {
           
        }
    }
}
#endif