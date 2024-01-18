using Cinemachine;
using DG.Tweening;
using SunkCost.HH.Modules.InputSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private float rotationSpeed = 60f;
        [SerializeField] private float zoomSpeed = 1f;

        private Vector2 _moveInput;
        private bool _rotateClockwiseInput;
        private bool _rotateCounterClockwiseInput;
        private Tween _zoomTween;
        
        private CinemachineTransposer _transposer;

        private void Awake()
        {
            _transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void OnEnable()
        {
            InputManager.instance.onMoveInput.AddListener(m => _moveInput = m);
            InputManager.instance.onRotateClockwiseInput.AddListener(r => _rotateClockwiseInput = r);
            InputManager.instance.onRotateCounterClockwiseInput.AddListener(r => _rotateCounterClockwiseInput = r);
            InputManager.instance.onZoomInput.AddListener(zoomAmount =>
            {
                var targetOffset = new Vector3(
                    _transposer.m_FollowOffset.x,
                    Mathf.Clamp(_transposer.m_FollowOffset.y - zoomAmount * zoomSpeed, 3, 12),
                    Mathf.Clamp(_transposer.m_FollowOffset.z + zoomAmount * zoomSpeed, -12, -3)
                );
                
                _zoomTween = DOTween.To(() => _transposer.m_FollowOffset, x => _transposer.m_FollowOffset = x, targetOffset, 0.5f);
            });
        }

        private void OnDisable()
        {
            InputManager.instance.onMoveInput.RemoveAllListeners();
            InputManager.instance.onRotateClockwiseInput.RemoveAllListeners();
            InputManager.instance.onRotateCounterClockwiseInput.RemoveAllListeners();
            InputManager.instance.onZoomInput.RemoveAllListeners();
        }

        private void Update()
        {
            var right = cameraTarget.right;
            var position = cameraTarget.position;
            var rotation = cameraTarget.rotation;
            var rotationEuler = rotation.eulerAngles;
            
            var moveX = right * (_moveInput.x * movementSpeed);
            var moveY = Vector3.Cross(right, Vector3.up) * (_moveInput.y * movementSpeed);
            var targetPosition = position + moveX + moveY;
            
            cameraTarget.position = Vector3.Lerp(position, targetPosition, Time.deltaTime);

            if (_rotateClockwiseInput)
            {
                var targetRotation = Quaternion.Euler(
                    rotationEuler.x,
                    rotationEuler.y + rotationSpeed * Time.deltaTime,
                    rotationEuler.z
                );
                cameraTarget.rotation = Quaternion.Slerp(rotation, targetRotation, Time.deltaTime);
            }

            if (_rotateCounterClockwiseInput)
            {
                var targetRotation = Quaternion.Euler(
                    rotationEuler.x,
                    rotationEuler.y - rotationSpeed,
                    rotationEuler.z
                );
                cameraTarget.rotation = Quaternion.Slerp(rotation, targetRotation, Time.deltaTime);
            }
        }
    }
}
