using InteractionSystem.Interactable;

namespace MiniGame.FarmOverview
{
    public class IncomePublishBehaviour : AInteractionBehaviour
    {
        public IncomeSource incomeSource;
        public bool interacted = false;

        public override void OnInteract()
        {
            if (!interacted)
            {
                interacted = true;
                EventBus.Publish(EventId.FarmOverviewMiniGameEvents.AddIncome, incomeSource);
                gameObject.SetActive(false);
            }
        }
    }
}