using UnityEngine;

namespace InteractionSystem.County
{
    using Interactable;
    using MiniGame.CountyPuzzle;

    public class County : Interactable
    {
        public VisualGuidanceTarget VisualGuidanceTarget;
        public MeshRenderer checkmarkRenderer;
        public CountyType countyType;
        public MeshCollider meshCollider;
        public bool success = false;
        public float threshold = 0.05f;
        public float downScale = 0.35f;
        public Vector3 startPosition;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(VisualGuidanceTarget, "County is missing a reference to a VisualGuidanceTarget");
            EventBus.SaveRegisterCallback(this, EventId.CountyPuzzleMiniGameEvents.SetCounty, "OnSetCounty");
            checkmarkRenderer.enabled = false;
            transform.localScale = new Vector3(downScale, downScale, downScale);
        }

        public void OnSetCounty(bool solved)
        {
            VisualGuidanceTarget.gameObject.SetActive(!solved);
            if (solved)
            {
                transform.localPosition = Vector3.zero;
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                success = true;
                checkmarkRenderer.enabled = true;
                meshCollider.enabled = false;
            }
            else
            {
                transform.localPosition = startPosition;
                transform.localScale = new Vector3(downScale, downScale, downScale);
                success = false;
                checkmarkRenderer.enabled = false;
                meshCollider.enabled = true;
            }
        }

        public void CheckCounty()
        {
            if (Vector3.Distance(transform.position, transform.parent.position) <= threshold)
            {
                VisualGuidanceTarget.gameObject.SetActive(false);
                transform.localPosition = Vector3.zero;
                success = true;
                checkmarkRenderer.enabled = true;
                meshCollider.enabled = false;
                EventBus.Publish(EventId.CountyPuzzleMiniGameEvents.SuccessChanged, countyType, success);
            }
            else
            {
                VisualGuidanceTarget.gameObject.SetActive(true);
                transform.localPosition = startPosition;
                transform.localScale = new Vector3(downScale, downScale, downScale);
            }
        }

        public void CheckScale()
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.CountyPuzzleMiniGameEvents.SetCounty, "OnSetCounty");
        }
    }
}
