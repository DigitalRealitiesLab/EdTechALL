using UnityEngine.XR.ARFoundation;

namespace MiniGame.MapLegend
{
    public class MapLegendMarkerReactionBehaviour : AImageMarkerReactionBehaviour
    {
        public ReferenceLibrarySignature exchangeSignature;
        private ReferenceLibrarySignature previousSignature;
        private bool active = false;

        private void Awake()
        {
            previousSignature = EdTechALLConfig.referenceLibrary;
        }

        public override void OnImageMarkerDetected(ARTrackedImage image)
        {
            if (active)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MapLegendMiniGameEvents.MunicipalityMarkerDetected, image));
            }
        }

        public void ActivateMarkerSearch()
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, false);
            EdTechALLConfig.referenceLibrary = exchangeSignature;
            active = true;
        }

        public void StopMarkerSearch()
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.EnableMarkerTrackingIndicator, true);
            EdTechALLConfig.referenceLibrary = previousSignature;
            active = false;
        }

        public override void OnImageMarkerLost(ARTrackedImage image) { }
        public override void OnImageMarkerUpdate(ARTrackedImage image) { }
    }
}