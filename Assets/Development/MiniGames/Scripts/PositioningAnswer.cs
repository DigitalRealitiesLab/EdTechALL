using UnityEngine;
using UnityEngine.Video;

namespace MiniGame
{
    public class PositioningAnswer : MonoBehaviour
    {
        public int identification;
        public int order;
        public bool correct = false;
        public float maxDistance = 2.0f;
        public Material selectedMaterial;
        public SpriteRenderer spriteRenderer;
        public string promptText = "";
        public Sprite sprite = null;
        public VideoClip videoClip = null;
        public AudioClip audioClip = null;
        public string wrongAnswerText = "";
        private Material startMaterial;
        private bool selected = false;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;

                if (!spriteRenderer)
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    Debug.Assert(spriteRenderer, "PositioningAnswer GameObject is missing a SpriteRenderer");
                }

                if (!startMaterial)
                {
                    startMaterial = spriteRenderer.material;
                    Debug.Assert(startMaterial, "PositioningAnswer GameObject has a SpriteRenderer without a Material");
                }

                if (selected)
                {
                    spriteRenderer.material = selectedMaterial;
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                else
                {
                    spriteRenderer.material = startMaterial;
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }

        private void Awake()
        {
            Debug.Assert(selectedMaterial, "PositioningAnswer is missing a reference to a Material");
        }
    }
}
