using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;

namespace MiniGame.DairyTour
{
    public class QualityControl2 : MonoBehaviour
    {
        public Image milkImage;
        public Image magnifyingGlassFatImage;
        public Image magnifyingGlassBacteriaImage;
        public Texture[] magnifyingGlassTextures;
        public Button[] activeButtons;
        public Sprite[] sprites;
        public VideoClip[] videoClips;
        public AudioClip[] audioClips;
        public AudioSource audioSource;
        public RectTransform counterTransform, fridgeTransform, trashcanTransform;

        private const int numPacks = 6;
        private int numGood = 0;
        private int numBad = 0;
        private int currentPack = 0;
        private int playNext = -1;
        private bool inactive = false;
        private bool finished = false;
        private List<int> packs = new List<int>();
        private List<Image> packCounterImages = new List<Image>();
        private Sprite currentSprite = null;

        private void Awake()
        {
            Debug.Assert(milkImage, "QualityControl2 is missing a reference to an Image");
            Debug.Assert(magnifyingGlassFatImage, "QualityControl2 is missing a reference to an Image");
            Debug.Assert(magnifyingGlassBacteriaImage, "QualityControl2 is missing a reference to an Image");
            Debug.Assert(magnifyingGlassTextures.Length == 6, "QualityControl2 needs 6 Textures");
            Debug.Assert(sprites.Length == 4, "QualityControl2 needs exactly 4 Sprites");
            Debug.Assert(videoClips.Length == 1, "QualityControl2 needs exactly 1 VideoClip");
            Debug.Assert(audioClips.Length == 10, "QualityControl2 needs exactly 10 AudioClips");
            Debug.Assert(audioSource, "QualityControl2 is missing a reference to an AudioSource");
            Debug.Assert(counterTransform, "QualityControl2 is missing a reference to an RectTransform");
            Debug.Assert(fridgeTransform, "QualityControl2 is missing a reference to an RectTransform");
            Debug.Assert(trashcanTransform, "QualityControl2 is missing a reference to an RectTransform");

            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");

            for (int i = 0; i < numPacks; i++)
            {
                int pack = Random.Range(0, numPacks);

                while (packs.Contains(pack))
                {
                    pack = Random.Range(0, numPacks);
                }

                packs.Add(pack);
            }

            foreach (int pack in packs)
            {
                GameObject newPack = Instantiate(counterTransform.gameObject, counterTransform.parent);
                counterTransform.anchoredPosition += new Vector2(0.0f, 135.0f);
                Image newPackImage = newPack.GetComponent<Image>();
                switch (pack)
                {
                    case 0:
                    case 3:
                        newPackImage.sprite = sprites[0];
                        break;
                    case 1:
                    case 4:
                        newPackImage.sprite = sprites[1];
                        break;
                    case 2:
                    case 5:
                        newPackImage.sprite = sprites[2];
                        break;
                }
                packCounterImages.Add(newPackImage);
            }

            LoadPack();

            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClips[0], audioClips[0]);
        }

        private void Update()
        {
            if (playNext >= 0 && !audioSource.isPlaying)
            {
                audioSource.clip = audioClips[playNext];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                if (finished && playNext != 9)
                {
                    playNext = 9;
                }
                else
                {
                    playNext = -1;
                }
            }
            else if (playNext < 0 && !audioSource.isPlaying && inactive)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                inactive = false;
                foreach (Button button in activeButtons)
                {
                    button.gameObject.SetActive(true);
                }
                LoadPack();
            }
            else if (playNext < 0 && !audioSource.isPlaying && finished)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, false));
                string sendName = gameObject.name;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.SubMiniGameEnded, sendName));
            }
        }

        public void OnOKButtonpClicked()
        {
            foreach (Button button in activeButtons)
            {
                button.gameObject.SetActive(false);
            }

            if (packs[currentPack] > 2)
            {
                audioSource.clip = audioClips[2];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                inactive = true;
            }
            else
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                playNext = 3 + packs[currentPack];
                inactive = true;
                NextPack(true);
            }
        }

        public void OnNotOKButtonpClicked()
        {
            foreach (Button button in activeButtons)
            {
                button.gameObject.SetActive(false);
            }

            if (packs[currentPack] <= 2)
            {
                audioSource.clip = audioClips[2];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                inactive = true;
            }
            else
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.EnableSkipAudioButton, true));
                playNext = 3 + packs[currentPack];
                inactive = true;
                NextPack(false);
            }
        }

        public void OnSkipAudioButtonPressed()
        {
            audioSource.Stop();
        }

        private void NextPack(bool good)
        {
            packCounterImages[currentPack].sprite = currentSprite;
            RectTransform counterImageRectTransform = packCounterImages[currentPack].GetComponent<RectTransform>();

            if (good)
            {
                counterImageRectTransform.anchoredPosition = fridgeTransform.anchoredPosition + new Vector2(0.0f, -150.0f - 100.0f * numGood);
                numGood++;
            }
            else
            {
                counterImageRectTransform.anchoredPosition = trashcanTransform.anchoredPosition + new Vector2(0.0f, -150.0f - 100.0f * numBad);
                numBad++;
            }

            currentPack++;
            if (currentPack >= numPacks)
            {
                finished = true;
            }

        }

        private void LoadPack()
        {
            if (currentPack < packs.Count)
            {
                if (packCounterImages[currentPack].sprite != sprites[3])
                {
                    currentSprite = packCounterImages[currentPack].sprite;
                    packCounterImages[currentPack].sprite = sprites[3];
                }
                switch (packs[currentPack])
                {
                    case 0:
                        milkImage.sprite = sprites[0];
                        magnifyingGlassFatImage.material.mainTexture = magnifyingGlassTextures[0];
                        magnifyingGlassBacteriaImage.material.mainTexture = magnifyingGlassTextures[2];
                        break;
                    case 1:
                        milkImage.sprite = sprites[1];
                        magnifyingGlassFatImage.material.mainTexture = magnifyingGlassTextures[0];
                        magnifyingGlassBacteriaImage.material.mainTexture = magnifyingGlassTextures[3];
                        break;
                    case 2:
                        milkImage.sprite = sprites[2];
                        magnifyingGlassFatImage.material.mainTexture = magnifyingGlassTextures[0];
                        magnifyingGlassBacteriaImage.material.mainTexture = magnifyingGlassTextures[4];
                        break;
                    case 3:
                        milkImage.sprite = sprites[0];
                        magnifyingGlassFatImage.material.mainTexture = magnifyingGlassTextures[0];
                        magnifyingGlassBacteriaImage.material.mainTexture = magnifyingGlassTextures[5];
                        break;
                    case 4:
                        milkImage.sprite = sprites[1];
                        magnifyingGlassFatImage.material.mainTexture = magnifyingGlassTextures[1];
                        magnifyingGlassBacteriaImage.material.mainTexture = magnifyingGlassTextures[5];
                        break;
                    case 5:
                        milkImage.sprite = sprites[2];
                        magnifyingGlassFatImage.material.mainTexture = magnifyingGlassTextures[1];
                        magnifyingGlassBacteriaImage.material.mainTexture = magnifyingGlassTextures[4];
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SkipAudioButtonPressed, "OnSkipAudioButtonPressed");
        }
    }
}
