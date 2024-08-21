using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageMarkerDetector : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    private Dictionary<TrackingState, List<string>> imagesByTrackingState;

    private void Awake()
    {
        if (!imageManager && !(imageManager = FindObjectOfType<ARTrackedImageManager>()))
        {
            Debug.LogError("ImageMarkerDetector is missing a reference to an ARTrackedImageManager");
        }
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        imagesByTrackingState = new Dictionary<TrackingState, List<string>>
        {
            { TrackingState.None, new List<string>() },
            { TrackingState.Limited, new List<string>() },
            { TrackingState.Tracking, new List<string>() }
        };
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        for (int i = 0; i < args.updated.Count; i++)
        {
            List<string> trackedImagesNames = imagesByTrackingState[args.updated[i].trackingState];
            if (trackedImagesNames.Contains(args.updated[i].referenceImage.name))
            {
                if (args.updated[i].trackingState == TrackingState.Tracking)
                {
                    ImageMarkerUpdate(args.updated[i]);
                }
            }
            else
            {
                TrackingState previousTrackingState = GetPreviousTrackingState(args.updated[i]);
                switch (args.updated[i].trackingState)
                {
                    case TrackingState.None:
                        switch (previousTrackingState)
                        {
                            case TrackingState.Limited:
                            case TrackingState.Tracking:
                                ImageMarkerLost(args.updated[i]);
                                break;
                        }
                        break;
                    case TrackingState.Limited:
                        switch (previousTrackingState)
                        {
                            case TrackingState.None:
                            case TrackingState.Tracking:
                                ImageMarkerLost(args.updated[i]);
                                break;
                        }
                        ImageMarkerUpdate(args.updated[i]);
                        break;
                    case TrackingState.Tracking:
                        switch (previousTrackingState)
                        {
                            case TrackingState.None:
                            case TrackingState.Limited:
                                ImageMarkerDetected(args.updated[i]);
                                break;

                        }
                        ImageMarkerUpdate(args.updated[i]);
                        break;
                }
                imagesByTrackingState[(TrackingState)((((int)args.updated[i].trackingState) + 1) % 3)].Remove(args.updated[i].referenceImage.name);
                imagesByTrackingState[(TrackingState)((((int)args.updated[i].trackingState) + 2) % 3)].Remove(args.updated[i].referenceImage.name);
                trackedImagesNames.Add(args.updated[i].referenceImage.name);
            }
        }
    }

    private TrackingState GetPreviousTrackingState(ARTrackedImage image)
    {
        TrackingState previousTrackingState = TrackingState.None;
        for (int i = 1; i <= 2; i++)
        {
            TrackingState other = (TrackingState)((((int)image.trackingState) + i) % 3);
            previousTrackingState = imagesByTrackingState[other].Contains(image.referenceImage.name) ? other : previousTrackingState;
        }
        return previousTrackingState;
    }

    private void ImageMarkerDetected(ARTrackedImage image)
    {
        gameObject.BroadcastMessage("OnImageMarkerDetected", image);
    }

    private void ImageMarkerLost(ARTrackedImage image)
    {
        gameObject.BroadcastMessage("OnImageMarkerLost", image);
    }

    private void ImageMarkerUpdate(ARTrackedImage image)
    {
        gameObject.BroadcastMessage("OnImageMarkerUpdate", image);
    }

    private void OnDestroy()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
}
