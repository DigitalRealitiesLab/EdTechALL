using UnityEngine;

namespace InteractionSystem.MagnifyingGlass
{
    using Interactable;

    public class MagnifyingGlass : Interactable
    {
        public Material magnifyingGlassMaterial;
        public RectTransform rectTransform;

        public Vector3 startPosition = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(magnifyingGlassMaterial, "MagnifyingGlass is missing a reference to a Material");
            Debug.Assert(rectTransform, "MagnifyingGlass is missing a reference to a RectTransform");
            if (rectTransform)
            {
                startPosition = rectTransform.position;
            }
        }

        private void Update()
        {
            if (rectTransform)
            {
                magnifyingGlassMaterial.mainTextureScale = new Vector2(1.0f / (Screen.width / (rectTransform.rect.width * rectTransform.localScale.x)),
                    1.0f / (Screen.height / (rectTransform.rect.height * rectTransform.localScale.y)));
                magnifyingGlassMaterial.mainTextureOffset = new Vector2((rectTransform.position.x - rectTransform.rect.width * rectTransform.localScale.x / 2.0f) *
                    magnifyingGlassMaterial.mainTextureScale.x / (rectTransform.rect.width * rectTransform.localScale.x),
                    (rectTransform.position.y - rectTransform.rect.height * rectTransform.localScale.y / 2.0f) *
                    magnifyingGlassMaterial.mainTextureScale.y / (rectTransform.rect.height * rectTransform.localScale.y));
            }
        }

        public void CheckMagnifyingGlass()
        {
            if (rectTransform)
            {
                rectTransform.position = startPosition;
            }
        }
    }
}