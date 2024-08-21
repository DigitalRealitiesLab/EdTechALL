using UnityEngine;

namespace MiniGame
{
    using System.Collections;
    using MiniGame.UI;
    using UnityEngine.XR.ARSubsystems;

    public class BaseMiniGameManager : MonoBehaviour
    {
        public CardData cardData;
        public BaseMiniGameUIController baseMiniGameUIController;

        public MiniGameConfig miniGameConfig { get; set; } = null;
        public AMiniGame activeMiniGame { get; protected set; } = null;

        public XRReferenceImageLibrary referenceImageLibrary;
        public TextureToPrefabMapper prefabMapper;

        public bool hasTutorial = false;

        protected bool started = false;

        protected void Awake()
        {
            Debug.Assert(cardData, "BaseMiniGameManager is missing a reference to a CardData");
            Debug.Assert(baseMiniGameUIController, "BaseMiniGameManager is missing a reference to a BaseMiniGameUIController");
            Debug.Assert(referenceImageLibrary, "BaseMiniGameManager is missing a reference to a XRReferenceImageLibrary");
            Debug.Assert(prefabMapper, "BaseMiniGameManager is missing a reference to a TextureToPrefabMapper");

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameEnded, "OnMiniGameEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameAborted, "OnMiniGameAborted");

            miniGameConfig = cardData.miniGameData;
        }

        protected void Update()
        {
            if (!started && !hasTutorial)
            {
                started = true;
                for (int i = 0; i < baseMiniGameUIController.miniGameWindows.Length; i++)
                {
                    baseMiniGameUIController.miniGameWindows[i].enabled = false;
                    baseMiniGameUIController.miniGameWindows[i].gameObject.SetActive(false);
                }

                if (baseMiniGameUIController is MiniGameUIController)
                {
                    (baseMiniGameUIController as MiniGameUIController).OnRequestStartWindow();
                }
                else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
                {
                    (baseMiniGameUIController as VirtualTourMiniGameUIController).OnRequestStartWindow();
                }
                if (baseMiniGameUIController is MiniGameUIController)
                {
                    baseMiniGameUIController.OnRequestStartWindow();
                }
            }
        }

        public virtual void StartMiniGame()
        {
            if (miniGameConfig == null)
            {
                return;
            }

            activeMiniGame = FindObjectOfType<AMiniGame>();

            activeMiniGame.miniGameConfig = miniGameConfig;

            activeMiniGame.StartMiniGame();
        }

        public void ExitActiveMiniGame()
        {
            if (!activeMiniGame)
            {
                return;
            }
            activeMiniGame.ExitMiniGame();
        }

        public void AbortActiveMiniGame()
        {
            if (!activeMiniGame)
            {
                return;
            }
            activeMiniGame.AbortMiniGame();
        }

        public void OnMiniGameEnded()
        {
            StartCoroutine(ExitCoroutine());
        }

        public IEnumerator ExitCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            XRReferenceImage referenceImage = referenceImageLibrary[0];
            ReferenceLibrarySignature libraryRequestSignature = new ReferenceLibrarySignature();
            libraryRequestSignature.texture = referenceImage.texture;
            libraryRequestSignature.widthInMeters = referenceImage.width;
            libraryRequestSignature.textureName = referenceImage.name;
            EdTechALLConfig.referenceLibrary = libraryRequestSignature;
            EdTechALLConfig.prefabMapper = prefabMapper;

            PlayerPrefs.SetInt(Tutorial.InitialTutorial.initialTutorialSkipString, 1);
            PlayerPrefs.Save();

            SceneLoader.RequestLoadSceneSingle("MiniGameMarkerDetectionScene");
        }

        public void OnMiniGameAborted()
        {
            StartCoroutine(AbortCoroutine());
        }

        public IEnumerator AbortCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            XRReferenceImage referenceImage = referenceImageLibrary[0];
            ReferenceLibrarySignature libraryRequestSignature = new ReferenceLibrarySignature();
            libraryRequestSignature.texture = referenceImage.texture;
            libraryRequestSignature.widthInMeters = referenceImage.width;
            libraryRequestSignature.textureName = referenceImage.name;
            EdTechALLConfig.referenceLibrary = libraryRequestSignature;
            EdTechALLConfig.prefabMapper = prefabMapper;

            PlayerPrefs.SetInt(Tutorial.InitialTutorial.initialTutorialSkipString, 1);
            PlayerPrefs.Save();

