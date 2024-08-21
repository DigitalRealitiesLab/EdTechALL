using UnityEngine.UI;
using InfinityCode.uPano.Tours;
using InfinityCode.uPano.HotSpots;
using UnityEngine;
using System.Collections.Generic;

namespace MiniGame
{
    public class VirtualTourNavigationUI : MonoBehaviour
    {
        public Image image;
        public GameObject[] buttonGroups;
        public bool subMiniGameActive = false;
        public bool subMiniGameAudioPlaying = false;
        public VirtualTourNavigationButtonUI currentButton;

        private bool initialized = false;

        private bool admin = false;

        // if the admin mode is stopped during a virtual tour, the highest room visited and all lower rooms will stay unlocked
        public static int maxStep = 0;

        private void Awake()
        {
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.VirtualTourStep, "OnVirtualTourStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.EnableVirtualTourUI, "OnEnableVirtualTourUI");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
            EventBus.SaveRegisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
            gameObject.SetActive(false);

            if (PlayerPrefs.HasKey(EventId.AdminMode))
            {
                admin = PlayerPrefs.GetInt(EventId.AdminMode) != 0;
            }
        }

        public void OnVirtualTourStep(int step)
        {
            if (transform.parent.gameObject.activeSelf)
            {
                bool endRoom = false;
                foreach (GameObject buttonGroup in buttonGroups)
                {
                    VirtualTourNavigationButtonUI[] virtualTourNavigationButtons = buttonGroup.GetComponentsInChildren<VirtualTourNavigationButtonUI>(true);
                    if (step >= virtualTourNavigationButtons.Length)
                    {
                        endRoom = true;
                    }
                    else
                    {
                        virtualTourNavigationButtons[step].button.interactable = true;
                        if (virtualTourNavigationButtons[step].tourItem.transform.parent.gameObject.activeSelf)
                        {
                            currentButton = virtualTourNavigationButtons[step];
                            image.transform.position = virtualTourNavigationButtons[step].transform.position;
                            EventBus.Publish(EventId.MiniGameEvents.VirtualTourNavigate, virtualTourNavigationButtons[step].identifier, virtualTourNavigationButtons[step].infoText);
                        }
                    }
                }

                if (step > maxStep && !endRoom)
                {
                    maxStep = step;
                }
            }
        }

        public void OnEnableVirtualTourUI()
        {
            gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);

            if (!initialized)
            {
                List<Tour> tours = new List<Tour>(FindObjectsOfType<Tour>(true));

                for (int i = 0; i < tours.Count; i++)
                {
                    if (!tours[i].transform.parent.parent.gameObject.activeSelf)
                    {
                        tours.RemoveAt(i);
                        i--;
                    }
                }

                if (tours.Count > 1)
                {
                    if (tours[0].gameObject.name.EndsWith('1'))
                    {
                        Tour tmp = tours[0];
                        tours[0] = tours[1];
                        tours[1] = tmp;
                    }

                    VirtualTourNavigationButtonUI[] virtualTourNavigationButtons = buttonGroups[0].GetComponentsInChildren<VirtualTourNavigationButtonUI>(true);

                    for (int i = 0; i < virtualTourNavigationButtons.Length; i++)
                    {
                        virtualTourNavigationButtons[i].tourItem = tours[0].items[i];
                        virtualTourNavigationButtons[i].button.interactable = admin || (i < 1);
                        virtualTourNavigationButtons[i].switchableTourItem = tours[1].items[i];
                    }

                    virtualTourNavigationButtons = buttonGroups[1].GetComponentsInChildren<VirtualTourNavigationButtonUI>(true);

                    for (int i = 0; i < virtualTourNavigationButtons.Length; i++)
                    {
                        virtualTourNavigationButtons[i].tourItem = tours[1].items[i];
                        virtualTourNavigationButtons[i].button.interactable = admin || (i < 1);
                        virtualTourNavigationButtons[i].switchableTourItem = tours[0].items[i];
                    }
                }
                else if (tours.Count > 0)
                {
                    VirtualTourNavigationButtonUI[] virtualTourNavigationButtons = buttonGroups[0].GetComponentsInChildren<VirtualTourNavigationButtonUI>(true);

                    for (int i = 0; i < virtualTourNavigationButtons.Length; i++)
                    {
                        virtualTourNavigationButtons[i].tourItem = tours[0].items[i];
                        virtualTourNavigationButtons[i].button.interactable = admin || (i < 1);
                    }
                }
                
                initialized = true;
            }
        }

        public void OnSubMiniGameStarted(HotSpot sender)
        {
            subMiniGameActive = true;
            gameObject.SetActive(false);
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            subMiniGameActive = false;
            gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
        }

        public virtual void OnSubMiniGameAborted(string subMiniGameName)
        {
            subMiniGameActive = false;
            gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
        }

        public void OnAudioSubMiniGameIsPlaying(bool isPlaying)
        {
            subMiniGameAudioPlaying = isPlaying;
            gameObject.SetActive(!subMiniGameActive && !subMiniGameAudioPlaying);
        }

        public void OnQuitAdminMode()
        {
            admin = false;

            foreach (GameObject buttonGroup in buttonGroups)
            {
                VirtualTourNavigationButtonUI[] virtualTourNavigationButtons = buttonGroup.GetComponentsInChildren<VirtualTourNavigationButtonUI>(true);

                for (int i = 0; i < virtualTourNavigationButtons.Length; i++)
                {
                    virtualTourNavigationButtons[i].button.interactable = i <= maxStep;

                    if (i < maxStep)
                    {
                        if (virtualTourNavigationButtons[i].tourItem)
                        {
                            foreach (SubMiniGame subMiniGame in virtualTourNavigationButtons[i].tourItem.GetComponentsInChildren<SubMiniGame>())
                            {
                                EventBus.Publish(EventId.MiniGameEvents.SetSubMiniGame, subMiniGame.gameObject.name);
                            }

                            VirtualTourAudioEnabled virtualTourAudioEnabled = virtualTourNavigationButtons[i].tourItem.GetComponentInChildren<VirtualTourAudioEnabled>(true);
                            if (virtualTourAudioEnabled)
                            {
                                virtualTourAudioEnabled.finishedPlaying = true;
                            }
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.VirtualTourStep, "OnVirtualTourStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.EnableVirtualTourUI, "OnEnableVirtualTourUI");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AudioSubMiniGameIsPlaying, "OnAudioSubMiniGameIsPlaying");
            EventBus.SaveDeregisterCallback(this, EventId.QuitAdminMode, "OnQuitAdminMode");
        }
    }
}