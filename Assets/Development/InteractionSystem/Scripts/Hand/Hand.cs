using UnityEngine;

namespace InteractionSystem.Hand
{
    using System.Collections.Generic;
    using Interactable;
    using UnityEngine.EventSystems;
    using County;
    using Food;
    using Packaging;
    using MagnifyingGlass;
    using Centrifuge;

    public class Hand : MonoBehaviour
    {
        public HandVisualizer handVisualization;
        public Renderer handRenderer;
        public AudioSource touchAudioSource;
        public GameObject dotPrefab;

        private Camera activeCamera;

        // variables to determine best touch in case of multi touch
        private int bestTouch = -1;
        private float bestRadius = float.MaxValue;
        private float bestDistance = float.MaxValue;
        private TouchPhase bestTouchPhase = TouchPhase.Canceled;
        private TouchType bestTouchType = TouchType.Indirect;

        // parameters to exclude potential ghost touches and palm touches
        private const float maxRadius = 100.0f;
        private const float minRadius = 4.0f;

        private const float sphereCastRadius = 0.005f;

        // visual feedback parameters for touch interactions
        private const float dotHeight = 0.0001f;
        private const float dotMinSize = 0.0007f;
        private const float dotMaxSize = 0.0008f;
        private const float dotGrowingSpeed = 0.0002f;
        private const float dotDistance = 0.06f;
        private GameObject dot;
        private bool dotGrowing = true;

        private List<TouchPhase> touchPhasePriority = new List<TouchPhase> { TouchPhase.Moved, TouchPhase.Stationary, TouchPhase.Began, TouchPhase.Ended, TouchPhase.Canceled };
        private List<TouchType> touchTypePriority = new List<TouchType> { TouchType.Direct, TouchType.Stylus, TouchType.Indirect };

        private List<RaycastResult> rayCastResults;
        private List<RaycastResult> hitInteractables;
        private List<RaycastHit> hits;
        private List<RaycastHit> interactables;
        private List<RaycastHit> municipalities;
        private List<RaycastHit> landmarks;
        private List<RaycastHit> animals;

        private Interactable _currentInteractable = null;
        public Interactable currentInteractable
        {
            get { return _currentInteractable; }
            set
            {
                if (_currentInteractable == value)
                {
                    return;
                }

                _currentInteractable = value;

                if (value)
                {
                    handVisualization.ScaleToSensable(value);
                }
                else
                {
                    handRenderer.enabled = false;
                }
            }
        }

        private void Awake()
        {
            if (!touchAudioSource && !TryGetComponent(out touchAudioSource))
            {
                Debug.LogError("Hand is missing a reference to a AudioSource");
            }

            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");

            activeCamera = Camera.main;
            currentInteractable = null;
            handRenderer.enabled = false;
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            currentInteractable = null;
            handRenderer.enabled = false;
        }

        public void OnSubMiniGameAborted(string subMiniGameName)
        {
            currentInteractable = null;
            handRenderer.enabled = false;
        }

        private void FixedUpdate()
        {
            GetBestTouch();
            CheckTouch();
            UpdateDot();
        }

