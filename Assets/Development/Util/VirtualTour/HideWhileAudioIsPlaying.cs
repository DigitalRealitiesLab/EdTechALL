using UnityEngine;

namespace MiniGame
{
    public class HideWhileAudioIsPlaying : MonoBehaviour
    {
        private void Awake()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
        }

        public void OnAudioSubMiniGameIsPlaying(bool isPlaying)
        {
            gameObject.SetActive(!isPlaying);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
        }
    }
}