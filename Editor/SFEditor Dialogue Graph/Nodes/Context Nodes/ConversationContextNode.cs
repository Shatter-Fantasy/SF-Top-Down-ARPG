using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
	using SFEditor.Nodes;
	using SF.DialogueModule;
	using SF.DialogueModule.Nodes;
	[Serializable]
	[UseWithGraph(typeof(DialogueGraph))]
	public class ConversationContextNode : ContextNode, 
		IDialogueNode, 
		IContextNodeConvertor
	{
		public string ExecutionPortName { get; } = "Dialogue Entry";
		
		public const string ConversationTitleName = "Conversation Name";
		
		public List<IRuntimeNode> RuntimeNodes = new ();
		/// <summary>
		/// Conversation that is only set and used during the <see cref="DialogueGraphImporter"/> processing. 
		/// </summary>
		[NonSerialized] public DialogueConversation Conversation;

		public IRuntimeNode ExecutionNode;
		
		protected override void OnDefineOptions(IOptionDefinitionContext  context)
		{ 
			context.AddOption<string>(ConversationTitleName);
		}
	    
		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<string>("Input Node").Build();
			context.AddOutputPort<string>(ExecutionPortName).Build();
		}

		public IRuntimeNode ConvertToRuntimeNode()
		{
			RuntimeNodes = ConvertToRuntimeNodes(Conversation);
			
			var executionNode = GetOutputPortByName(ExecutionPortName).FirstConnectedPort.GetNode();
			
			if (executionNode is INodeConvertor nodeConvertor)
			{
				return new ConversationRuntimeNode(RuntimeNodes,nodeConvertor.ConvertToRuntimeNode())
				{
					ShouldPauseGraphProcessing = true
				};
			}
			
			return new ConversationRuntimeNode(RuntimeNodes)
			{
				ShouldPauseGraphProcessing = true
			};
		}

		public List<IRuntimeNode> ConvertToRuntimeNodes(DialogueConversation dialogueConversation)
		{
			if (dialogueConversation != null)
			{
				GetNodeOptionByName(ConversationTitleName)
					.TryGetValue(out dialogueConversation.ConversationName);
			}
			
			RuntimeNodes.Clear();
			for(int i = 0; i < BlockCount; i++) 
			{
				var conversationNode = GetBlock(i);
						    
				if (conversationNode is not INodeConvertor convertor)
					return null;
				
				var convertedNode = convertor.ConvertToRuntimeNode();
						    
				if(convertedNode == null)
					continue;
						    
				// The below should be a switch statement after testing is done.
				if (convertedNode is ConversationEntryRuntimeNode entryNode)
				{
					RuntimeNodes.Add(entryNode);
				}
				else
				{
					RuntimeNodes.Add(convertedNode);
				}
			}
			
			
			
			
			return RuntimeNodes;
		}
	}
}
