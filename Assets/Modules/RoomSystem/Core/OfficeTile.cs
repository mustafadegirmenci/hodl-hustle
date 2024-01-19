using JetBrains.Annotations;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem.Core
{
    public class OfficeTile : MonoBehaviour
    {
        [CanBeNull] public Room occupantRoom;
        
        [CanBeNull] public OfficeTileSection frontWall;
        [CanBeNull] public OfficeTileSection backWall;
        [CanBeNull] public OfficeTileSection leftWall;
        [CanBeNull] public OfficeTileSection rightWall;
        [CanBeNull] public OfficeTileSection frontWallDecoration;
        [CanBeNull] public OfficeTileSection backWallDecoration;
        [CanBeNull] public OfficeTileSection leftWallDecoration;
        [CanBeNull] public OfficeTileSection rightWallDecoration;
        [CanBeNull] public OfficeTileSection floor;
        [CanBeNull] public OfficeTileSection floorDecoration;
        
        [HideInInspector] public float x;
        [HideInInspector] public float z;

        public void SelectForConstruction()
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        public void DeselectForConstruction()
        {
            GetComponent<Renderer>().material.color = Color.gray;
        }
    }
}
