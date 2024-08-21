using UnityEngine;

namespace MiniGame
{
    public class HideBeforeSubMiniGame : MonoBehaviour
    {
        public string otherSubMiniGameName = "";
        public VirtualTourCheckmarkUI virtualTourCheckmarkUI;

        private bool playingSubMiniGame = false;
        private bool audioPlaying = false;
        private bool otherMinigameFinished = false;
        private bool admin = false;

        private void Awake()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SetSubMiniGame, "OnSetSubMiniGame");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");

            if (!virtualTourCheckmarkUI)
            {
                virtualTourCheckmarkUI = gameObject.GetComponentInChildren<VirtualTourCheckmarkUI>(true);
            }

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }

            gameObject.SetActive((otherMinigameFinished || admin) && !audioPlaying);
        }

        public void OnSubMiniGameStarted(InfinityCode.uPano.HotSpots.HotSpot sender)
        {
            if (virtualTourCheckmarkUI.subMiniGameName == sender.title)
            {
                playingSubMiniGame = true;
            }
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            if (subMiniGameName == otherSubMiniGameName)
            {
                otherMinigameFinished = true;
                gameObject.SetActive((otherMinigameFinished || admin) && !audioPlaying);
            }

            if (subMiniGameName == virtualTourCheckmarkUI.subMiniGameName)
            {
                playingSubMiniGame = false;

                if (admin)
                {
                    EventBus.Publish(EventId.MiniGameEvents.SetSubMiniGame, otherSubMiniGameName);
                }
            }
        }

        public void OnSubMiniGameAborted(string subMiniGameName)
        {
            if (subMiniGameName == virtualTourCheckmarkUI.subMiniGameName)
            {
                playingSubMiniGame = false;
            }
        }

        public void OnSetSubMiniGame(string subMiniGameName)
        {
            if (subMiniGameName == virtualTourCheckmarkUI.subMiniGameName)
            {
                otherMinigameFinished = true;
                gameObject.SetActive(otherMinigameFinished && !audioPlaying);
                EventBus.Publish(EventId.MiniGameEvents.SetSubMiniGame, otherSubMiniGameName);
            }

            if (subMiniGameName == otherSubMiniGameName)
            {
                otherMinigameFinished = true;
                gameObject.SetActive(otherMinigameFinished && !audioPlaying);
            }
        }

        public void OnAudioSubMiniGameIsPlaying(bool isPlaying)
        {
            audioPlaying = isPlaying;
            gameObject.SetActive((otherMinigameFinished || admin) && !audioPlaying);
        }

        public void OnQuitAdminMode()
        {
            admin = false;

            if (playingSubMiniGame)
            {
                EventBus.Publish(EventId.MiniGameEvents.SetSubMiniGame, otherSubMiniGameName);
            }

            gameObject.SetActive(otherMinigameFinished && !audioPlaying);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SetSubMiniGame, "OnSetSubMiniGame");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
        }
    }
}