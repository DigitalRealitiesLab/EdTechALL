using UnityEngine.Video;
using UnityEngine;

namespace Tutorial
{
    public class InitialTutorial : MonoBehaviour
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;

        private const string startText = "Hallo! Willkommen bei EdTechALL!";
        private const string headphonesText = "Setze die Kopfhörer auf.\nDann hörst du, wie es weiter geht.";

        private int gameMode = 0;

        private int step = 0;

        private bool tutorialSkip = false;

        public const string initialTutorialSkipString = "InitialTutorialSkip";

        private void Start()
        {
            Debug.Assert(sprites.Length == 3, "InitialTutorial needs exactly 3 Sprites");
            Debug.Assert(videoClips.Length == 2, "InitialTutorial needs exactly 2 VideoClips");
            Debug.Assert(audioClips.Length == 4, "InitialTutorial needs exactly 4 AudioClips");

            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");

            if (PlayerPrefs.HasKey(MiniGame.EventId.GameMode))
            {
                gameMode = PlayerPrefs.GetInt(MiniGame.EventId.GameMode);
            }

            if (PlayerPrefs.HasKey(initialTutorialSkipString))
            {
                tutorialSkip = PlayerPrefs.GetInt(initialTutorialSkipString) != 0;
            }

            if (tutorialSkip)
            {
                step = 3;
                PlayerPrefs.SetInt(initialTutorialSkipString, 0);
                PlayerPrefs.Save();

                switch (gameMode)
                {
                    case 0:
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[2], null, audioClips[1]);
                        break;
                    case 1:
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[1], audioClips[2]);
                        break;
                    case 2:
                        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[2], null, audioClips[3]);
                        break;
                }
            }
            else
            {
                FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel(startText, 0.0f, sprites[0]);
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            switch (step)
            {
                case 0:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel(headphonesText, 3.0f, sprites[1]);
                    break;
                case 1:
                    FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
                    break;
                case 2:
                    switch (gameMode)
                    {
                        case 0:
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[2], null, audioClips[1]);
                            break;
                        case 1:
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[1], audioClips[2]);
                            break;
                        case 2:
                            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, sprites[2], null, audioClips[3]);
                            break;
                    }
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
