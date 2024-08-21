using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem.Packaging
{
    using Interactable;
    using InteractionSystem.TrashContainer;

    public class Packaging : Interactable
    {
        public TrashContainer.TrashContainerType correctTrashContainerType;

        public int id;

        public AudioClip successAudio;

        public float threshold = 0.1f;

        private Vector3 startPosition;
        private List<TrashContainer> trashContainers;

        protected override void Awake()
        {
            base.Awake();
            startPosition = transform.localPosition;
            trashContainers = new List<TrashContainer>(FindObjectsOfType<TrashContainer>(true));
        }

        public void CheckTrashContainers()
        {
            TrashContainer closestTrashContainer = null;
            float shortestDistance = -1.0f;

            foreach (TrashContainer trashContainer in trashContainers)
            {
                float currentDistance = Vector3.Distance(trashContainer.transform.position, transform.position);
                if (currentDistance <= threshold && (shortestDistance < 0 || shortestDistance > currentDistance))
                {
                    closestTrashContainer = trashContainer;
                    shortestDistance = currentDistance;
                }
            }

            if (shortestDistance >= 0)
            {
                EventBus.Publish(MiniGame.EventId.MiniGameEvents.TrashContainerUsed, id, closestTrashContainer.trashContainerType);
            }

            transform.localPosition = startPosition;
        }
    }
}
