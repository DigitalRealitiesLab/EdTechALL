using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.FarmOverview.UI
{
    public class TypeSelectionPanel : MonoBehaviour
    {
        public ToggleGroup toggleGroup;
        public UIPanel incomeUI;
        public MiniGame.UI.MiniGameInfoUI infoUI;
        public MiniGame.UI.ToggleInfoUI toggleInfoUI;

        private int previousToggleIndex = 0;
        private int toggleIndex = 0;

        private void Awake()
        {
            infoUI = FindObjectOfType<MiniGame.UI.MiniGameInfoUI>(true);
            toggleInfoUI = FindObjectOfType<MiniGame.UI.ToggleInfoUI>(true);
            Debug.Assert(toggleGroup, "TypeSelectionPanel is missing a reference to a toggle group");
            Debug.Assert(incomeUI, "TypeSelectionPanel is missing a reference to a UI panel");
            Debug.Assert(infoUI, "TypeSelectionPanel is missing a reference to a MiniGameInfoUI");
            Debug.Assert(toggleInfoUI, "TypeSelectionPanel is missing a reference to a ToggleInfoUI");
        }

        private void Start()
        {
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
            }
            previousToggleIndex = toggleIndex;
        }

        private void OnEnable()
        {
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(false);
            infoUI.gameObject.SetActive(false);
            infoUI.exitButton.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            toggleInfoUI.toggleInfoImage.gameObject.SetActive(true);
            infoUI.gameObject.SetActive(toggleInfoUI.infoUIActive);
            infoUI.exitButton.gameObject.SetActive(true);
        }

        private void ToggleValueChanged(Toggle toggle)
        {
            if (toggle.isOn)
            {
                previousToggleIndex = toggleIndex;
                toggleIndex = toggle.transform.GetSiblingIndex();
            }
        }

        public void OnConfirm()
        {
            if (previousToggleIndex == toggleIndex)
            {
                foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
                {
                    ToggleValueChanged(toggle);
                }
            }

            incomeUI.gameObject.SetActive(true);
            gameObject.SetActive(false);

            EventBus.Publish(EventId.FarmOverviewMiniGameEvents.PrefabSpawned, toggleIndex);
        }
    }
}
