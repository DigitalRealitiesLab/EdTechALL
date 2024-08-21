using UnityEngine;

namespace MiniGame.FarmOverview.UI
{
    public class IncomeUI : MonoBehaviour
    {
        private void Awake()
        {
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, "OnSetMaxIncomeCount");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
            gameObject.SetActive(false);
        }

        public void OnSetMaxIncomeCount(int count)
        {
            gameObject.SetActive(count > 0);
        }

        public void OnFinishCountingButtonPressed()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, "OnSetMaxIncomeCount");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
        }
    }
}
