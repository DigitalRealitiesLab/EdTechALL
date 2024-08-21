using MiniGame;
using UnityEngine;

namespace Tutorial
{
    public class BaseTutorial : MonoBehaviour
    {
        public MultiMarkerTracker multiMarkerTracker;
        public MiniGameManager miniGameManager;
        public UIPanelController panelController;
        public Sprite[] sprites;
        public AudioSource audioSource;
        public AudioClip[] audioClips;

        private const string scanText = "Schaue durch das Tablet auf die Karte.";

        private int gameMode = 0;

        private int step = 0;

        private bool tutorialSkip = false;

        public const string markerTutorialSkipString = "MarkerTutorialSkip";

        void Start()
        {
            Debug.Assert(multiMarkerTracker, "BaseTutorial is missing a reference to a MultiMarkerTracker");
            Debug.Assert(miniGameManager, "BaseTutorial is missing a reference to a MiniGameManager");
            Debug.Assert(sprites.Length == 3, "BaseTutorial needs exactly 3 Sprites");
            Debug.Assert(audioSource, "BaseTutorial is missing a reference to an AudioSource");
            Debug.Assert(audioClips.Length == 3, "BaseTutorial needs exactly 3 AudioClips");

            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");

            if (PlayerPrefs.HasKey(MiniGame.EventId.GameMode))
            {
                gameMode = PlayerPrefs.GetInt(MiniGame.EventId.GameMode);
            }

            if (PlayerPrefs.HasKey(markerTutorialSkipString))
            {
                tutorialSkip = PlayerPrefs.GetInt(markerTutorialSkipString) != 0;
            }

            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[0], null, audioClips[0]);
        }

        void Update()
        {
            if (step == 3 && miniGameManager.hasTutorial && multiMarkerTracker.hasInstance && !panelController.GetPanelActiveByIndex(UIPanel.SettingsPanel))
            {
                UIPanel.ActivateUIPanelSingle(UIPanel.MiniGamePanel);
                miniGameManager.hasTutorial = false;
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            switch (step)
            {
                case 0:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel(scanText, 0.0f, sprites[1], null, audioClips[1]);
                    break;
                case 1:
                    switch (gameMode)
                    {
                        case 0:
                            if (miniGameManager.miniGameConfig.type == MiniGameType.MunicipalitySearch)
                            {
                                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[2], null, audioClips[2]);
                            }
                            else
                            {
                                step++;
                            }
                            break;
                        case 1:
                            if (!tutorialSkip)
                            {
                                tutorialSkip = true;
                                PlayerPrefs.SetInt(markerTutorialSkipString, 1);
                                PlayerPrefs.Save();
                                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[2], null, audioClips[2]);
                            }
                            else
                            {
                                step++;
                            }
                            break;
                        case 2:
                            step++;
                            break;
                    }
                    break;
                case 2:
                    break;
            }

            if (step < 3)
            {
                step++;
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
        }
    }
}
