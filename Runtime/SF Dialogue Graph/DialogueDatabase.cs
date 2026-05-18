using UnityEngine;

namespace SF.DialogueModule
{
    using SF.DataModule;
    [CreateAssetMenu(fileName = "Dialogue Database", menuName = "SF/Dialogue System/Dialogue Database")]
    public class DialogueDatabase : SFAssetDatabase<DialogueConversation>
    {
        public static DialogueDatabase ActiveDialogueDatabase;
        
        public bool GetConversation(int guid, out DialogueConversation conversation)
        {
            conversation = DataEntries.Find(x => x.GUID == guid);

            return conversation != null;
        }
        
        public override void OnRegisterDatabase()
        {
            ActiveDialogueDatabase = this;
        }

        public override void OnDeregisterDatabase()
        {
            // ActiveDialogueDatabase should always == this, but for safety adding the check.
            if(ActiveDialogueDatabase == this)
                ActiveDialogueDatabase = null; 
        }
        
        
#if UNITY_EDITOR
        [ContextMenu("Set Data Ids")]
        private void ResetIds()
        {
            SetDataIdsByListIndex<DialogueDatabase>();
        }
#endif
    }
}
