using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SF.Managers
{
    using SF.DataManagement;
    using SF.InputModule;
    using SF.Settings;
    
	/// <summary>
	/// The current state that is controlling the games input and actions. 
	/// </summary>
	public enum GameControlState
	{
		Player,
		SceneChanging,
		Cutscenes,
		Transition, // Player being moved within a scene, but has no control over the player. Think teleporting.
        Dialogue,
        Menu,
	}
	/// <summary>
	/// The current play state of the game loop that describes what type of logic loop is being updated.
	/// </summary>

    [AutoStaticsCleanup]
    [DefaultExecutionOrder(-5)]
    public partial class GameManager : ManagerBaseStaticCleanUp<GameManager>
    {
        [SerializeReference]
        public List<SaveDataBlock> SaveDataBlocks = new List<SaveDataBlock> ();

        [SerializeField] private GameControlState _controlState;
        
        public GameControlState ControlState
        {
            get { return _controlState;}
            set
            {
                if (_controlState != value)
                {
                    _controlState = value;
                    OnGameControlStateChanged?.Invoke(_controlState);
                }
            }
        }

        public Action<GameControlState> OnGameControlStateChanged;

        public static event Action GamePausedHandler;
        public static event Action GameUnpausedHandler;

        /// <summary>
        /// The data object that holds the settings players can change in game.
        /// </summary>
        public GameSettings GameSettings;
        
        private void Awake()
        {
            if(GameSettings != null)
                GameSettings.ProcessSettings();

            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(this);
            } // We want to destroy the child object managers so they are not doubles as well.
        }

        protected void OnEnable()
        {
            if (SFInputManager.Instance != null)
            {
                SFInputManager.Controls.GameControl.Enable();
                SFInputManager.Controls.GameControl.ExitGame.performed += OnExitGame;
            }
            //DialogueManager.DialogueStartedHandler += OnDialogueStarted;
            //DialogueManager.DialogueEndedHandler += OnDialogueEnded;
        }

        protected void OnDisable ()
        {
            if (SFInputManager.Instance != null)
                SFInputManager.Controls.GameControl.ExitGame.performed -= OnExitGame;
            
            //DialogueManager.DialogueStartedHandler -= OnDialogueStarted;
            //DialogueManager.DialogueEndedHandler -= OnDialogueEnded;
        }
        
        private void OnExitGame(InputAction.CallbackContext obj)
        {
            ExitGame();
        }

        /// <summary>
        /// Exits the game and closes all related computer processes.
        /// </summary>
        public static void ExitGame()
        {
            // Will need to do checks later for preventing shutdowns during saving and loading.
            Application.Quit();
        }

        public static void OnPausedToggle()
        {
            if(Instance._controlState == GameControlState.Player)
                Pause();
            else // So we are already paused or in another menu.
                Unpause();
        }

        protected static void Pause()
        {
            Instance.ControlState = GameControlState.Menu;
            GamePausedHandler?.Invoke();
        }

        public static void Unpause()
        {
            Instance.ControlState = GameControlState.Player;
            GameUnpausedHandler?.Invoke();
        }
        
        private void OnDialogueStarted()
        {
            /* TODO: Switch statement for type of dialogue.
            * Allow for background dialogue that don't freeze the player control. */
            
            _controlState = GameControlState.Dialogue;
        }
        
        private void OnDialogueEnded()
        {
            /* TODO: Switch statement for type of dialogue.
             * Allow for background dialogue that don't freeze the player control. */
            
            _controlState = GameControlState.Player;
        }
    }
}