using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.DecorationSystem;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.UiSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomUiHandler : MonoBehaviour
    {
        [SerializeField] private Canvas roomCanvas;
        [SerializeField] private Room room;
        [SerializeField] private SC_Button editRoomButton;
        [SerializeField] private SC_Button decorateRoomButton;
        [SerializeField] private LineRenderer lineRenderer;

        private Transform _camTransform;

        private void Awake()
        {
            _camTransform = Camera.main!.transform;
        }

        private void Start()
        {
            editRoomButton.onClick.AddListener(HandleStartEditingClicked);
            room.onRoomChanged.AddListener(UpdateCanvasPosition);
            room.onEditStarted.AddListener(() =>
            {
                roomCanvas.gameObject.SetActive(false);
                lineRenderer.enabled = false;
            });
            room.onEditFinished.AddListener(() =>
            {
                roomCanvas.gameObject.SetActive(true);
                lineRenderer.enabled = true;
            });
            decorateRoomButton.onClick.AddListener(() =>
            {
                DecorationUiHandler.instance.ShowDecorationItemsList();
            });
            InitializeLine();
            
            roomCanvas.gameObject.SetActive(false);
            lineRenderer.enabled = false;
        }

        private void InitializeLine()
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.useWorldSpace = true;
        }

        private void HandleStartEditingClicked()
        {
            room.StartEditing();
        }

        private void Update()
        {
            LookAtCamera();
            ScaleCanvasBasedOnDistance();
        }

        private void LookAtCamera()
        {
            var rotation = _camTransform.transform.rotation;
            roomCanvas.transform.LookAt(roomCanvas.transform.position + rotation * Vector3.forward,
                rotation * Vector3.up);
        }

        private void ScaleCanvasBasedOnDistance()
        {
            const float scaleFactor = 0.03f;

            var distance = Vector3.Distance(roomCanvas.transform.position, _camTransform.position);
            var newScale = distance * scaleFactor;
            roomCanvas.transform.localScale = new Vector3(newScale, newScale, newScale);
            
            lineRenderer.startWidth = newScale / 10;
            lineRenderer.endWidth = newScale / 10;
        }

        private void UpdateCanvasPosition(List<GridTile> tiles)
        {
            var canvasRect = roomCanvas.GetComponent<RectTransform>();

            if (tiles.Count == 0)
            {
                canvasRect.gameObject.SetActive(false);
            }
            else
            {
                canvasRect.gameObject.SetActive(true);
                
                var averagePosition = tiles.Aggregate(Vector3.zero, (current, tile) => current + tile.WorldPosition);
                averagePosition /= tiles.Count;

                roomCanvas.GetComponent<Transform>().position = averagePosition + new Vector3(0, 3, 0);
                
                DrawDashedLine(roomCanvas.transform.position, averagePosition);
            }
        }
        
        private void DrawDashedLine(Vector3 start, Vector3 end, float dashLength = 0.5f)
        {
            Vector3 lineDirection = end - start;
            float lineLength = lineDirection.magnitude;
            lineDirection.Normalize();

            float dashCount = Mathf.Floor(lineLength / (dashLength + dashLength));
            lineRenderer.positionCount = (int)dashCount * 2;

            for (int i = 0; i < dashCount; i++)
            {
                Vector3 dashStart = start + lineDirection * (i * (dashLength + dashLength));
                Vector3 dashEnd = start + lineDirection * ((i + 1) * dashLength + i * dashLength);

                lineRenderer.SetPosition(i * 2, dashStart);
                lineRenderer.SetPosition(i * 2 + 1, dashEnd);
            }
        }
    }
}
