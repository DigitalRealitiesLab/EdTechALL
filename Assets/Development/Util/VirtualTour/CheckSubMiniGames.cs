using UnityEngine;

namespace MiniGame
{
    public class CheckSubMiniGames : MonoBehaviour
    {
        public GameObject[] objectsToActivate;
        public bool hasOtherTour = false;
        public CheckSubMiniGames otherObject;
        public bool activate = false;
        public bool hidden = false;

        private VirtualTourCheckmarkUI[] virtualTourCheckmarkUIs;

        private void Awake()
        {
            if (hasOtherTour)
            {
                EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SwitchableVirtualTourSetOther, "OnSwitchableVirtualTourSetOther");
            }
            else
            {
                foreach (GameObject objectToActivate in objectsToActivate)
                {
                    objectToActivate.SetActive(activate && !hidden);
                }
            }
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
        }

        private void Start()
        {
            if (hasOtherTour)
            {
                EventBus.Publish(EventId.MiniGameEvents.SwitchableVirtualTourSetOther, this);
            }
        }

        private void Update()
        {
            virtualTourCheckmarkUIs = transform.parent.parent.GetComponentsInChildren<VirtualTourCheckmarkUI>(true);

            bool tmp = true;

            for (int i = 0; i < virtualTourCheckmarkUIs.Length; i++)
            {
                if (!virtualTourCheckmarkUIs[i].checkmarkToggle.isOn)
                {
                    tmp = false;
                    break;
                }
            }

            if (hasOtherTour)
            {
                if (activate != tmp)
                {
                    activate = tmp;
                }
                if (otherObject.activate == activate)
                {
                    for (int i = 0; i < objectsToActivate.Length; i++)
                    {
                        objectsToActivate[i].SetActive(activate && !hidden);
                    }
                }
            }
            else
            {
                if (activate != tmp)
                {
                    activate = tmp;
                    for (int i = 0; i < objectsToActivate.Length; i++)
                    {
                        objectsToActivate[i].SetActive(activate && !hidden);
                    }
                }
            }
        }

        public void OnSwitchableVirtualTourSetOther(CheckSubMiniGames other)
        {
            if (transform.parent.parent.name == other.transform.parent.parent.name && this != other)
            {
                otherObject = other;
                foreach (GameObject objectToActivate in objectsToActivate)
                {
                    objectToActivate.SetActive(activate && !hidden);
                }
            }
        }

        public void OnAudioSubMiniGameIsPlaying(bool isPlaying)
        {
            hidden = isPlaying;
            if (hasOtherTour)
            {
                if (otherObject.activate == activate)
                {
                    foreach (GameObject objectToActivate in objectsToActivate)
                    {
                        objectToActivate.SetActive(activate && !hidden);
                    }
                }
                else
                {
                    foreach (GameObject objectToActivate in objectsToActivate)
                    {
                        objectToActivate.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (GameObject objectToActivate in objectsToActivate)
                {
                    objectToActivate.SetActive(activate && !hidden);
                }
            }
        }

        private void OnDestroy()
        {
            if (hasOtherTour)
            {
                EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SwitchableVirtualTourSetOther, "OnSwitchableVirtualTourSetOther");
            }
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
        }
    }
}
