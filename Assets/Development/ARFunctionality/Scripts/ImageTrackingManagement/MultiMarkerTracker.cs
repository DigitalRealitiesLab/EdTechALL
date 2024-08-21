using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultiMarkerTracker : MonoBehaviour
{
    public GameObject prefab;
    public string imageName = "HandMapMarker";
    public bool hasInstance { get { return instance; } }

    private ARSession _ARSession;

    private Dictionary<int, List<MultiMarkerImage>> multiMarkerImages;
    private MultiMarkerImage[] newMultiMarkerImages;
    private List<Vector3> positions = new List<Vector3>();
    private List<float> positionWeights = new List<float>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private List<float> rotationWeights = new List<float>();
    private GameObject instance = null;
    private float bestWeight = -1.0f;

    private const float maxMovementThresholdHandMap = 0.2f;
    private const float maxMovementThresholdFloorMap = 2.0f;
    private const float maxPositionThresholdHandMap = 0.1f;
    private const float maxPositionThresholdFloorMap = 1.0f;
    private const float minWeightThreshold = 0.2f;
    private const float inactiveMarkerMaxWeight = 0.0001f;

    private Vector3 lastPosition = Vector3.zero;

    private float resetTimer = 1.0f;
    private float resetTime = 0.0f;

    private bool sessionTracking = false;
    private bool positionInitialized = false;

    private void Awake()
    {
        multiMarkerImages = new Dictionary<int, List<MultiMarkerImage>>();
        _ARSession = FindObjectOfType<ARSession>(true);
        sessionTracking = ARSession.state == ARSessionState.SessionTracking;

        ARSession.stateChanged += OnStateChanged;
    }

    private void Update()
    {
        if (sessionTracking)
        {
            ResetMultiMarkerImages();
        }
    }

    private void OnStateChanged(ARSessionStateChangedEventArgs obj)
    {
        sessionTracking = obj.state == ARSessionState.SessionTracking;
    }

    public void ResetMultiMarkerImages()
    {
        bestWeight = -1.0f;
        multiMarkerImages.Clear();
        newMultiMarkerImages = FindObjectsOfType<MultiMarkerImage>(true);

        for (int i = 0; i < newMultiMarkerImages.Length; i++)
        {
            if (newMultiMarkerImages[i].imageName.Contains(imageName))
            {
                if (!multiMarkerImages.ContainsKey(newMultiMarkerImages[i].imageCountSqrt))
                {
                    multiMarkerImages.Add(newMultiMarkerImages[i].imageCountSqrt, new List<MultiMarkerImage>());
                }
                multiMarkerImages[newMultiMarkerImages[i].imageCountSqrt].Add(newMultiMarkerImages[i]);

                if (!newMultiMarkerImages[i].gameObject.activeSelf && newMultiMarkerImages[i].weight > inactiveMarkerMaxWeight)
                {
                    newMultiMarkerImages[i].weight = inactiveMarkerMaxWeight;
                }

                if (newMultiMarkerImages[i].weight > bestWeight)
                {
                    bestWeight = newMultiMarkerImages[i].weight;
                }
            }
        }

        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.SendBestMarkerWeight, bestWeight));

        if (multiMarkerImages.Count > 0 && (bestWeight > minWeightThreshold || !positionInitialized))
        {
            positionInitialized = true;
            foreach (int imageCountSqrt in multiMarkerImages.Keys)
            {
                if (!instance)
                {
                    instance = Instantiate(prefab, FindObjectOfType<ReferenceImageLibraryManager>().transform);
                }

                positions.Clear(); ;
                positionWeights.Clear();
                rotations.Clear();
                rotationWeights.Clear();
                Vector3 scale = Vector3.zero;

                Vector3 highestWeightPosition = Vector3.zero;
                float highestWeight = 0;

                for (int i = 0; i < multiMarkerImages[imageCountSqrt].Count; i++)
                {
                    Vector3 processedOffset = new Vector3(multiMarkerImages[imageCountSqrt][i].offset.x * multiMarkerImages[imageCountSqrt][i].transform.localScale.x, 0.0f,
                        multiMarkerImages[imageCountSqrt][i].offset.z * multiMarkerImages[imageCountSqrt][i].transform.localScale.z);
                    processedOffset = multiMarkerImages[imageCountSqrt][i].transform.localRotation * (processedOffset / (2.0f - (imageCountSqrt % 2)));
                    positions.Add(multiMarkerImages[imageCountSqrt][i].transform.localPosition + processedOffset);
                    rotations.Add(multiMarkerImages[imageCountSqrt][i].transform.localRotation);
                    rotationWeights.Add(multiMarkerImages[imageCountSqrt][i].weight);
                    positionWeights.Add(multiMarkerImages[imageCountSqrt][i].weight);
                    scale += multiMarkerImages[imageCountSqrt][i].transform.localScale;
                    if (multiMarkerImages[imageCountSqrt][i].weight > highestWeight)
                    {
                        highestWeight = multiMarkerImages[imageCountSqrt][i].weight;
                        highestWeightPosition = multiMarkerImages[imageCountSqrt][i].transform.localPosition + processedOffset;
                    }
                }

                Tuple<Quaternion, float> averageRotation = AverageRotation(rotations, rotationWeights);
                Vector3 newPosition = Vector3.zero;

                if (averageRotation.Item2 != 0.0f)
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        newPosition += positions[i] * positionWeights[i];
                    }
                    newPosition /= averageRotation.Item2;
                }
                else
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        newPosition += positions[i];
                    }
                    newPosition /= positions.Count;
                }

                if (lastPosition == Vector3.zero)
                {
                    lastPosition = newPosition;
                }

                instance.transform.localPosition = newPosition;
                instance.transform.localRotation = averageRotation.Item1;
                instance.transform.localScale = scale / multiMarkerImages[imageCountSqrt].Count * imageCountSqrt;

                if (_ARSession)
                {
                    if (imageName == "HandMapMarker")
                    {
                        if ((newPosition - highestWeightPosition).magnitude > maxPositionThresholdHandMap || (newPosition - lastPosition).magnitude > maxMovementThresholdHandMap)
                        {
                            if (resetTime >= resetTimer)
                            {
                                lastPosition = Vector3.zero;
                                resetTime = 0.0f;
                                positionInitialized = false;
                                for (int i = 0; i < newMultiMarkerImages.Length; i++)
                                {
                                    newMultiMarkerImages[i].gameObject.SetActive(false);
                                }
                                _ARSession.Reset();
                            }
                            else
                            {
                                resetTime += Time.deltaTime;
                            }
                        }
                        else
                        {
                            resetTime = 0.0f;
                            lastPosition = newPosition;
                        }
                    }
                    else
                    {
                        if ((newPosition - highestWeightPosition).magnitude > maxPositionThresholdFloorMap || (newPosition - lastPosition).magnitude > maxMovementThresholdFloorMap)
                        {
                            if (resetTime >= resetTimer)
                            {
                                lastPosition = Vector3.zero;
                                resetTime = 0.0f;
                                positionInitialized = false;
                                for (int i = 0; i < newMultiMarkerImages.Length; i++)
                                {
                                    newMultiMarkerImages[i].gameObject.SetActive(false);
                                }
                                _ARSession.Reset();
                            }
                            else
                            {
                                resetTime += Time.deltaTime;
                            }
                        }
                        else
                        {
                            resetTime = 0.0f;
                            lastPosition = newPosition;
                        }
                    }
                }
            }
        }
    }

    Tuple<Quaternion, float> AverageRotation(List<Quaternion> rotations, List<float> rotationWeights)
    {
        while (rotations.Count > 1)
        {
            for (int i = 0; i + 1 < rotations.Count; i++)
            {
                if (rotationWeights[i] == 0.0f && rotationWeights[i + 1] == 0.0f)
                {
                    rotations[i] = Quaternion.Lerp(rotations[i], rotations[i + 1], 0.5f);
                    rotationWeights[i] = 0.0f;
                }
                else if (rotationWeights[i] == 0.0f)
                {
                    rotations[i] = rotations[i + 1];
                    rotationWeights[i] = rotationWeights[i + 1];
                }
                else if (rotationWeights[i] < rotationWeights[i + 1])
                {
                    rotations[i] = Quaternion.Lerp(rotations[i + 1], rotations[i], rotationWeights[i] / rotationWeights[i + 1]);
                    rotationWeights[i] = rotationWeights[i] + rotationWeights[i + 1];
                }
                else if (rotationWeights[i + 1] != 0.0f)
                {
                    rotations[i] = Quaternion.Lerp(rotations[i], rotations[i + 1], rotationWeights[i + 1] / rotationWeights[i]);
                    rotationWeights[i] = rotationWeights[i] + rotationWeights[i + 1];
                }
                rotations.RemoveAt(i + 1);
                rotationWeights.RemoveAt(i + 1);
            }
        }

        return new Tuple<Quaternion, float>(rotations[0], rotationWeights[0]);
    }

    private void OnDestroy()
    {
        ARSession.stateChanged -= OnStateChanged;
    }
}