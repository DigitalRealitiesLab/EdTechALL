using UnityEngine;
using TMPro;
using System.Collections;

namespace MiniGame.FarmOverview.UI
{
    public class CowFindUI : MonoBehaviour
    {
        public TextMeshProUGUI cowCountText;
        private int maxCowCount = 0;
        private int cowCount = 0;
        public int CowCount
        {
            get
            {
                return cowCount;
            }
            set
            {
                cowCount = value;
                cowCountText.text = cowCount.ToString();
            }
        }

        private void Awake()
        {
            Debug.Assert(cowCountText, "CowFindUI is missing a reference to a TMPro cowCountText");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, "OnSetMaxIncomeCount");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.DeactivateUI, "OnDeactivateUI");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxFindCow, "OnSetMaxFindCow");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetFindCow, "OnSetFindCow");
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            EventBus.SaveRegisterCallback(this, EventId.FarmOverviewMiniGameEvents.FindCow, "OnFindCow");
        }

        public void OnFindCow()
        {
            CowCount++;
        }

        public void OnSetMaxIncomeCount(int count)
        {
            gameObject.SetActive(count == 0);
        }

        public void OnDeactivateUI()
        {
            StartCoroutine(DeactivateCoroutine());
        }

        private IEnumerator DeactivateCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            gameObject.SetActive(false);
        }

        public void OnSetMaxFindCow(int maxFindCow)
        {
            maxCowCount = maxFindCow;
        }

        public void OnSetFindCow(int findCow)
        {
            CowCount = findCow;
        }

        public void OnFinishCountingButtonPressed()
        {
            cowCountText.text = cowCount.ToString() + "/" + maxCowCount.ToString();
        }

        public void OnDisable()
        {
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.FindCow, "OnFindCow");
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxIncomeCount, "OnSetMaxIncomeCount");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.DeactivateUI, "OnDeactivateUI");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetMaxFindCow, "OnSetMaxFindCow");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.SetFindCow, "OnSetFindCow");
            EventBus.SaveDeregisterCallback(this, EventId.FarmOverviewMiniGameEvents.FinishCountingButtonPressed, "OnFinishCountingButtonPressed");
        }
    }
}
