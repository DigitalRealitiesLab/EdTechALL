namespace MiniGame.UI
{
    public class MiniGameStartUI : AMiniGameWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (baseMiniGameUIController is MiniGameUIController)
            {
                mainButton.onClick.AddListener((baseMiniGameUIController as MiniGameUIController).OnStartConfirm);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                mainButton.onClick.AddListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnStartConfirm);
            }
            else
            {
                mainButton.onClick.AddListener(baseMiniGameUIController.OnStartConfirm);
            }

            exitButton.onClick.AddListener(baseMiniGameUIController.OnStartAbort);
            replayButton.onClick.AddListener(baseMiniGameUIController.OnReplayInstruction);
        }

        public override void LoadMiniGameConfig(MiniGameConfig config)
        {
            base.LoadMiniGameConfig(config);
            miniGameDescriptionText.text = config.description;      
        }

        protected override void OnDisable()
        {
            base.OnDisable();


            if (baseMiniGameUIController is MiniGameUIController)
            {
                mainButton.onClick.RemoveListener((baseMiniGameUIController as MiniGameUIController).OnStartConfirm);
            }
            else if (baseMiniGameUIController is VirtualTourMiniGameUIController)
            {
                mainButton.onClick.RemoveListener((baseMiniGameUIController as VirtualTourMiniGameUIController).OnStartConfirm);
            }
            else
            {
                mainButton.onClick.RemoveListener(baseMiniGameUIController.OnStartConfirm);
            }

            exitButton.onClick.RemoveListener(baseMiniGameUIController.OnStartAbort);
            replayButton.onClick.RemoveListener(baseMiniGameUIController.OnReplayInstruction);
        }
    }
}