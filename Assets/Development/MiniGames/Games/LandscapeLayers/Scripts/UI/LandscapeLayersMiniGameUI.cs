using UnityEngine;

namespace MiniGame.LandscapeLayers.UI
{
    public class LandscapeLayersMiniGameUI : MonoBehaviour
    {
        public LandscapeLayersLookup landscapeLayersLookup;
        public Transform toggleParent;
        public GameObject layerTogglePrefab;


        private void Awake()
        {
            Debug.Assert(landscapeLayersLookup, "LandscapeLayersMiniGameUI is missing a reference to a LandscapeLayersLookup");
        }

        private void OnEnable()
        {
            foreach (Transform child in toggleParent.GetComponentsInFirstGenerationChildren<Transform>())
            {
                Destroy(child.gameObject);
            }
            InstantiateLayerToggles();
        }

        private void InstantiateLayerToggles()
        {
            for (int i = 0; i < landscapeLayersLookup.landscapeLayersData.Length; i++)
            {
                GameObject instance = Instantiate(layerTogglePrefab, toggleParent);
                instance.name = landscapeLayersLookup.landscapeLayersData[i].name;
                LandscapeLayersMiniGameToggle toggle;
                if (instance.TryGetComponent(out toggle))
                {
                    toggle.landscapeLayersData = landscapeLayersLookup.landscapeLayersData[i];
                }
                else
                {
                    Debug.LogError("LandscapeLayersMiniGameToggle Prefab is missing a Toggle component");
                }
            }
        }
    }
}