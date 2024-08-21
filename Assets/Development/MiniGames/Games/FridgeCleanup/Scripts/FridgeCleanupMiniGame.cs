using System.Collections;
using System.Collections.Generic;
using InteractionSystem.Food;
using InteractionSystem.Packaging;
using InteractionSystem.TrashContainer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MiniGame.FridgeCleanup
{
    public class FridgeCleanupMiniGame : AMiniGame
    {
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public GameObject checkmarkPrefab;

        public const string bestBeforeText = "Das Mindesthaltbarkeitsdatum ist am {0}.";

        private int step = 0;

        private bool nextStepOnClose = false;
        private bool abortWindowOpen = false;
        private bool scoreWindowOpen = false;
        private bool popUpActive = false;
        private bool openEndPopUp = false;

        private Dictionary<int, Food> groceries = new Dictionary<int, Food>();
        private List<Tool> tools;
        private List<Store> stores;

        private Dictionary<int, Packaging> packagings = new Dictionary<int, Packaging>();
        private Dictionary<int, PackagingBundle> packagingBundles = new Dictionary<int, PackagingBundle>();
        private List<TrashContainer> trashContainers;

        private int trashCount = 0;
        private bool oneTrashPopUp = false;

        private int fridgeTutorialSubstep = 0;
        private int trashTutorialSubstep = 0;

        private void Awake()
        {
            Debug.Assert(sprites.Length == 4, "FridgeCleanupMiniGame needs exactly 4 Sprites");
            Debug.Assert(videoClips.Length == 4, "FridgeCleanupMiniGame needs exactly 4 VideoClips");
            Debug.Assert(audioClips.Length == 10, "FridgeCleanupMiniGame needs exactly 10 AudioClips");

            List<Food> groceriesTmp = new List<Food>(FindObjectsOfType<Food>(true));

            int id = 0;

            foreach (Food food in groceriesTmp)
            {
                food.id = id;
                groceries.Add(id, food);
                id++;
            }

            tools = new List<Tool>(FindObjectsOfType<Tool>(true));

            stores = new List<Store>(FindObjectsOfType<Store>(true));

            List<Packaging> packagingsTmp = new List<Packaging>(FindObjectsOfType<Packaging>(true));

            id = 0;

            foreach (Packaging packaging in packagingsTmp)
            {
                packaging.id = id;
                packagings.Add(id, packaging);
                id++;
            }

            List<PackagingBundle> packagingsBundleTmp = new List<PackagingBundle>(FindObjectsOfType<PackagingBundle>(true));

            id = 0;

            foreach (PackagingBundle packagingBundle in packagingsBundleTmp)
            {
                packagingBundle.id = id;
                packagingBundles.Add(id, packagingBundle);
                id++;
            }

            trashContainers = new List<TrashContainer>(FindObjectsOfType<TrashContainer>(true));
        }

        public override void AbortMiniGame()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ToolUsed, "OnToolUsed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.StoreUsed, "OnStoreUsed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.TrashContainerUsed, "OnTrashContainerUsed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
        }

        public override void ExitMiniGame()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ToolUsed, "OnToolUsed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.StoreUsed, "OnStoreUsed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.TrashContainerUsed, "OnTrashContainerUsed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
        }

        public override void StartMiniGame()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestAbortWindow, "OnRequestAbortWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.RequestScoreWindow, "OnRequestScoreWindow");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ToolUsed, "OnToolUsed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.StoreUsed, "OnStoreUsed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.TrashContainerUsed, "OnTrashContainerUsed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");

            foreach (TrashContainer trashContainer in trashContainers)
            {
                trashContainer.gameObject.SetActive(false);
            }

            foreach (Packaging packaging in packagings.Values)
            {
                packaging.gameObject.SetActive(false);
            }

            foreach (PackagingBundle packagingBundle in packagingBundles.Values)
            {
                packagingBundle.gameObject.SetActive(false);
            }

            foreach (Tool tool in tools)
            {
                tool.gameObject.SetActive(true);
            }

            foreach (Store store in stores)
            {
                store.gameObject.SetActive(true);
            }

            foreach (Food food in groceries.Values)
            {
                food.gameObject.SetActive(true);
                foreach (Tool tool in tools)
                {
                    switch (tool.toolType)
                    {
                        case Tool.ToolType.See:
                            if (!food.seen)
                            {
                                Canvas canvas = tool.GetComponentInChildren<Canvas>(true);
                                food.seenCheckmark = Instantiate(checkmarkPrefab, canvas.transform);
                                food.seenCheckmark.transform.localPosition = Vector3.zero;
                                food.seenCheckmark.SetActive(false);
                            }
                            break;
                        case Tool.ToolType.Smell:
                            if (!food.smelled)
                            {
                                Canvas canvas = tool.GetComponentInChildren<Canvas>(true);
                                food.smelledCheckmark = Instantiate(checkmarkPrefab, canvas.transform);
                                food.smelledCheckmark.transform.localPosition = Vector3.zero;
                                food.smelledCheckmark.SetActive(false);
                            }
                            break;
                        case Tool.ToolType.Taste:
                            if (!food.tasted)
                            {
                                Canvas canvas = tool.GetComponentInChildren<Canvas>(true);
                                food.tastedCheckmark = Instantiate(checkmarkPrefab, canvas.transform);
                                food.tastedCheckmark.transform.localPosition = Vector3.zero;
                                food.tastedCheckmark.SetActive(false);
                            }
                            break;
                    }
                }
            }

            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[0], audioClips[0]));
        }

        private IEnumerator RequestPromptCoroutine(string text = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);
            popUpActive = true;
            RequestPromptWindow(text, buttonTime, sprite, videoClip, audioClip);
        }

        private IEnumerator RequestFullScreenPromptPanelCoroutine(string text = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            popUpActive = true;
            return FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanelCoroutine(text, buttonTime, sprite, videoClip, audioClip);
        }

        public void CheckGroceries()
        {
            foreach (Food food in groceries.Values)
            {
                if (food.gameObject.activeSelf)
                {
                    return;
                }
            }

            openEndPopUp = true;
        }

        public void CheckPackagings()
        {
            foreach (Packaging packaging in packagings.Values)
            {
                if (packaging.gameObject.activeSelf)
                {
                    return;
                }
            }

            bool setNextStepOnClose = true;
            foreach (PackagingBundle packagingBundle in packagingBundles.Values)
            {
                if (packagingBundle.gameObject.activeSelf)
                {
                    setNextStepOnClose = false;
                }
                else if (!packagingBundle.finished)
                {
                    setNextStepOnClose = false;
                    packagingBundle.gameObject.SetActive(true);
                }
            }

            if (setNextStepOnClose)
            {
                nextStepOnClose = true;
            }
        }

        public void OnTrashContainerUsed(int id, TrashContainer.TrashContainerType trashContainerType)
        {
            Packaging packaging = packagings[id];
            Sprite sprite;
            AudioClip audioClip;
            if (trashContainerType == packaging.correctTrashContainerType)
            {
                sprite = sprites[2];
                audioClip = packaging.successAudio;
                packaging.gameObject.SetActive(false);
                CheckPackagings();
            }
            else
            {
                sprite = sprites[3];
                audioClip = audioClips[9];
            }
            popUpActive = true;
            windowText = "";
            RequestPromptWindow("", 0.0f, sprite, null, audioClip);
        }

        public void OnStoreUsed(int id, Store.StoreType storeType)
        {
            Food food = groceries[id];

            AudioClip audioClip = null;
            if (food.seen && food.smelled && food.tasted)
            {
                if (storeType == food.correctStoreType)
                {
                    if (storeType == Store.StoreType.Trash)
                    {
                        trashCount++;
                        if (trashCount == 1)
                        {
                            oneTrashPopUp = true;
                        }
                    }
                    food.gameObject.SetActive(false);
                    CheckGroceries();
                }

                switch (storeType)
                {
                    case Store.StoreType.Fridge:
                        audioClip = food.fridgeAudio;
                        break;
                    case Store.StoreType.Storeroom:
                        audioClip = food.storeroomAudio;
                        break;
                    case Store.StoreType.Trash:
                        audioClip = food.trashAudio;
                        break;
                }
            }
            else
            {
                audioClip = audioClips[2];
            }

            popUpActive = true;
            windowText = "";
            RequestPromptWindow("", 0.0f, food.seeSprite, null, audioClip);
        }

        public void OnToolUsed(int id, Tool.ToolType toolType)
        {
            Food food = groceries[id];
            Sprite sprite = food.seeSprite;
            AudioClip audioClip = null;
            popUpActive = true;
            switch (toolType)
            {
                case Tool.ToolType.See:
                    if (!food.seen)
                    {
                        food.seen = true;
                        if (food.seenCheckmark)
                        {
                            Toggle checkmarkToggle = food.seenCheckmark.GetComponentInChildren<Toggle>(true);
                            checkmarkToggle.isOn = true;
                        }
                    }
                    windowText = string.Format(bestBeforeText, System.DateTime.Now.Date.AddDays(food.bestBeforeDays).AddMonths(food.bestBeforeMonths).ToString("dd.MM.yyyy"));
                    audioClip = food.seeAudio;
                    break;
                case Tool.ToolType.Smell:
                    windowText = "";
                    if (food.seen)
                    {
                        if (!food.smelled)
                        {
                            food.smelled = true;
                            if (food.smelledCheckmark)
                            {
                                Toggle checkmarkToggle = food.smelledCheckmark.GetComponentInChildren<Toggle>(true);
                                checkmarkToggle.isOn = true;
                            }
                        }
                        audioClip = food.smellAudio;
                    }
                    else
                    {
                        audioClip = audioClips[3];
                    }
                    break;
                case Tool.ToolType.Taste:
                    windowText = "";
                    if (food.seen && food.smelled)
                    {
                        if (!food.tasted)
                        {
                            food.tasted = true;
                            if (food.tastedCheckmark)
                            {
                                Toggle checkmarkToggle = food.tastedCheckmark.GetComponentInChildren<Toggle>(true);
                                checkmarkToggle.isOn = true;
                            }
                        }
                        audioClip = food.tasteAudio;
                    }
                    else if (!food.seen)
                    {
                        audioClip = audioClips[3];
                    }
                    else
                    {
                        audioClip = audioClips[4];
                    }
                    break;
            }
            RequestPromptWindow("", 0.0f, sprite, null, audioClip);
        }

        public void OnPromptWindowClosed()
        {
            popUpActive = false;
            if (oneTrashPopUp && !abortWindowOpen)
            {
                oneTrashPopUp = false;
                StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[0], null, audioClips[5]));
            }
            else if (openEndPopUp && !abortWindowOpen)
            {
                openEndPopUp = false;
                nextStepOnClose = true;
                StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[1], null, audioClips[6]));
            }
            else if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            popUpActive = false;
            if (step == 0)
            {
                if (fridgeTutorialSubstep < 1)
                {
                    StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[1], audioClips[1]));
                    fridgeTutorialSubstep++;
                }
            }
            else
            {
                if (trashTutorialSubstep < 1)
                {
                    StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[3], audioClips[8]));
                    trashTutorialSubstep++;
                }
            }
        }

        public void OnRequestAbortWindow()
        {
            abortWindowOpen = true;
        }

        public void OnRequestScoreWindow()
        {
            scoreWindowOpen = true;
        }

        public void OnMiniGameResumed()
        {
            abortWindowOpen = false;
            if (!scoreWindowOpen && nextStepOnClose)
            {
                OnNextStep();
            }
            else if (oneTrashPopUp)
            {
                oneTrashPopUp = false;
                StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[0], null, audioClips[5]));
            }
            else if (openEndPopUp)
            {
                openEndPopUp = false;
                nextStepOnClose = true;
                StartCoroutine(RequestPromptCoroutine("", 0.0f, sprites[1], null, audioClips[6]));
            }
            else
            {
                nextStepOnClose = false;
                switch (step)
                {
                    case 0:
                        if (fridgeTutorialSubstep < 1)
                        {
                            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[0], audioClips[0]));
                        }
                        else
                        {
                            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[1], audioClips[1]));
                        }
                        break;
                    case 1:
                        if (scoreWindowOpen)
                        {
                            foreach (Packaging packaging in packagings.Values)
                            {
                                packaging.gameObject.SetActive(false);
                            }

                            foreach (PackagingBundle packagingBundle in packagingBundles.Values)
                            {
                                packagingBundle.finished = false;
                                packagingBundle.gameObject.SetActive(true);
                            }
                        }
                        if (trashTutorialSubstep < 1)
                        {
                            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[2], audioClips[7]));
                        }
                        else
                        {
                            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[3], audioClips[8]));
                        }
                        break;
                }
                scoreWindowOpen = false;
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            openEndPopUp = false;

            if (scoreWindowOpen)
            {
                nextStepOnClose = false;
            }

            if (miniGameConfig.steps.Length <= step + 1)
            {
                nextStepOnClose = false;
                step = miniGameConfig.steps.Length - 1;
                RequestScoreWindow();
            }
            else if (!abortWindowOpen)
            {
                nextStepOnClose = false;
                switch (step)
                {
                    case 0:
                        foreach (Tool tool in tools)
                        {
                            tool.gameObject.SetActive(false);
                        }

                        foreach (Store store in stores)
                        {
                            store.gameObject.SetActive(false);
                        }

                        foreach (Food food in groceries.Values)
                        {
                            food.gameObject.SetActive(false);
                        }

                        foreach (TrashContainer trashContainer in trashContainers)
                        {
                            trashContainer.gameObject.SetActive(true);
                        }

                        foreach (Packaging packaging in packagings.Values)
                        {
                            packaging.gameObject.SetActive(false);
                        }

                        foreach (PackagingBundle packagingBundle in packagingBundles.Values)
                        {
                            packagingBundle.finished = false;
                            packagingBundle.gameObject.SetActive(true);
                        }

                        if (trashTutorialSubstep < 1)
                        {
                            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[2], audioClips[7]));
                        }
                        else
                        {
                            StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[3], audioClips[8]));
                        }
                        break;
                }
                step++;
            }
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            nextStepOnClose = false;
            openEndPopUp = false;

            if (!abortWindowOpen)
            {
                step--;
                if (step < 0)
                {
                    foreach (Food food in groceries.Values)
                    {
                        food.gameObject.SetActive(true);
                        food.Reset();
                    }
                    trashCount = 0;
                    oneTrashPopUp = false;
                    step = 0;
                    RequestStartWindow();
                }
                else
                {
                    switch (step)
                    {
                        case 0:

                            foreach (TrashContainer trashContainer in trashContainers)
                            {
                                trashContainer.gameObject.SetActive(false);
                            }

                            foreach (Packaging packaging in packagings.Values)
                            {
                                packaging.gameObject.SetActive(false);
                            }

                            foreach (PackagingBundle packagingBundle in packagingBundles.Values)
                            {
                                packagingBundle.finished = false;
                                packagingBundle.gameObject.SetActive(false);
                            }

                            foreach (Tool tool in tools)
                            {
                                tool.gameObject.SetActive(true);
                            }

                            foreach (Store store in stores)
                            {
                                store.gameObject.SetActive(true);
                            }

                            foreach (Food food in groceries.Values)
                            {
                                food.gameObject.SetActive(true);
                                food.Reset();
                            }

                            trashCount = 0;
                            oneTrashPopUp = false;

                            if (fridgeTutorialSubstep < 1)
                            {
                                StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[0], audioClips[0]));
                            }
                            else
                            {
                                StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[1], audioClips[1]));
                            }
                            break;
                    }
                }
            }
        }

        public void OnReplayInstruction()
        {
            if (!popUpActive)
            {
                switch (step)
                {
                    case 0:
                        fridgeTutorialSubstep = 0;
                        StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[0], audioClips[0]));
                        break;
                    case 1:
                        trashTutorialSubstep = 0;
                        StartCoroutine(RequestFullScreenPromptPanelCoroutine("", 0.0f, null, videoClips[2], audioClips[7]));
                        break;
                }
            }
        }
    }
}
