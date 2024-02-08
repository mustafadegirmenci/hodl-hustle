using System;

namespace SunkCost.HH.Modules.PhotographySystem
{
    [Flags]
    public enum PhotoDirection
    {
        Front = 1,
        Back = 2,
        Left = 4,
        Right = 8
    }
}