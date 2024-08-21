using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace MiniGame
{
    public abstract class AMiniGame : MonoBehaviour
    {
        public MiniGameConfig miniGameConfig { get; set; }

        [HideInInspector]
        public string windowText = "";

        public void RequestScoreWindow()
        {
            EventBus.Publish(EventId.MiniGameEvents.RequestScoreWindow);
        }

        public void RequestStartWindow()
        {
            StartCoroutine(RequestStartWindowDelayed());
        }

        public IEnumerator RequestStartWindowDelayed()
        {
            yield return new WaitForSeconds(0.1f);

            EventBus.Publish(EventId.MiniGameEvents.RequestStartWindow, false);
        }

        public void RequestPromptWindow(string text = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            EventBus.Publish(EventId.MiniGameEvents.RequestPromptWindow, text, buttonTime, sprite, videoClip, audioClip);
        }

        public void RequestQuestionWindow(string text = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            EventBus.Publish(EventId.MiniGameEvents.RequestQuestionWindow, text, buttonTime, sprite, videoClip, audioClip);
        }

        public void RequestDiaryWindow(string text = "", string diaryNumberText = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null,
            string promptText = "", Sprite promptSprite = null, VideoClip promptVideoClip = null, AudioClip promptAudioClip = null)
        {
            EventBus.Publish(EventId.MiniGameEvents.RequestDiaryWindow, text, diaryNumberText, buttonTime, sprite, videoClip, audioClip, promptText, promptSprite, promptVideoClip, promptAudioClip);
        }

        public abstract void StartMiniGame();
        public abstract void ExitMiniGame();
        public abstract void AbortMiniGame();
    }

    public enum MiniGameType
    {
        MunicipalitySearch,
        CountyPuzzle,
        MapLegend,
        Topography,
        FarmOverview,
        FarmTour,
        LandscapeLayers,
        VirtualAnimals,
        DairyTour,
        FridgeCleanup,
        None
    }
}