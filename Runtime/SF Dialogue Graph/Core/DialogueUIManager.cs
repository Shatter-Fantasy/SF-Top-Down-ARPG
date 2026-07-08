using UnityEngine;
using UnityEngine.UIElements;

namespace SF.DialogueModule
{
    public class DialogueUIManager : MonoBehaviour
    {
        [SerializeField] private PanelRenderer _dialogueOverlayUXML;
        private VisualElement _dialogueView;
        private Label _dialogueLabel;
        private Label _speakerLabel;

        private DialogueEntry _currentEntry;

        private void OnDialogueUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            _dialogueView  = rootElement.Q<VisualElement>(name: "dialogue__view");
            _dialogueLabel = rootElement.Q<Label>(name: "overlay-dialogue__label");
            _speakerLabel  = rootElement.Q<Label>(name: "dialogue-speaker__label");
        }

        private void OnEnable()
        {
            DialogueManager.DialogueStartedHandler += OnDialogueStarted;
            DialogueManager.DialogueEndedHandler += OnDialogueEnded;
            DialogueManager.DialogueTextChangedHandler += OnTextChanged;
            
            if (_dialogueOverlayUXML != null)
                _dialogueOverlayUXML.RegisterUIReloadCallback(OnDialogueUIReloaded);
        }
        
        private void OnDisable()
        {
            DialogueManager.DialogueStartedHandler -= OnDialogueStarted;
            DialogueManager.DialogueEndedHandler -= OnDialogueEnded;
            DialogueManager.DialogueTextChangedHandler -= OnTextChanged;
            
            if (_dialogueOverlayUXML != null)
                _dialogueOverlayUXML.UnregisterUIReloadCallback(OnDialogueUIReloaded);
        }

        /// <summary>
        /// Updates the UI text. 
        /// </summary>
        /// <param name="dialogueEntry"></param>
        private void OnTextChanged(DialogueEntry dialogueEntry)
        {
            _currentEntry = dialogueEntry;
            _dialogueLabel.text = _currentEntry.Text; 
            _speakerLabel.text = _currentEntry.SpeakerName;
        }
        
        private void OnDialogueStarted()
        {
            _dialogueView.style.visibility = Visibility.Visible;
            _dialogueView.enabledSelf      = true;
            if (_currentEntry != null)
            {
                _dialogueLabel.text = _currentEntry.Text;
                _speakerLabel.text = _currentEntry.SpeakerName;
            }
        }

        private void OnDialogueEnded()
        {
            _dialogueView.style.visibility = Visibility.Hidden;
            _dialogueLabel.text = "";
            _speakerLabel.text = "";
        }
    }
}
