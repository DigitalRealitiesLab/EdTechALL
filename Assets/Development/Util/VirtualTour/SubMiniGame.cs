using InfinityCode.uPano.HotSpots;
using UnityEngine;

namespace MiniGame
{
    public class SubMiniGame : MonoBehaviour
    {
        public GameObject subMiniGamePrefab;

        private GameObject spawnedInstance;
        private HotSpot hotSpot = null;

        private void Awake()
        {
            Debug.Assert(subMiniGamePrefab, "SubMiniGame is missing a subMiniGamePrefab");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
        }

        public void OnSubMiniGameStarted(HotSpot sender)
        {
            if (sender.title.Equals(gameObject.name))
            {
                hotSpot = sender;

                spawnedInstance = Instantiate(subMiniGamePrefab);

                spawnedInstance.name = gameObject.name;

                spawnedInstance.transform.parent = transform.parent.parent.parent.parent;

                hotSpot.instance.gameObject.SetActive(false);
            }
        }

        public void OnSubMiniGameEnded(string subMiniGameName)
        {
            if (spawnedInstance)
            {
                if (spawnedInstance.name.Equals(subMiniGameName))
                {
                    Destroy(spawnedInstance);
                    spawnedInstance = null;
                    gameObject.SetActive(false);
                    if (hotSpot != null)
                    {
                        hotSpot.instance.gameObject.SetActive(true);
                    }
                }
            }
            else if (hotSpot != null)
            {
                if (hotSpot.title.Equals(subMiniGameName))
                {
                    gameObject.SetActive(false);
                    hotSpot.instance.gameObject.SetActive(true);
                }
            }
            else if (gameObject.name.Equals(subMiniGameName))
            {
                gameObject.SetActive(false);
            }
        }

        public virtual void OnSubMiniGameAborted(string subMiniGameName)
        {
            if (spawnedInstance)
            {
                if (spawnedInstance.name.Equals(subMiniGameName))
                {
                    Destroy(spawnedInstance);
                    spawnedInstance = null;

                    if (hotSpot != null)
                    {
                        hotSpot.instance.gameObject.SetActive(true);
                    }
                }
            }
            else if (hotSpot != null)
            {
                if (hotSpot.title.Equals(subMiniGameName))
                {
                    gameObject.SetActive(false);
                    hotSpot.instance.gameObject.SetActive(true);
                }
            }
            else if (gameObject.name.Equals(subMiniGameName))
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameStarted, "OnSubMiniGameStarted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameAborted, "OnSubMiniGameAborted");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.SubMiniGameEnded, "OnSubMiniGameEnded");
        }
    }
}
