namespace InteractionSystem.Interactable
{
    public class InteractableEventExtension : AInteractionBehaviour
    {
        public UnityEngine.Events.UnityEvent OnInteractEvent = new UnityEngine.Events.UnityEvent();
        public InteractableEvent OnInteractEventParam = new InteractableEvent();

        public override void OnInteract()
        {
            OnInteractEvent.Invoke(); OnInteractEventParam.Invoke(interactable);
        }

        private void OnDestroy()
        {
            OnInteractEvent.RemoveAllListeners();
            OnInteractEventParam.RemoveAllListeners();
        }
    }
}