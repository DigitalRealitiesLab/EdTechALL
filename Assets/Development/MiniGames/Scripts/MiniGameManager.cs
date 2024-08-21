namespace MiniGame
{
    public class MiniGameManager : BaseMiniGameManager
    {
        public override void StartMiniGame()
        {
            if (miniGameConfig == null)
            {
                return;
            }

            activeMiniGame = FindObjectOfType<AMiniGame>();

            activeMiniGame.miniGameConfig = miniGameConfig;
            ((UI.MiniGameUIController)baseMiniGameUIController).infoUI.LoadMiniGameConfig(miniGameConfig);

            activeMiniGame.StartMiniGame();
        }
    }
}