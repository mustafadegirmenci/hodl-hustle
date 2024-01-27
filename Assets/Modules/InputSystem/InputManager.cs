using Modules.InputSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SunkCost.HH.Modules.InputSystem
{
    public class InputManager : MonoSingleton<InputManager>, Controls.ICameraActions, Controls.IOfficeActions
    {
        [HideInInspector] public UnityEvent<Vector2> onMoveInput = new();
        [HideInInspector] public UnityEvent<bool> onRotateClockwiseInput = new();
        [HideInInspector] public UnityEvent<bool> onRotateCounterClockwiseInput = new();
        [HideInInspector] public UnityEvent<float> onZoomInput = new();
        [HideInInspector] public UnityEvent onMouseDown = new();
        [HideInInspector] public UnityEvent onMouseUp = new();
        [HideInInspector] public UnityEvent onMouseDelta = new();
        [HideInInspector] public UnityEvent onMouseSecondaryClick = new();
        [HideInInspector] public UnityEvent onRotateItem = new();
        [HideInInspector] public bool mouseDragging = false;

        public Vector2 MousePosition => _controls.Office.MousePosition.ReadValue<Vector2>();
        
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
            
            _controls.Office.SetCallbacks(this);
            _controls.Office.Enable();
        }

        private void OnDisable()
        {
            _controls.Camera.Disable();
            _controls.Office.Disable();
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
        
        public void OnMousePosition(InputAction.CallbackContext context)
        {
            
        }

        #endregion

        public void OnMouseClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                onMouseDown.Invoke();
                mouseDragging = true;
            }
            else if (context.canceled)
            {
                onMouseUp.Invoke();
                mouseDragging = false;
            }
        }

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                onMouseDelta.Invoke();
            }
        }

        public void OnMouseSecondaryClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onMouseSecondaryClick.Invoke();
            }
        }

        public void OnRotateItem(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onRotateItem.Invoke();
            }
        }
    }
}
