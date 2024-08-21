using UnityEngine.UI;
using UnityEngine;

namespace MiniGame
{
    public class VirtualTourCheckmarkUI : MonoBehaviour
    {
        public Toggle checkmarkToggle;
        public VisualGuidanceTarget visualGuidanceTarget;
        public string subMiniGameName;
        public bool inNavigationBar = true;

        private void Awake()
        {
            Debug.Assert(checkmarkToggle, "VirtualTourCheckmarkUI is missing a reference to a Toggle");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SetSubMiniGame, "OnSetSubMiniGame");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameAborted, "OnMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameEnded, "OnMiniGameEnded");
        }

        private void Start()
        {
            if (!inNavigationBar)
            {
                subMiniGameName = transform.parent.parent.parent.gameObject.name;
            }
        }

        public void OnSubMiniGameEnded(string otherSubMiniGameName)
        {
            if (subMiniGameName == otherSubMiniGameName)
            {
                checkmarkToggle.isOn = true;
                if (visualGuidanceTarget)
                {
                    visualGuidanceTarget.gameObject.SetActive(false);
                }
            }
        }

        public void OnSetSubMiniGame(string otherSubMiniGameName)
        {
            if (subMiniGameName == otherSubMiniGameName)
            {
                checkmarkToggle.isOn = true;
                if (visualGuidanceTarget)
                {
                    visualGuidanceTarget.gameObject.SetActive(false);
                }
            }
        }

        public void OnMiniGameAborted()
        {
            checkmarkToggle.isOn = false;
            if (visualGuidanceTarget)
            {
                visualGuidanceTarget.gameObject.SetActive(true);
            }
        }

        public void OnMiniGameEnded()
        {
            checkmarkToggle.isOn = false;
            if (visualGuidanceTarget)
            {
                visualGuidanceTarget.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SetSubMiniGame, "OnSetSubMiniGame");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameAborted, "OnMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameEnded, "OnMiniGameEnded");
        }
    }
}
