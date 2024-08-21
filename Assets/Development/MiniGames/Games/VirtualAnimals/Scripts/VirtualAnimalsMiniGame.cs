using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.VirtualAnimals
{
    public class VirtualAnimalsMiniGame : AMiniGame
    {
        public Sprite[] sprites;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public VirtualAnimal[] animals;
        private List<VirtualAnimal.AnimalType> touchedAnimals = new List<VirtualAnimal.AnimalType>();
        private int step = 0;
        private bool popUpOpened = true;
        private bool nextStepOnClose = false;
        private bool audioWasPlaying = false;
        private bool abortWindowOpen = false;
        private bool scoreWindowOpen = false;
        private bool startWindowOpen = false;
        private bool popUpActive = false;
        private bool goingBack = false;
        private float activationTimer = 0.1f;
        private float activationTime = 0.0f;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 4, "RealisticCowMiniGame needs exactly 4 Sprites");
            Debug.Assert(audioClips.Length == 8, "RealisticCowMiniGame needs exactly 8 AudioClips");
            Debug.Assert(audioSource, "RealisticCowMiniGame is missing a reference to an AudioSource");
            Debug.Assert(animals != null && animals.Length == 5, "RealisticCowMiniGame Animals not setup correctly");

            foreach (VirtualAnimal animal in animals)
            {
                animal.touchCollider.enabled = false;
                if (animal.type == VirtualAnimal.AnimalType.Calf || animal.type == VirtualAnimal.AnimalType.Bull)
                {
                    animal.gameObject.SetActive(false);
                    animal.visualGuidanceTarget.gameObject.SetActive(false);
                }
                else
                {
                    animal.gameObject.SetActive(true);
                    animal.visualGuidanceTarget.gameObject.SetActive(true);
                }
            }
        }

        public override void AbortMiniGame()
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AnimalTouch, "OnAnimalTouch");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void ExitMiniGame()
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AnimalTouch, "OnAnimalTouch");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }

        public override void StartMiniGame()
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, false);
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AnimalTouch, "OnAnimalTouch");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            audioSource.clip = audioClips[0];
            audioSource.Play();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            popUpOpened = false;
        }

        private void Update()
        {
            if (!popUpOpened && !audioSource.isPlaying && !startWindowOpen && !abortWindowOpen && !scoreWindowOpen)
            {
                if (activationTime >= activationTimer)
                {
                    goingBack = false;
                    popUpOpened = true;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));

                    switch (step)
                    {
                        case 0:
                            foreach (VirtualAnimal animal in animals)
                            {
                                if (animal.type != VirtualAnimal.AnimalType.Calf && animal.type != VirtualAnimal.AnimalType.Bull)
                                {
                                    animal.touchCollider.enabled = true;
                                }
                            }
                            break;

                        case 1:
                            if (touchedAnimals.Count == 3)
                            {
                                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
                            }
                            else
                            {
                                foreach (VirtualAnimal animal in animals)
                                {
                                    if (animal.type != VirtualAnimal.AnimalType.Goat && animal.type != VirtualAnimal.AnimalType.Sheep)
                                    {
                                        animal.touchCollider.enabled = true;
                                    }
                                }
                            }
                            break;
                    }

                }
                else
                {
                    activationTime += Time.deltaTime;
                }
            }
            else
            {
                activationTime = 0.0f;
            }
        }

        private IEnumerator RequestPromptCoroutine(Sprite sprite = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);
            popUpActive = true;
            RequestPromptWindow("", 0.0f, sprite, null, audioClip);
            foreach (VirtualAnimal animal in animals)
            {
                animal.touchCollider.enabled = false;
            }
        }

        public void OnAnimalTouch(VirtualAnimal.AnimalType type)
        {
            bool failed = true;
            foreach (VirtualAnimal animal in animals)
            {
                if (animal.type == type && animal.touchCollider.enabled)
                {
                    if (!touchedAnimals.Contains(type))
                    {
                        failed = false;
                        touchedAnimals.Add(type);
                        animal.visualGuidanceTarget.gameObject.SetActive(false);

                        if (touchedAnimals.Count == 3 && step == 0)
                        {
                            nextStepOnClose = true;
                        }

                        switch(type)
                        {
                            case VirtualAnimal.AnimalType.Cow:
                                switch (step)
                                {
                                    case 0:
                                        StartCoroutine(RequestPromptCoroutine(sprites[0], audioClips[1]));
                                        break;
                                    case 1:
                                        audioSource.clip = audioClips[5];
                                        audioSource.Play();
                                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                                        popUpOpened = false;
                                        break;
                                }
                                break;
                            case VirtualAnimal.AnimalType.Goat:
                                StartCoroutine(RequestPromptCoroutine(sprites[1], audioClips[2]));
                                break;
                            case VirtualAnimal.AnimalType.Sheep:
                                StartCoroutine(RequestPromptCoroutine(sprites[2], audioClips[3]));
                                break;
                            case VirtualAnimal.AnimalType.Calf:
                                audioSource.clip = audioClips[6];
                                audioSource.Play();
                                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                                popUpOpened = false;
                                break;
                            case VirtualAnimal.AnimalType.Bull:
                                audioSource.clip = audioClips[7];
                                audioSource.Play();
                                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                                popUpOpened = false;
                                break;
                        }
                    }
                }
            }
            if (!failed)
            {
                foreach (VirtualAnimal animal in animals)
                {
                    animal.touchCollider.enabled = false;
                }
            }
        }

        public void OnPromptWindowClosed()
        {
            popUpActive = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
            }
            else if (!startWindowOpen && !abortWindowOpen && !scoreWindowOpen && !goingBack)
            {
                switch (step)
                {
                    case 0:
                        foreach (VirtualAnimal animal in animals)
                        {
                            if (animal.type != VirtualAnimal.AnimalType.Calf && animal.type != VirtualAnimal.AnimalType.Bull)
                            {
                                animal.touchCollider.enabled = true;
                            }
                        }
                        break;
                    case 1:
                        foreach (VirtualAnimal animal in animals)
                        {
                            if (animal.type != VirtualAnimal.AnimalType.Goat && animal.type != VirtualAnimal.AnimalType.Sheep)
                            {
                                animal.touchCollider.enabled = true;
                            }
                        }
                        break;
                }
            }
        }

        public void OnRequestAbortWindow()
        {
            foreach (VirtualAnimal animal in animals)
            {
                animal.touchCollider.enabled = false;
            }
            abortWindowOpen = true;

            if (audioSource.isPlaying)
            {
                audioWasPlaying = true;
                audioSource.Stop();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            }
        }

        public void OnRequestScoreWindow()
        {
            scoreWindowOpen = true;
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            touchedAnimals.Clear();
            switch (step)
            {
                case 0:
                    foreach (VirtualAnimal animal in animals)
                    {
                        animal.touchCollider.enabled = false;
                        if (animal.type == VirtualAnimal.AnimalType.Calf || animal.type == VirtualAnimal.AnimalType.Bull)
                        {
                            animal.gameObject.SetActive(false);
                            animal.visualGuidanceTarget.gameObject.SetActive(false);
                        }
                        else
                        {
                            animal.gameObject.SetActive(true);
                            animal.visualGuidanceTarget.gameObject.SetActive(true);
                        }
                    }
                    break;
                case 1:
                    foreach (VirtualAnimal animal in animals)
                    {
                        animal.touchCollider.enabled = false;
                        if (animal.type == VirtualAnimal.AnimalType.Goat || animal.type == VirtualAnimal.AnimalType.Sheep)
                        {
                            animal.gameObject.SetActive(false);
                            animal.visualGuidanceTarget.gameObject.SetActive(false);
                        }
                        else
                        {
                            animal.gameObject.SetActive(true);
                            animal.visualGuidanceTarget.gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }

        public void OnMiniGameResumed()
        {
            abortWindowOpen = false;

            foreach (VirtualAnimal animal in animals)
            {
                animal.touchCollider.enabled = false;
            }

            if (!scoreWindowOpen && !startWindowOpen && audioWasPlaying)
            {
                audioWasPlaying = false;
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
            else if (!scoreWindowOpen && !startWindowOpen && nextStepOnClose)
            {
                OnNextStep();
            }
            else
            {
                nextStepOnClose = false;
                scoreWindowOpen = false;
                startWindowOpen = false;
                audioWasPlaying = false;
                switch (step)
                {
                    case 0:
                        audioSource.clip = audioClips[0];
                        audioSource.Play();
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                        popUpOpened = false;
                        break;
                    case 1:
                        StartCoroutine(RequestPromptCoroutine(sprites[3], audioClips[4]));
                        break;
                }
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            popUpOpened = true;
            goingBack = false;
            if (miniGameConfig.steps.Length <= step + 1)
            {
                nextStepOnClose = false;
                step = miniGameConfig.steps.Length - 1;
                RequestScoreWindow();
            }
            else
            {
                if (scoreWindowOpen)
                {
                    nextStepOnClose = false;
                }
                if (!abortWindowOpen)
                {
                    audioSource.Stop();
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                    nextStepOnClose = false;
                    touchedAnimals.Clear();
                    foreach (VirtualAnimal animal in animals)
                    {
                        animal.touchCollider.enabled = false;
                        if (animal.type == VirtualAnimal.AnimalType.Goat || animal.type == VirtualAnimal.AnimalType.Sheep)
                        {
                            animal.gameObject.SetActive(false);
                            animal.visualGuidanceTarget.gameObject.SetActive(false);
                        }
                        else
                        {
                            animal.gameObject.SetActive(true);
                            animal.visualGuidanceTarget.gameObject.SetActive(true);
                        }
                    }
                    StartCoroutine(RequestPromptCoroutine(sprites[3], audioClips[4]));
                    step++;
                }
            }
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            popUpOpened = false;
            nextStepOnClose = false;
            touchedAnimals.Clear();
            audioSource.Stop();
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
            step--;
            if (step < 0)
            {
                step = 0;
                startWindowOpen = true;
                RequestStartWindow();
            }
            else
            {
                goingBack = true;
                audioSource.clip = audioClips[0];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
            }
            foreach (VirtualAnimal animal in animals)
            {
                animal.touchCollider.enabled = false;
                if (animal.type == VirtualAnimal.AnimalType.Calf || animal.type == VirtualAnimal.AnimalType.Bull)
                {
                    animal.gameObject.SetActive(false);
                    animal.visualGuidanceTarget.gameObject.SetActive(false);
                }
                else
                {
                    animal.gameObject.SetActive(true);
                    animal.visualGuidanceTarget.gameObject.SetActive(true);
                }
            }
        }

        public void OnReplayInstruction()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.Play();
            }
            else
            {
                if (!popUpActive)
                {
                    foreach (VirtualAnimal animal in animals)
                    {
                        animal.touchCollider.enabled = false;
                    }
                    switch (step)
                    {
                        case 0:
                            audioSource.clip = audioClips[0];
                            audioSource.Play();
                            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                            popUpOpened = false;
                            break;
                        case 1:
                            StartCoroutine(RequestPromptCoroutine(sprites[3], audioClips[4]));
                            break;
                    }
                }
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }
    }
}