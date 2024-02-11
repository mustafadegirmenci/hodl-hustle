using SunkCost.HH.Modules.Utility;
using UnityEngine;

namespace SunkCost.HH.Modules.PhotographySystem
{
    public class PhotoConverter : MonoSingleton<PhotoConverter>
    {
        public Sprite Texture2DToSprite(Texture2D texture2D)
        {
            var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);

            return sprite;
        }
    }
}