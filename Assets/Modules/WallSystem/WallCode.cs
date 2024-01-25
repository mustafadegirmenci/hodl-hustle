using System;

namespace SunkCost.HH.Modules.WallSystem
{
    [Flags]
    public enum WallCode
    {
        None = 0,
        NearLeft = 1,
        NearRight = 2,
        FarLeft = 4,
        FarRight = 8
    }
}
