using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.PhotographySystem
{
    public class PhotographyTester : MonoBehaviour
    {
        [SerializeField] private GameObject testObject;
        [SerializeField] private RawImage testImage;

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var photo = await PhotoTaker.instance.TakePhoto(
                    testObject,
                    PhotoDirection.Front | PhotoDirection.Left,
                    250,
                    250,
                    offset: Vector3.up * 0.5f
                );

                testImage.texture = photo;
            }
        }
    }
}