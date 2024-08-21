namespace MiniGame.UI
{
    public class MiniGameQuestionUI : AMiniGameWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (baseMiniGameUIController is MiniGameUIController)
            {
                mainButton.onClick.AddListener((baseMiniGameUIController as MiniGameUIController).OnQuestionWindowYes);
                exitButton.onClick.AddListener((baseMiniGameUIController as MiniGameUIController).OnQuestionWindowNo);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                mainButton.onClick.AddListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnQuestionWindowYes);
                exitButton.onClick.AddListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnQuestionWindowNo);
            }
            else
            {
                mainButton.onClick.AddListener(baseMiniGameUIController.OnQuestionWindowYes);
                exitButton.onClick.AddListener(baseMiniGameUIController.OnQuestionWindowNo);
            }

            replayButton.onClick.AddListener(baseMiniGameUIController.OnReplayInstruction);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!answered)
            {
                answered = true;
                EventBus.Publish(EventId.MiniGameEvents.QuestionWindowNo);
            }

            if (baseMiniGameUIController is MiniGameUIController)
            {
                mainButton.onClick.RemoveListener((baseMiniGameUIController as MiniGameUIController).OnQuestionWindowYes);
                exitButton.onClick.RemoveListener((baseMiniGameUIController as MiniGameUIController).OnQuestionWindowNo);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                mainButton.onClick.RemoveListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnQuestionWindowYes);
                exitButton.onClick.RemoveListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnQuestionWindowNo);
            }
            else
            {
                mainButton.onClick.RemoveListener(baseMiniGameUIController.OnQuestionWindowYes);
                exitButton.onClick.RemoveListener(baseMiniGameUIController.OnQuestionWindowNo);
            }

            replayButton.onClick.RemoveListener(baseMiniGameUIController.OnReplayInstruction);
        }
    }
}