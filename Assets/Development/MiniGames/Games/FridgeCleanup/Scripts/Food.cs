using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem.Food
{
    using Interactable;
    using UnityEngine.UI;

    public class Food : Interactable
    {
        public Store.StoreType correctStoreType;

        public int id;

        public int bestBeforeDays = 0;
        public int bestBeforeMonths = 0;
        public Sprite seeSprite;
        public AudioClip seeAudio;
        public AudioClip smellAudio;
        public AudioClip tasteAudio;
        public AudioClip fridgeAudio;
        public AudioClip storeroomAudio;
        public AudioClip trashAudio;
        public bool seen = false;
        public bool smelled = false;
        public bool tasted = false;

        public float threshold = 0.1f;

        public Image seeImage;
        public Image smellImage;
        public Image tasteImage;

        public Image fridgeImage;
        public Image storeroomImage;
        public Image trashImage;

        public GameObject seenCheckmark;
        public GameObject smelledCheckmark;
        public GameObject tastedCheckmark;

        private Vector3 startPosition;
        private List<Store> stores;
        private List<Tool> tools;

        protected override void Awake()
        {
            base.Awake();
            startPosition = transform.localPosition;
            stores = new List<Store>(FindObjectsOfType<Store>(true));
            tools = new List<Tool>(FindObjectsOfType<Tool>(true));
        }

        public void CheckStores()
        {
            Store closestStore = null;
            float shortestDistance = -1.0f;

            foreach (Store store in stores)
            {
                float currentDistance = Vector3.Distance(store.transform.position, transform.position);
                if (currentDistance <= threshold && (shortestDistance < 0 || shortestDistance > currentDistance))
                {
                    closestStore = store;
                    shortestDistance = currentDistance;
                }
            }

            if (shortestDistance >= 0)
            {
                EventBus.Publish(MiniGame.EventId.MiniGameEvents.StoreUsed, id, closestStore.storeType);
            }
        }

        public void CheckTools()
        {
            if (seenCheckmark)
            {
                seenCheckmark.SetActive(false);
            }

            if (smelledCheckmark)
            {
                smelledCheckmark.SetActive(false);
            }

            if (tastedCheckmark)
            {
                tastedCheckmark.SetActive(false);
            }

            seeImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            smellImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            tasteImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            fridgeImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            storeroomImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            trashImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            Tool closestTool = null;
            float shortestDistance = -1.0f;

            foreach (Tool tool in tools)
            {
                float currentDistance = Vector3.Distance(tool.transform.position, transform.position);
                if (currentDistance <= threshold && (shortestDistance < 0 || shortestDistance > currentDistance))
                {
                    closestTool = tool;
                    shortestDistance = currentDistance;
                }
            }

            if (shortestDistance >= 0)
            {
                EventBus.Publish(MiniGame.EventId.MiniGameEvents.ToolUsed, id, closestTool.toolType);
            }

            transform.localPosition = startPosition;
        }

        public void ActivateCheckmarks()
        {
            if (seenCheckmark)
            {
                seenCheckmark.SetActive(true);
            }
            else
            {
                seeImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }

            if (smelledCheckmark)
            {
                smelledCheckmark.SetActive(true);
                if (!seen)
                {
                    smellImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
            }
            else
            {
                smellImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }

            if (tastedCheckmark)
            {
                tastedCheckmark.SetActive(true);
                if (!seen || !smelled)
                {
                    tasteImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
            }
            else
            {
                tasteImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }

            if (!seen || !smelled || !tasted)
            {
                fridgeImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                storeroomImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                trashImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        public void Reset()
        {
            if (seenCheckmark)
            {
                seen = false;
                Toggle checkmarkToggle = seenCheckmark.GetComponentInChildren<Toggle>(true);
                checkmarkToggle.isOn = false;
            }
            if (smelledCheckmark)
            {
                smelled = false;
                Toggle checkmarkToggle = smelledCheckmark.GetComponentInChildren<Toggle>(true);
                checkmarkToggle.isOn = false;
            }
            if (tastedCheckmark)
            {
                tasted = false;
                Toggle checkmarkToggle = tastedCheckmark.GetComponentInChildren<Toggle>(true);
                checkmarkToggle.isOn = false;
            }
        }
    }
}
