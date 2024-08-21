using UnityEngine;

namespace InteractionSystem.Packaging
{
    public class PackagingBundle : MonoBehaviour
    {
        public Packaging[] packagings;

        public int id;

        public bool finished = false;

        private void Awake()
        {
            EventBus.SaveRegisterCallback(this, MiniGame.EventId.MiniGameEvents.PackagingBundleActivated, "OnPackagingBundleActivated");
        }

        public void OnPackagingBundleActivated(int otherId)
        {
            if (otherId == id)
            {
                foreach (Packaging packaging in packagings)
                {
                    packaging.gameObject.SetActive(true);
                }
                finished = true;
            }
            gameObject.SetActive(false);
        }

        public void OnPackagingBundleButtonClick()
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.PackagingBundleActivated, id);
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, MiniGame.EventId.MiniGameEvents.PackagingBundleActivated, "OnPackagingBundleActivated");
        }
    }
}
