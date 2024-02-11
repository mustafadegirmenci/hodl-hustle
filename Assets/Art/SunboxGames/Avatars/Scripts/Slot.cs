
using UnityEngine;

namespace SunkCost.HH.Art.SunboxGames.Avatars.Scripts {

    public class Slot : MonoBehaviour {
        public SlotType SlotType;
        public AttachmentType AttachmentType;
        public Transform BoneTransform;

        void Awake() {
            BoneTransform = GetComponent<Transform>();
        }
        
    }

}