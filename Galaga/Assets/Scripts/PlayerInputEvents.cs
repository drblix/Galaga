using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Galaga
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputEvents : MonoBehaviour
    {
        public static event PlayerActionDelegate<float> OnPlayerMove;
        public static event PlayerActionDelegate OnPlayerShoot;

        public delegate void PlayerActionDelegate<T>(T item);
        public delegate void PlayerActionDelegate();

        private static InputAction[] _inputActions;

        private PlayerInput _playerInput;

        private void Awake()
        {
            if (FindObjectsByType<PlayerInputEvents>(FindObjectsSortMode.None).Length > 1)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += PlayerActionTriggered;

            _inputActions = GetActionsArray();
        }

        private void PlayerActionTriggered(InputAction.CallbackContext context)
        {
            // player move
            if (context.action == _inputActions[0])
            {
                OnPlayerMove.Invoke(context.ReadValue<float>());
            }
            
            // player shoot (should only be triggered when the player finishes pressing the button)
            if (context.action == _inputActions[1])
            {
                if (context.action.phase != InputActionPhase.Performed) return;
                OnPlayerShoot.Invoke();
            }
        }

        private InputAction[] GetActionsArray()
        {
            List<InputAction> actionList = new();

            var maps = _playerInput.actions.actionMaps;
            for (int i = 0; i < maps.Count; i++)
            {
                var inputActions = maps[i].actions;
                for (int j = 0; j < inputActions.Count; j++)
                    actionList.Add(inputActions[j]);
            }

            return actionList.ToArray();
        }
    }
}
