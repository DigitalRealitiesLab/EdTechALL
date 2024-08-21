using UnityEngine.UI;
using UnityEngine;

namespace MiniGame.FarmOverview
{
    public class TimePieChartImage : MonoBehaviour
    {
        public IncomeSource incomeSource;
        public Image image;

        private void Awake()
        {
            Debug.Assert(image, "TimePieChartImage is missing a reference to an Image");
            image.enabled = false;
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.AddIncome, "OnAddIncome");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, "OnSetMaxIncomeCount");
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.AddIncome, "OnAddIncome");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, "OnSetMaxIncomeCount");
        }

        public void OnAddIncome(IncomeSource addedIncomeSource)
        {
            if (incomeSource == addedIncomeSource)
            {
                image.enabled = true;
            }
        }

        public void OnSetMaxIncomeCount(int count)
        {
            image.enabled = false;
        }
    }
}
