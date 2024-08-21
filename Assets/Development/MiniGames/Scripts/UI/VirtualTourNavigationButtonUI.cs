using UnityEngine.UI;
using InfinityCode.uPano;
using InfinityCode.uPano.Tours;
using UnityEngine;

namespace MiniGame
{
    public class VirtualTourNavigationButtonUI : MonoBehaviour
    {
        public Image image;
        public Button button;
        public TourItem tourItem;
        public TourItem switchableTourItem;
        public bool switchable = false;
        public bool flachland = false;
        public string infoText = "";
        public string identifier = "";
        public string title = "";
        public int step = 0;

        private void Awake()
        {
            Debug.Assert(button, "VirtualTourNavigationButtonUI is missing a reference to an Image");
            Debug.Assert(button, "VirtualTourNavigationButtonUI is missing a reference to a Button");
        }

        public void OnVirtualTourNavigationButtonClick()
        {
            if (tourItem && (switchableTourItem || !switchable))
            {
                if (switchable && tourItem.transform.parent != switchableTourItem.transform.parent)
                {
                    switchableTourItem.transform.parent.gameObject.SetActive(false);
                    tourItem.transform.parent.gameObject.SetActive(true);
                }
                EventBus.Publish(EventId.MiniGameEvents.VirtualTourStep, step);

                if (tourItem.gameObject.activeSelf && tourItem.transform.parent.gameObject.activeSelf)
                {
                    return;
                }

                Pano changedPano = null;
                foreach (Pano activePano in tourItem.transform.parent.GetComponentsInChildren<Pano>(true))
                {
                    if (activePano.gameObject.activeSelf)
                    {
                        activePano.gameObject.SetActive(false);
                        changedPano = activePano;
                        break;
                    }
                }

                if (changedPano)
                {
                    Pano pano = tourItem.gameObject.GetComponent<Pano>();
                    if (pano)
                    {
                        pano.pan = changedPano.pan;
                        pano.tilt = changedPano.tilt;
                    }
                }

                tourItem.gameObject.SetActive(true);

                if (switchable)
                {
                    if (!tourItem.transform.parent.gameObject.activeSelf)
                    {
                        tourItem.transform.parent.gameObject.SetActive(true);
                        switchableTourItem.transform.parent.gameObject.SetActive(false);
                    }

                    if (switchableTourItem.gameObject.activeSelf)
                    {
                        return;
                    }

                    changedPano = null;
                    foreach (Pano activePano in switchableTourItem.transform.parent.GetComponentsInChildren<Pano>(true))
                    {
                        if (activePano.gameObject.activeSelf)
                        {
                            activePano.gameObject.SetActive(false);
                            changedPano = activePano;
                            break;
                        }
                    }

                    if (changedPano)
                    {
                        Pano pano = switchableTourItem.gameObject.GetComponent<Pano>();
                        if (pano)
                        {
                            pano.pan = changedPano.pan;
                            pano.tilt = changedPano.tilt;
                        }
                    }

                    switchableTourItem.gameObject.SetActive(true);
                }
            }
        }
    }
}