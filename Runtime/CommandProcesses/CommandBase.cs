using UnityEngine;

namespace SF.CommandModule
{
    public interface ICommand
    {
        
    }
    public abstract class CommandBase
    {
        public string Name;
        /// <summary>
        /// Should the command be run async or not.
        /// </summary>
        [field:SerializeField] protected bool IsAsyncCommand { get; set; }

        [SerializeField] protected Timer _delayTimer;
        
        public async Awaitable Use()
        {
            if (!CanProcessCommand())
                return;
            
            if (_delayTimer.Duration > 0)
            {
                _delayTimer = new Timer(_delayTimer.Duration, DelayCommand);
                await _delayTimer.StartTimerAsync();
            }
            else
            {
                if (IsAsyncCommand)
                    await ProcessCommandAsync();
                else
                    ProcessCommand();
            }
        }
        
        protected void DelayCommand()
        {
            if (IsAsyncCommand)
                ProcessCommandAsync();
            else
                ProcessCommand();
        }
        
        protected abstract bool CanProcessCommand();
        protected abstract void ProcessCommand();
        protected abstract Awaitable ProcessCommandAsync();
        
    }
}
