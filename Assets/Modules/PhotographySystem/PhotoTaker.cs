using System.Threading.Tasks;
using SunkCost.HH.Modules.Utility;
using UnityEngine;

namespace SunkCost.HH.Modules.PhotographySystem
{
    public class PhotoTaker : MonoSingleton<PhotoTaker>
    {
        [SerializeField] private Camera photographyCamera;
        [SerializeField] private string photographySubjectLayerName;

        public async Task<Texture2D> TakePhoto(
            GameObject subject,
            PhotoDirection from = PhotoDirection.Front,
            float width = 500,
            float height = 500,
            float distance = -1,
            Vector3 cameraOffset = default,
            Vector3 lookAtOffset = default
            )
        {
            var renderTexture = PrepareRenderTexture(width, height);
            var subjectInstance = InstantiateSubject(subject);
            
            SetSubjectLayers(subjectInstance);
            
            var bounds = CalculateBounds(subjectInstance);
            
            SetCameraPositionAndRotation(bounds, from, distance, cameraOffset, lookAtOffset);
            
            var photoRenderTexture = CapturePhoto(renderTexture);
            
            DestroyAndCleanup(subjectInstance);

            return ConvertRenderTextureToTexture2D(photoRenderTexture, width, height);
        }

        private RenderTexture PrepareRenderTexture(float width, float height)
        {
            var renderTexture = new RenderTexture((int)width, (int)height, 24);
            photographyCamera.targetTexture = renderTexture;
            return renderTexture;
        }

        private GameObject InstantiateSubject(GameObject subjectPrefab)
        {
            var subjectInstance = Instantiate(subjectPrefab, Vector3.zero, Quaternion.identity);
            subjectInstance.SetActive(true);
            
            return subjectInstance;
        }

        private void SetSubjectLayers(GameObject subjectInstance)
        {
            var children = subjectInstance.GetComponentsInChildren<Renderer>(true);
            foreach (var child in children) child.gameObject.layer = LayerMask.NameToLayer(photographySubjectLayerName);
        }

        private void SetCameraPositionAndRotation(
            Bounds bounds, 
            PhotoDirection from, 
            float distance, 
            Vector3 offset,
            Vector3 lookAtOffset)
        {
            var calculatedDistance = (distance < 0) ? Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.5f : distance;

            photographyCamera.transform.position = CalculateCameraPosition(bounds, from, calculatedDistance, offset);
            photographyCamera.transform.LookAt(bounds.center + lookAtOffset);
        }

        private Vector3 CalculateCameraPosition(Bounds bounds, PhotoDirection from, float distance, Vector3 offset)
        {
            var position = bounds.center;

            if (from.HasFlag(PhotoDirection.Front)) position += Vector3.forward * (bounds.size.z * distance);
            if (from.HasFlag(PhotoDirection.Back)) position -= Vector3.forward * (bounds.size.z * distance);
            if (from.HasFlag(PhotoDirection.Left)) position -= Vector3.right * (bounds.size.x * distance);
            if (from.HasFlag(PhotoDirection.Right)) position += Vector3.right * (bounds.size.x * distance);

            position += offset;

            return position;
        }

        private RenderTexture CapturePhoto(RenderTexture renderTexture)
        {
            photographyCamera.gameObject.SetActive(true);
            photographyCamera.Render();
            photographyCamera.gameObject.SetActive(false);

            RenderTexture.active = renderTexture;
            return renderTexture;
        }

        private void DestroyAndCleanup(GameObject subjectInstance)
        {
            Destroy(subjectInstance);
            photographyCamera.targetTexture = null;
        }

        private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture, float width, float height)
        {
            var photoTexture = new Texture2D((int)width, (int)height);
            photoTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            photoTexture.Apply();
            RenderTexture.active = null;

            return photoTexture;
        }

        private Bounds CalculateBounds(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            var bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();

            foreach (var ren in renderers)
            {
                bounds.Encapsulate(ren.bounds);
            }

            return bounds;
        }
    }
}
