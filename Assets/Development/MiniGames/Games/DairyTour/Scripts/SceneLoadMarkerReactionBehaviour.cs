using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SceneLoadMarkerReactionBehaviour : AImageMarkerReactionBehaviour
{
    public MiniGameSceneLookup lookup;
    public string deleteMarkerName = "TrashMarker";

    private void Awake()
    {
        Debug.Assert(lookup, "SceneLoadMarkerReactionBehaviour is missing a reference to a MiniGameSceneLookup");
    }

    public override void OnImageMarkerDetected(ARTrackedImage image)
    {
        if (image.referenceImage.name == deleteMarkerName)
        {
            EventBus.Publish(MiniGame.EventId.RequestDeleteSaveWindow, true);
        }
        else
        {
            MiniGameSceneLoadData data;
            if (TryGetImageMarkerData(lookup, image.referenceImage.name, out data))
            {
                EventBus.Publish(MiniGame.EventId.GameModeMarker, (int)data.miniGameType);
            }
        }
    }

    public override void OnImageMarkerLost(ARTrackedImage image){}
    public override void OnImageMarkerUpdate(ARTrackedImage image){ }
}
