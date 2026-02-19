using SF.Characters;
using UnityEngine;

namespace SF.SpawnModule
{
    using AudioModule;
    using Characters.Data;
    using StateMachine.Core;
    using CommandModule;
    
    public class CombatantHealth : CharacterHealth
    {
        private CombatantData _combatantData;
        private StateMachineBrain _combatantStateBrain;

        [SerializeField] protected SpriteBlinkCommand _spriteBlink;
        protected override void Awake()
        {
            base.Awake();
            _combatantData       = GetComponent<CombatantData>();
            _combatantStateBrain = GetComponentInChildren<StateMachineBrain>();
            _currentHealth       = MaxHealth;
        }
        
        protected override void Kill(Vector2 knockback = new Vector2())
        {
            if (_combatantData is not null)
            {
                // TODO: Will need checks later for allies and summonings to not grant experience.
                //  Grant the player his experience from the enemy kill.
                
                if (_combatantData.EnemyLootTable is not null)
                {
                    _combatantData.EnemyLootTable.DropRandomLoot(transform.position);
                }
            }

            if (_controllerBody2D != null)
            {
                _controllerBody2D.CharacterState.CharacterStatus = CharacterStatus.Dead;
                _controllerBody2D.AddVelocity(knockback);
            }
            
            if(_deathSFX != null)
                AudioManager.Instance.PlayOneShot(_deathSFX);
            
            _ = _spriteBlink.Use();
            _ = _deathTimer.StartTimerAsync();
        }

        public override void Respawn()
        {
            base.Respawn();
            
            if(_combatantStateBrain != null)
                _combatantStateBrain.InitStateBrain();
        }

        protected override void OnEnable()
        {
            _deathTimer = new Timer(_deathTimer.Duration,Despawn);
        }
    }
}
