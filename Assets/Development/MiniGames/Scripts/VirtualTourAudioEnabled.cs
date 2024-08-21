using UnityEngine;

namespace MiniGame
{
    public class VirtualTourAudioEnabled : MonoBehaviour
    {
        public AudioSource audioSource;
        // counterpart in switchable tour
        public VirtualTourAudioEnabled other;
        public bool finishedPlaying = false;

        private bool abortWindowOpen = false;
        private bool audioWasPlaying = false;
        private bool isEnabled = false;
        private bool playAudio = false;

        private void Awake()
        {
            Debug.Assert(audioSource, "VirtualTourAudioEnabled is missing a reference to an AudioSource");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameAborted, "OnMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameAborted, "OnMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        private void OnEnable()
        {
            if (!finishedPlaying)
            {
                playAudio = true;
            }
            isEnabled = true;
        }

        private void OnDisable()
        {
            isEnabled = false;
        }

        private void Update()
        {
            if (isEnabled && !finishedPlaying && !audioSource.isPlaying && !abortWindowOpen)
            {
                if (playAudio)
                {
                    playAudio = false;
                    audioSource.Play();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, true));
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                }
                else
                {
                    End();
                }
            }
        }

        public void OnMiniGameAborted()
        {
            EventBus.Publish(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false);
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
            if (audioSource.isPlaying)
            {
                audioWasPlaying = true;
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        public void OnMiniGameResumed()
        {
            abortWindowOpen = false;
            if (audioWasPlaying)
            {
                audioWasPlaying = false;
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
        }
        public void OnReplayInstruction()
        {
            if (isEnabled && !abortWindowOpen)
            {
                finishedPlaying = false;
                if (other)
                {
                    other.finishedPlaying = false;
                }
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, true));
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        public void End()
        {
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, false));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            finishedPlaying = true;
            if (other)
            {
                other.finishedPlaying = true;
            }
        }
    }
}
