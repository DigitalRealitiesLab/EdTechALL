namespace InteractionSystem.Interactable
{
    public class Interactable : Sensable, IInteractable
    {
        public Interactable rootAsInteractable => root as Interactable;
        public virtual void Interact()
        {
            foreach (AInteractionBehaviour interactionBehaviour in GetComponents<AInteractionBehaviour>())
            {
                if (interactionBehaviour.enabled)
                {
                    interactionBehaviour.OnInteract();
                }
            }
        }
    }

    [System.Serializable]
    public class InteractableEvent : UnityEngine.Events.UnityEvent<Interactable> { }
}
