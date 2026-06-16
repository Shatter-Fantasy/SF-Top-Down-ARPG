using System;

namespace SFEditor.Dialogue.Graphs
{
    using SF.DialogueModule;
    [Serializable]
    public class StartDialogueNode : DialogueNode, IDialogueNode
    {
        public override string ExecutionPortName { get; } = "Dialogue Start";
        public const string DialogueDBName = "Dialogue Database";
	    
        protected override void OnDefineOptions(IOptionDefinitionContext  context)
        {
            context.AddOption<DialogueDatabase>(DialogueDBName);
        }
	    
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<string>(ExecutionPortName).Build();
        }
    }
}

