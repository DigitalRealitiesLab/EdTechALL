using InteractionSystem.Interactable;

namespace MiniGame.FarmOverview
{
    public class CowFindBehaviour : AInteractionBehaviour
    {
        public bool interacted = false;

        protected override void Awake()
        {
            base.Awake();
            transform.parent.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public override void OnInteract()
        {
            if (!interacted)
            {
                interacted = true;
                EventBus.Publish(EventId.FarmOverviewMiniGameEvents.FindCow);
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}