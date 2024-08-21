namespace InteractionSystem.Interactable
{
    public class InteractableEventPublisher : AInteractionBehaviour
    {
        public string eventId;

        public override void OnInteract()
        {
            EventBus.Publish(eventId);
        }
    }
}