        private void GetBestTouch()
        {
            bestTouch = -1;
            bestRadius = float.MaxValue;
            bestDistance = float.MaxValue;
            bestTouchPhase = TouchPhase.Canceled;
            bestTouchType = TouchType.Indirect;
            for (int i = 0; i < Input.touchCount; i++)
            {
                bool setBestTouch = false;
                if (currentInteractable)
                {
                    Ray ray = activeCamera.ScreenPointToRay(Input.touches[i].position);
                    float distance = Vector3.Cross(ray.direction, currentInteractable.transform.position - ray.origin).magnitude;
                    if (Input.touches[i].radius >= minRadius && Input.touches[i].radius <= maxRadius &&
                        touchPhasePriority.IndexOf(bestTouchPhase) >= touchPhasePriority.IndexOf(Input.touches[i].phase) &&
                        touchTypePriority.IndexOf(bestTouchType) >= touchTypePriority.IndexOf(Input.touches[i].type))
                    {
                        if (touchPhasePriority.IndexOf(bestTouchPhase) > touchPhasePriority.IndexOf(Input.touches[i].phase) ||
                            touchTypePriority.IndexOf(bestTouchType) > touchTypePriority.IndexOf(Input.touches[i].type))
                        {
                            setBestTouch = true;
                        }
                        else if (Input.touches[i].radius < bestRadius || distance < bestDistance)
                        {
                            setBestTouch = true;
                        }
                    }
                    if (setBestTouch)
                    {
                        bestTouch = i;
                        bestRadius = Input.touches[i].radius;
                        bestDistance = distance;
                        bestTouchPhase = Input.touches[i].phase;
                        bestTouchType = Input.touches[i].type;
                    }
                }
                else
                {
                    if (Input.touches[i].radius >= minRadius && Input.touches[i].radius <= maxRadius &&
                        touchPhasePriority.IndexOf(bestTouchPhase) >= touchPhasePriority.IndexOf(Input.touches[i].phase) &&
                        touchTypePriority.IndexOf(bestTouchType) >= touchTypePriority.IndexOf(Input.touches[i].type))
                    {
                        if (touchPhasePriority.IndexOf(bestTouchPhase) > touchPhasePriority.IndexOf(Input.touches[i].phase) ||
                            touchTypePriority.IndexOf(bestTouchType) > touchTypePriority.IndexOf(Input.touches[i].type))
                        {
                            setBestTouch = true;
                        }
                        else if (Input.touches[i].radius < bestRadius)
                        {
                            setBestTouch = true;
                        }
                    }
                    if (setBestTouch)
                    {
                        bestTouch = i;
                        bestRadius = Input.touches[i].radius;
                        bestTouchPhase = Input.touches[i].phase;
                        bestTouchType = Input.touches[i].type;
                    }
                }
            }

            if (bestTouch == -1)
            {
                if (dot)
                {
                    Destroy(dot);
                    dot = null;
                }
            }
            else
            {
                Touch touch = Input.GetTouch(bestTouch);

                if (bestTouchPhase == TouchPhase.Began)
                {
                    touchAudioSource.Play();
                }

                if (!dot && !currentInteractable && !IsTouchOverCanvasObject(touch))
                {
                    dot = Instantiate(dotPrefab);
                    dotGrowing = true;
                }

                if (dot)
                {
                    if (currentInteractable || IsTouchOverCanvasObject(touch))
                    {
                        Destroy(dot);
                        dot = null;
                    }
                    else
                    {
                        dot.transform.position = activeCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, dotDistance));
                        dot.transform.rotation = Quaternion.LookRotation(activeCamera.transform.up, activeCamera.transform.forward);
                        dot.transform.parent = activeCamera.transform;
                    }
                }
            }
        }

        private void CheckTouch()
        {
            if (bestTouch == -1 && currentInteractable)
            {
                if (currentInteractable is County)
                {
                    (currentInteractable as County).CheckCounty();
                }
                else if (currentInteractable is Food)
                {
                    (currentInteractable as Food).CheckStores();
                    (currentInteractable as Food).CheckTools();
                }
                else if (currentInteractable is Packaging)
                {
                    (currentInteractable as Packaging).CheckTrashContainers();
                }
                else if (currentInteractable is MagnifyingGlass)
                {
                    (currentInteractable as MagnifyingGlass).CheckMagnifyingGlass();
                }
                else if (currentInteractable is Centrifuge)
                {
                    (currentInteractable as Centrifuge).CheckCentrifuge();
                }

                currentInteractable.Interact();
                currentInteractable = null;
            }
            else if (bestTouch > -1)
            {
                Touch touch = Input.GetTouch(bestTouch);

                if (currentInteractable is County || currentInteractable is Food || currentInteractable is Packaging)
                {
                    Plane plane = new Plane(currentInteractable.transform.parent.parent.up, currentInteractable.transform.position);
                    Ray ray = activeCamera.ScreenPointToRay(touch.position);
                    float enter;
                    if (plane.Raycast(ray, out enter))
                    {
                        currentInteractable.transform.position = ray.GetPoint(enter);
                        if (currentInteractable is County)
                        {
                            (currentInteractable as County).CheckScale();
                        }

                        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            if (currentInteractable is County)
                            {
                                (currentInteractable as County).CheckCounty();
                            }
                            else if (currentInteractable is Food)
                            {
                                (currentInteractable as Food).CheckStores();
                                (currentInteractable as Food).CheckTools();
                            }
                            else if (currentInteractable is Packaging)
                            {
                                (currentInteractable as Packaging).CheckTrashContainers();
                            }
                            currentInteractable = null;
                        }
                        else
                        {
                            if (currentInteractable is Food)
                            {
                                (currentInteractable as Food).ActivateCheckmarks();
                            }
                        }
                    }
                    return;
                }
                else if (currentInteractable is MagnifyingGlass)
                {
                    if ((currentInteractable as MagnifyingGlass).rectTransform)
                    {
                        RectTransform rectTransform = (currentInteractable as MagnifyingGlass).rectTransform;
                        (currentInteractable as MagnifyingGlass).rectTransform.position = new Vector3(touch.position.x - rectTransform.rect.width * rectTransform.localScale.x / 4.0f,
                            touch.position.y + rectTransform.rect.height * rectTransform.localScale.y / 4.0f, 0.0f);
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        (currentInteractable as MagnifyingGlass).CheckMagnifyingGlass();
                        currentInteractable = null;
                    }
                    return;
                }
                else if (currentInteractable is Centrifuge)
                {
                    (currentInteractable as Centrifuge).OnTouch(new Vector3(touch.position.x, touch.position.y, 0.0f));

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        (currentInteractable as Centrifuge).CheckCentrifuge();
                        currentInteractable = null;
                    }
                    return;
                }
                else
                {
                    if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId) && (currentInteractable == null || !IsTouchOverCanvasObject(touch)))
                    {
                        rayCastResults = GetTouchOverCanvasObject(touch);
                        if (rayCastResults.Count > 0 && currentInteractable == null)
                        {
                            hitInteractables = rayCastResults.FindAll((rayCastResult) => rayCastResult.gameObject.GetComponent<Interactable>());
                            if (hitInteractables.Count > 0)
                            {
                                currentInteractable = hitInteractables[0].gameObject.GetComponent<Interactable>().rootAsInteractable;
                            }
                        }
                        else
                        {
                            Ray ray = activeCamera.ScreenPointToRay(touch.position);
                            hits = new List<RaycastHit>(Physics.SphereCastAll(ray, sphereCastRadius));

                            if (hits.Count > 0)
                            {
                                interactables = hits.FindAll((hit) => hit.transform.GetComponent<Interactable>());
                                municipalities = hits.FindAll((hit) => hit.transform.GetComponent<CadastralCommunity>());
                                landmarks = hits.FindAll((hit) => hit.transform.GetComponent<Landmark>());
                                animals = hits.FindAll((hit) => hit.transform.GetComponent<VirtualAnimal>());
                                if (interactables.Count > 0 || municipalities.Count > 0 || landmarks.Count > 0 || animals.Count > 0)
                                {
                                    if (municipalities.Count > 0 && currentInteractable == null)
                                    {
                                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.MunicipalityTouch, municipalities[0].transform.name));
                                    }
                                    if (landmarks.Count > 0 && currentInteractable == null)
                                    {
                                        string landmarksString = "";
                                        foreach (RaycastHit landmark in landmarks)
                                        {
                                            landmarksString += landmark.transform.gameObject.GetComponent<Landmark>().landmarkName;
                                        }
                                        StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.LandmarkTouch, landmarksString));
                                    }
                                    if (animals.Count > 0 && currentInteractable == null)
                                    {
                                        float closestDistance = -1.0f;
                                        RaycastHit closestRayCastHit = default(RaycastHit);
                                        foreach (RaycastHit animal in animals)
                                        {
                                            if (animal.distance < closestDistance || closestDistance < 0.0f)
                                            {
                                                closestDistance = animal.distance;
                                                closestRayCastHit = animal;
                                            }
                                        }
                                        if (closestDistance >= 0.0f)
                                        {
                                            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.AnimalTouch, closestRayCastHit.transform.gameObject.GetComponent<VirtualAnimal>().type));
                                        }
                                    }
                                    if (interactables.Count > 0)
                                    {
                                        if (currentInteractable == null)
                                        {
                                            currentInteractable = interactables[0].transform.GetComponent<Interactable>().rootAsInteractable;

                                            if (!(currentInteractable is County || currentInteractable is Food || currentInteractable is Packaging))
                                            {
                                                handRenderer.enabled = true;
                                            }

                                            transform.position = currentInteractable.transform.position;
                                            transform.rotation = currentInteractable.transform.rotation;
                                        }
                                        else
                                        {
                                            Interactable newInteractable = interactables[0].transform.GetComponent<Interactable>().rootAsInteractable;

                                            if (currentInteractable != newInteractable)
                                            {
                                                currentInteractable = newInteractable;

                                                if (currentInteractable is County || currentInteractable is Food || currentInteractable is Packaging)
                                                {
                                                    handRenderer.enabled = false;
                                                }
                                                else
                                                {
                                                    handRenderer.enabled = true;
                                                    handVisualization.ScaleToSensable(currentInteractable);
                                                }

                                                transform.position = newInteractable.transform.position;
                                                transform.rotation = newInteractable.transform.rotation;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateDot()
        {
            if (dot)
            {
                if (dotGrowing)
                {
                    float newSize = dot.transform.localScale.x + Time.deltaTime * dotGrowingSpeed;
                    if (newSize >= dotMaxSize)
                    {
                        dotGrowing = false;
                        newSize = dotMaxSize;
                    }
                    dot.transform.localScale = new Vector3(newSize, dotHeight, newSize);
                }
                else
                {
                    float newSize = dot.transform.localScale.x - Time.deltaTime * dotGrowingSpeed;
                    if (newSize <= dotMinSize)
                    {
                        dotGrowing = true;
                        newSize = dotMinSize;
                    }
                    dot.transform.localScale = new Vector3(newSize, dotHeight, newSize);
                }
            }
        }

        private bool IsTouchOverCanvasObject(Touch touch)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touch.position;
            eventData.pointerId = touch.fingerId;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        private List<RaycastResult> GetTouchOverCanvasObject(Touch touch)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touch.position;
            eventData.pointerId = touch.fingerId;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results;
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
        }
    }
}