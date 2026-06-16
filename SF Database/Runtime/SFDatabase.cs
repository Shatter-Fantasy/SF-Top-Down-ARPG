#if !SF_DATABASES
using System.Collections.Generic;
using UnityEngine;

namespace SF.DataModule
{
    /// <summary>
    /// Wrapper class for SFDatabases to make it easier to register different database types.
    /// <see cref="DatabaseRegistry"/> for example uses.
    /// </summary>
    public abstract class SFDatabase : ScriptableObject
    {
        /// <summary>
        /// The order of important in which databases are loaded. Allows making sure certain databases are loaded first.
        /// The lower the number the earlier it is loaded.
        /// </summary>
        public int DatabaseLoadOrder;
        
        /// <summary>
        /// SFDatabases are registered by the <see cref="DatabaseRegistry"/> which is loaded during the player start up
        /// as part of the preloaded assets set in the project's PlayerSettings via SetPreloadedAssets.
        /// So there OnEnable runs when the runtime player starts and is guaranteed to run before anything scene related.
        /// </summary>
        public virtual void OnRegisterDatabase() { }

        public virtual void OnDeregisterDatabase(){ }
        
    }
    
    /// <summary>
    /// A generic database class for storing data about DTOAssetBase Scriptable objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SFAssetDatabase<T> : SFDatabase where T : DTOAssetBase
    {
        [SerializeReference] public List<T> DataEntries = new List<T>();

        public virtual void AddData(T dataEntry)
        {
            if(dataEntry == null)
                return;
            
            DataEntries.Add(dataEntry);
            dataEntry.BaseData.ID = DataEntries.Count - 1;
        }

        public void RemoveData(T dataEntry)
        {
            if(dataEntry == null)
                return;

            DataEntries.Remove(dataEntry);
        }

        public T GetDataByID(int characterId)
        {
            return DataEntries.Find((T data) => data.ID == characterId);
        }
        
        public bool GetDataByID(int characterId, out T data)
        {
            var dataFound = DataEntries.Find((T data) => data.ID == characterId);
            data = dataFound;
            return data != null;
        }

        public int GetDataIndexInDB(T dtoAsset)
        {
            return DataEntries.IndexOf(dtoAsset);
        }

        public T this[int itemId]
        {
            get
            {
                return DataEntries.Find( data => data.ID == itemId);
            }
        }
        
#if UNITY_EDITOR
        protected void SetDataIdsByListIndex<TDatabaseType>() where TDatabaseType : SFAssetDatabase<T> 
        {
            var databaseRegistry = DatabaseRegistry.Registry;
            
            if (databaseRegistry == null)
                return;

            if (!DatabaseRegistry.TryGetDatabase(out TDatabaseType foundDatabase))
                return;

            for (int i = 0; i < foundDatabase.DataEntries.Count; i++)
            {
                foundDatabase.DataEntries[i].ID = i;
            }
        }
        
        protected virtual void SetCommonDataFields<TDatabaseType>() where TDatabaseType : SFAssetDatabase<T> 
        {
            
        }
#endif
    }
    
    /// <summary>
    /// A generic database class for storing data about DTOBase classes or sub classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SFDatabase<TDTOBase> : SFDatabase where TDTOBase : DTOBase
    {
        [SerializeReference] public List<TDTOBase> DataEntries = new List<TDTOBase>();

        public virtual void AddData(TDTOBase dataEntry)
        {
            if(dataEntry == null)
                return;

            DataEntries.Add(dataEntry);
        }

        public void RemoveData(TDTOBase dataEntry)
        {
            if(dataEntry == null)
                return;

            DataEntries.Remove(dataEntry);
        }
        
        public TDTOBase this[int index]
        {
            get
            {
                return DataEntries[index];
            }
        }
    }
}
#endif