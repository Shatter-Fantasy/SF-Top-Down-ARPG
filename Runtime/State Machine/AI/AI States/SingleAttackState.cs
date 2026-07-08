using UnityEngine;

namespace SF.StateMachine
{
    using Core;
    using U2D.Physics;
    using Weapons;
    
    public class SingleAttackState : StateCore
    {

        [SerializeField, SerializeReference] private WeaponBase _weapon;
        
        protected override void OnInit(TopdownControllerBody2D controllerBody2D)
        {
            base.OnInit(controllerBody2D);

            if (_weapon == null)
                _weapon = GetComponent<WeaponBase>();

            if (_weapon != null)
                _weapon.UseCompleted += OnUseCompleted;
        }

        protected override void OnStateEnter()
        {
            if (_weapon == null)
                return;
            
            _weapon.Use();
            _controllerBody2D.FreezeController(true);
        }

        protected void OnUseCompleted()
        {
            _controllerBody2D.UnfreezeController();
            StateBrain.ChangeStateWithCheck(StateBrain.PreviousState);
        }
    }
}
