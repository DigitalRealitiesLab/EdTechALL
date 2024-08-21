namespace InteractionSystem.Interactable
{
    public class InteractableForwarder : Interactable
    {
        public override void Interact()
        {
            rootAsInteractable.Interact();
        }
    }
}