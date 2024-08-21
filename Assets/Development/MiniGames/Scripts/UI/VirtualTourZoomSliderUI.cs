using InfinityCode.uPano.HotSpots;
using UnityEngine.UI;
using UnityEngine;

namespace MiniGame
{
    public class VirtualTourZoomSliderUI : MonoBehaviour
    {
        public Slider zoomSlider;
        private int previousValue;
        public bool subMiniGameActive = false;
        public bool subMiniGameAudioPlaying = false;

        private void Awake()
        {
            Debug.Assert(zoomSlider, "VirtualTourZoomSliderUI is missing a reference to a Slider");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.EnableVirtualTourUI, "OnEnableVirtualTourUI");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
            zoomSlider.gameObject.SetActive(false);
            previousValue = Mathf.RoundToInt(zoomSlider.value);
        }

        public void OnZoomSliderValueChanged()
        {
            int value = Mathf.RoundToInt(zoomSlider.value);
            if (value != previousValue)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.VirtualTourZoomChanged, value));
                previousValue = value;
            }
        }

        public void OnEnableVirtualTourUI()
        {
            zoomSlider.gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
            zoomSlider.value = -3.0f;
            OnZoomSliderValueChanged();
        }

        public void OnSubMiniGameStarted(HotSpot sender)
        {
            subMiniGameActive = true;
            zoomSlider.gameObject.SetActive(false);
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            subMiniGameActive = false;
            zoomSlider.gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
        }

        public virtual void OnSubMiniGameAborted(string subMiniGameName)
        {
            subMiniGameActive = false;
            zoomSlider.gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
        }

        public void OnAudioSubMiniGameIsPlaying(bool isPlaying)
        {
            subMiniGameAudioPlaying = isPlaying;
            zoomSlider.gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.EnableVirtualTourUI, "OnEnableVirtualTourUI");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
        }
    }
}
