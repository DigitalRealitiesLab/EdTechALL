using UnityEngine;

namespace InteractionSystem.Interactable
{
    [RequireComponent(typeof(Interactable))]
    public abstract class AInteractionBehaviour : MonoBehaviour
    {
        public Interactable interactable;

        protected virtual void Awake()
        {
            interactable = GetComponent<Interactable>();
        }

        public abstract void OnInteract();
    }
}