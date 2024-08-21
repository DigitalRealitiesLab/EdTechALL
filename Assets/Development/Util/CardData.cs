using UnityEngine.Video;
using UnityEngine;

namespace MiniGame
{
    [CreateAssetMenu(fileName = "CardData", menuName = "CardData", order = 2)]
    public class CardData : ScriptableObject
    {
        public MiniGameConfig miniGameData;
    }

    [System.Serializable]
    public class MiniGameConfig
    {
        public MiniGameType type;
        public string title;
        public string[] steps;

        public string abortText = "";
        public string scoreText = "";
        [TextArea]
        public string description;
        public Sprite sprite;
        public VideoClip videoClip;
        public AudioClip audioClip;
        public Sprite abortSprite;
        public VideoClip abortVideoClip;
        public AudioClip abortAudioClip;
        public Sprite scoreSprite;
        public VideoClip scoreVideoClip;
        public AudioClip scoreAudioClip;
    }
}