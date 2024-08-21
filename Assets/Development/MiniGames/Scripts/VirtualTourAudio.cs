using UnityEngine;

namespace MiniGame.FarmTour
{
    public class VirtualTourAudio : MonoBehaviour
    {
        public AudioSource audioSource;

        private void Awake()
        {
            Debug.Assert(audioSource, "VirtualTourAudio is missing a reference to an AudioSource");

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, true));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                EndSubMiniGame();
            }
        }

        public void EndSubMiniGame()
        {
            string sendName = gameObject.name;
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }
    }
}
