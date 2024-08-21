namespace MiniGame.UI
{
    public class MiniGamePromptUI : AMiniGameWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (baseMiniGameUIController is MiniGameUIController)
            {
                mainButton.onClick.AddListener((baseMiniGameUIController as MiniGameUIController).CloseActivePanel);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                mainButton.onClick.AddListener((baseMiniGameUIController as VirtualTourMiniGameUIController).CloseActivePanel);
            }
            else
            {
                mainButton.onClick.AddListener(baseMiniGameUIController.CloseActivePanel);
            }

            replayButton.onClick.AddListener(baseMiniGameUIController.OnReplayInstruction);
        }

        protected override void OnDisable()
        {
            if (baseMiniGameUIController is MiniGameUIController)
            {
                mainButton.onClick.RemoveListener((baseMiniGameUIController as MiniGameUIController).CloseActivePanel);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                mainButton.onClick.RemoveListener((baseMiniGameUIController as VirtualTourMiniGameUIController).CloseActivePanel);
            }
            else
            {
                mainButton.onClick.RemoveListener(baseMiniGameUIController.CloseActivePanel);
            }

            replayButton.onClick.RemoveListener(baseMiniGameUIController.OnReplayInstruction);
            if (baseMiniGameUIController.baseMiniGameManager.activeMiniGame)
            {
                baseMiniGameUIController.baseMiniGameManager.activeMiniGame.StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PromptWindowClosed));
            }
        }
    }
}