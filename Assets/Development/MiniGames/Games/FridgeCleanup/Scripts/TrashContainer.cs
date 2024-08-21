using UnityEngine;

namespace InteractionSystem.TrashContainer
{
    public class TrashContainer : MonoBehaviour
    {
        public TrashContainerType trashContainerType;

        public enum TrashContainerType
        {
            Residual,
            Paper,
            YellowSack
        }
    }
}
