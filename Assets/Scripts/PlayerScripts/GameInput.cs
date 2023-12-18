using System;
using GameEvent;
using NicoFramework.Tools.EventCenter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerScripts
{
    public class GameInput : Singleton<GameInput>
    {
        private GameInput() { }

        private PlayerInputAction _playerInputAction;
        

        protected override void Awake()
        {
            base.Awake();

            _playerInputAction = new PlayerInputAction();
            _playerInputAction.Enable();
            
            _playerInputAction.Player.Attack.performed += Attack_performed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _playerInputAction.Player.Attack.performed -= Attack_performed;
            _playerInputAction.Dispose();
        }

        private void Attack_performed(InputAction.CallbackContext obj)
        {
            EventCenter.Default.Publish(new InputEvent.AttackEvent());
        }

        public Vector2 GetMovementNormalized()
        {
            return _playerInputAction.Player.Move.ReadValue<Vector2>();
        }

        public Vector2 GetMousePosInWorldPoint()
        {
            return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
    }
}