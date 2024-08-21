using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MiniGame
{
    public class InteractionSelectionPanel : MonoBehaviour
    {
        public VideoClip[] videoClips;
        public RawImage[] videoTextures;
        public VideoPlayer[] videoPlayers;
        public TextMeshProUGUI heading;
        public ToggleGroup toggleGroup;
        public Button exitButton;

        private RenderTexture renderTexture0;
        private RenderTexture renderTexture1;

        private int currentHeadingIndex = -1;

        private int previousToggleIndex = 0;
        private int toggleIndex = 0;

        private bool ignoreValueChange = false;
        private Toggle initialToggle = null;

        private string[] headings = { "Suche aus, wie du die Bauernhöfe erkunden möchtest.", "Suche aus, wie du die Molkerei erkunden möchtest." };

        private void Awake()
        {
            Debug.Assert(toggleGroup, "InteractionSelectionPanel is missing a reference to a toggle group");

            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn)
                {
                    if (!initialToggle || initialToggle == toggle)
                    {
                        initialToggle = toggle;
                    }
                    else
                    {
                        toggle.isOn = false;
                    }
                }
            }

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ToggleInteractionSelectionPanel, "OnToggleInteractionSelectionPanel");
        }

        private void Start()
        {
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
            }

            previousToggleIndex = toggleIndex;

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            exitButton.gameObject.SetActive(false);
            if (initialToggle)
            {
                currentHeadingIndex = -1;
                previousToggleIndex = 0;
                toggleIndex = 0;
                ignoreValueChange = true;
                foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
                {
                    toggle.isOn = initialToggle == toggle;
                }
                ignoreValueChange = false;
            }
        }

        private void OnDisable()
        {
            exitButton.gameObject.SetActive(true);
        }

        private void ToggleValueChanged(Toggle toggle)
        {
            if (toggle.isOn && !ignoreValueChange)
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

            EventBus.Publish(EventId.MiniGameEvents.ToggleInteractionSelectionPanel, false, currentHeadingIndex);

            EventBus.Publish(EventId.MiniGameEvents.SelectedInteractionMethod, toggleIndex + 1);
        }

        public void OnToggleInteractionSelectionPanel(bool toggle, int headingIndex)
        {
            if (toggle)
            {
                gameObject.SetActive(true);

                if (currentHeadingIndex != headingIndex)
                {
                    videoPlayers[0].clip = videoClips[headingIndex * 2];
                    videoPlayers[1].clip = videoClips[1 + headingIndex * 2];

                    if (currentHeadingIndex == -1)
                    {
                        RectTransform rectTransform0 = videoPlayers[0].GetComponent<RectTransform>();

                        if (renderTexture0)
                        {
                            if (renderTexture0.IsCreated())
                            {
                                renderTexture0.Release();
                            }
                        }
                        renderTexture0 = new RenderTexture((int)rectTransform0.rect.width, (int)rectTransform0.rect.height, 32);
                        renderTexture0.Create();

                        videoTextures[0].texture = renderTexture0;
                        videoPlayers[0].targetTexture = renderTexture0;

                        RectTransform rectTransform1 = videoPlayers[1].GetComponent<RectTransform>();

                        if (renderTexture1)
                        {
                            if (renderTexture1.IsCreated())
                            {
                                renderTexture1.Release();
                            }
                        }
                        renderTexture1 = new RenderTexture((int)rectTransform1.rect.width, (int)rectTransform1.rect.height, 32);
                        renderTexture1.Create();

                        videoTextures[1].texture = renderTexture1;
                        videoPlayers[1].targetTexture = renderTexture1;
                    }

                    videoPlayers[0].Play();
                    videoPlayers[1].Play();
                }
                else
                {
                    videoPlayers[0].Play();
                    videoPlayers[1].Play();
                }
            }
            else
            {
                videoPlayers[0].Stop();
                videoPlayers[1].Stop();

                gameObject.SetActive(false);
            }

            currentHeadingIndex = headingIndex;
            heading.text = headings[headingIndex];
            toggleGroup.GetComponentsInChildren<Toggle>()[0].isOn = toggle;
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ToggleInteractionSelectionPanel, "OnToggleInteractionSelectionPanel");

            if (renderTexture0)
            {
                if (renderTexture0.IsCreated())
                {
                    renderTexture0.Release();
                }
            }

            if (renderTexture1)
            {
                if (renderTexture1.IsCreated())
                {
                    renderTexture1.Release();
                }
            }
        }
    }
}
