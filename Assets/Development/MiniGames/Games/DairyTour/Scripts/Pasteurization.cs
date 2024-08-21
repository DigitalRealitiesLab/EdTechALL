using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine;

namespace MiniGame.DairyTour
{
    public class Pasteurization : MonoBehaviour
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public Slider temperatureSlider;
        public Image backgroundImage;
        public Button startButton;
        public TextMeshProUGUI startButtonText;
        public Toggle[] toggles;
        public string startButtonTextContent = "Start";

        private int step = 0;

        Dictionary<int, bool> watchedVideos;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 1, "Pasteurization needs exactly 1 Sprites");
            Debug.Assert(videoClips.Length == 5, "Pasteurization needs exactly 5 VideoClips");
            Debug.Assert(audioClips.Length == 5, "Pasteurization needs exactly 5 AudioClips");
            Debug.Assert(backgroundImage, "Pasteurization is missing a reference to a Image");
            Debug.Assert(temperatureSlider, "Pasteurization is missing a reference to a Slider");
            Debug.Assert(startButton, "Pasteurization is missing a reference to a Button");
            Debug.Assert(startButtonText, "Pasteurization is missing a reference to a TextMeshProUGUI");
            Debug.Assert(toggles.Length == Mathf.RoundToInt(temperatureSlider.maxValue + 1.0f), "Pasteurization needs the same amount of Toggles and possible Slider Values");

            backgroundImage.sprite = sprites[0];
            backgroundImage.gameObject.SetActive(false);

            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");

            watchedVideos = new Dictionary<int, bool>();

            for (int i = 0; i <= Mathf.RoundToInt(temperatureSlider.maxValue); i++)
            {
                toggles[i].isOn = false;
                watchedVideos.Add(i, false);
            }

            startButtonText.text = startButtonTextContent;

            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
        }

        public void OnFullScreenPromptPanelClosed()
        {
            switch (step)
            {
                case 0:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[1], audioClips[1]);
                    step++;
                    break;
                case 1:
                    backgroundImage.gameObject.SetActive(true);
                    step++;
                    break;
                default:
                    foreach (bool watched in watchedVideos.Values)
                    {
                        if (!watched)
                        {
                            return;
                        }
                    }
                    string sendName = gameObject.name;
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
                    break;
            }
        }

        public void OnStartButtonPressed()
        {
            toggles[Mathf.RoundToInt(temperatureSlider.value)].isOn = true;
            watchedVideos[Mathf.RoundToInt(temperatureSlider.value)] = true;
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[2 + Mathf.RoundToInt(temperatureSlider.value)], audioClips[2 + Mathf.RoundToInt(temperatureSlider.value)]);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
        }
    }
}