            SceneLoader.RequestLoadSceneSingle("MiniGameMarkerDetectionScene");
        }

        protected void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameEnded, "OnMiniGameEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameAborted, "OnMiniGameAborted");
        }
    }

    public partial class EventId
    {
        public static class MiniGameEvents
        {
            public const string MiniGameStarted = "MiniGameStarted";

            public const string MiniGameEnded = "MiniGameEnded";

            public const string RequestScoreWindow = "RequestScoreWindow";

            public const string SetScoreSprite = "SetScoreSprite";

            public const string RequestStartWindow = "RequestStartWindow";

            public const string RequestAbortWindow = "RequestAbortWindow";

            public const string MiniGameAborted = "MiniGameAborted";

            public const string MiniGameResumed = "MiniGameResumed";

            public const string RequestPromptWindow = "RequestPromptWindow";

            public const string PromptWindowClosed = "PromptWindowClosed";

            public const string RequestQuestionWindow = "RequestQuestionWindow";

            public const string RequestDiaryWindow = "RequestDiaryWindow";

            public const string QuestionWindowYes = "QuestionWindowYes";

            public const string QuestionWindowNo = "QuestionWindowNo";

            public const string DiaryContinue = "DiaryContinue";

            public const string DiaryWindowClosed = "DiaryWindowClosed";

            public const string PreviousStep = "PreviousStep";

            public const string NextStep = "NextStep";

            public const string ReplayInstruction = "ReplayInstruction";

            public const string OverrideText = "OverrideText";

            public const string MunicipalityTouch = "MunicipalityTouch";

            public const string LandmarkTouch = "LandmarkTouch";

            public const string AnimalTouch = "AnimalTouch";

            public const string ToolUsed = "ToolUsed";

            public const string StoreUsed = "StoreUsed";

            public const string TrashContainerUsed = "TrashContainerUsed";

            public const string PackagingBundleActivated = "PackagingBundleActivated";

            public const string StartPositioningQuiz = "StartPositioningQuiz";

            public const string PositioningQuizEnded = "PositioningQuizEnded";

            public const string AbortPositioningQuiz = "AbortPositioningQuiz";

            public const string PositioningQuizConfirm = "PositioningQuizConfirm";

            public const string SwitchableVirtualTourSetOther = "SwitchableVirtualTourSetOther";

            public const string EnableVirtualTourUI = "EnableVirtualTourUI";

            public const string VirtualTourZoomChanged = "VirtualTourZoomChanged";

            public const string VirtualTourStep = "VirtualTourStep";

            public const string VirtualTourNavigate = "VirtualTourNavigate";

            public const string SubMiniGameStarted = "SubMiniGameStarted";

            public const string SubMiniGameAborted = "SubMiniGameAborted";

            public const string SubMiniGameEnded = "SubMiniGameEnded";

            public const string SetSubMiniGame = "SetSubMiniGame";

            public const string AudioSubMiniGameIsPlaying = "AudioSubMiniGameIsPlaying";

            public const string EnablePhotoButton = "EnablePhotoButton";

            public const string EnableSkipAudioButton = "EnableSkipAudioButton";

            public const string EnableVisualGuidance = "EnableVisualGuidance";

            public const string EnableMarkerTrackingIndicator = "EnableMarkerTrackingIndicator";

            public const string PhotoButtonPressed = "PhotoButtonPressed";

            public const string SkipAudioButtonPressed = "SkipAudioButtonPressed";

            public const string SendBestMarkerWeight = "SendBestMarkerWeight";

            public const string ToggleInteractionSelectionPanel = "ToggleInteractionSelectionPanel";

            public const string SelectedInteractionMethod = "SelectedInteractionMethod";
        }

        public const string AdminMode = "AdminMode";

        public const string QuitAdminMode = "QuitAdminMode";

        public const string InteractionMethod = "InteractionMethod";

        public const string GameMode = "GameMode";

        public static string[] SettingsToggle = { AdminMode };
        public static string[] SettingsToggleGroup = { InteractionMethod, GameMode };

        public const string GameModeProgress = "GameModeProgress";

        public const string GameModeMarker = "GameModeMarker";

        public const string DeletedSaves = "DeletedSaves";

        public const string ARSessionStateChanged = "OnARSessionStateChanged";

        public const string Error = "Error";

        public const string RequestMiniGameLoadWindow = "RequestMiniGameLoadWindow";

        public const string RequestCloseAppWindow = "RequestCloseAppWindow";

        public const string RequestDeleteSaveWindow = "RequestDeleteSaveWindow";
    }
}