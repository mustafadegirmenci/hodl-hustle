using Modules.InputSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SunkCost.HH.Modules.InputSystem
{
    public class InputManager : MonoSingleton<InputManager>, Controls.ICameraActions
    {
        [HideInInspector] public UnityEvent<Vector2> onMoveInput = new();
        [HideInInspector] public UnityEvent<bool> onRotateClockwiseInput = new();
        [HideInInspector] public UnityEvent<bool> onRotateCounterClockwiseInput = new();
        [HideInInspector] public UnityEvent<float> onZoomInput = new();
        
        private Controls _controls;

        #region MonoBehaviour Methods
        
        private void OnEnable()
        {
            if (_controls != null)
            {
                return;
            }

            _controls = new Controls();
            _controls.Camera.SetCallbacks(this);
            _controls.Camera.Enable();
        }

        private void OnDisable()
        {
            _controls.Camera.Disable();
        }
        
        #endregion

        #region Callback Methods
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                onMoveInput.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnRotateClockwise(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onRotateClockwiseInput.Invoke(true);
            }
            else if (context.canceled)
            {
                onRotateClockwiseInput.Invoke(false);
            }
        }

        public void OnRotateCounterClockwise(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onRotateCounterClockwiseInput.Invoke(true);
            }
            else if (context.canceled)
            {
                onRotateCounterClockwiseInput.Invoke(false);
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onZoomInput.Invoke(context.ReadValue<float>());
            }
        }

        #endregion
    }
}
