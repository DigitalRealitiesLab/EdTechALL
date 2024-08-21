using UnityEngine;

namespace MiniGame.LandscapeLayers
{
    [CreateAssetMenu(fileName = "LandscapeLayersLookup", menuName = "ScriptableObjects/LandscapeLayersLookup", order = 2)]
    public class LandscapeLayersLookup : ScriptableObject
    {
        public LandscapeLayersData[] landscapeLayersData;
    }

    [System.Serializable]
    public class LandscapeLayersData
    {
        public string name;
        public LandscapeType landscapeType;
        public Texture texture;
        [Range(0.0f,1.0f)]
        public float alpha = 1.0f;
        public Color toggleColor = Color.white;
        [System.NonSerialized, HideInInspector]
        public GameObject instance;
    }

    public enum LandscapeType
    {
        Woods,
        Waters,
        Agriculture,
        Buildings,
        Glaciers,
        Rock
    }
}