using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.UiSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomUiHandler : MonoBehaviour
    {
        [SerializeField] private Canvas roomCanvas;
        [SerializeField] private Room room;
        [SerializeField] private SC_Button startEditingRoomButton;
        [SerializeField] private LineRenderer lineRenderer;

        private Transform _camTransform;

        private void Awake()
        {
            _camTransform = Camera.main!.transform;
        }

        private void Start()
        {
            startEditingRoomButton.onClick.AddListener(HandleStartEditingClicked);
            room.onRoomChanged.AddListener(UpdateCanvasPosition);
            room.onEditStarted.AddListener(() => startEditingRoomButton.gameObject.SetActive(false));
            room.onEditFinished.AddListener(() => startEditingRoomButton.gameObject.SetActive(true));
            InitializeLine();
            roomCanvas.gameObject.SetActive(false);
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
            const float scaleFactor = 0.1f;

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

                roomCanvas.GetComponent<RectTransform>().position = averagePosition + new Vector3(0, 3, 0);
                
                DrawDashedLine(startEditingRoomButton.transform.position, averagePosition);
            }
        }
        
        private void DrawDashedLine(Vector3 start, Vector3 end, float dashLength = 0.1f)
        {
            var lineLength = Vector3.Distance(start, end);
            var segments = Mathf.CeilToInt(lineLength / (2 * dashLength));
            lineRenderer.positionCount = segments * 2;

            var step = (end - start) / segments;

            for (var i = 0; i < segments; i++)
            {
                var dashStart = start + i * step;
                var dashEnd = start + (i + 0.5f) * step;
                lineRenderer.SetPosition(i * 2, dashStart);
                lineRenderer.SetPosition(i * 2 + 1, dashEnd);
            }
        }
    }
}
