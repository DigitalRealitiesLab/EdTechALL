using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameSceneLookup", menuName = "ScriptableObjects/MiniGameSceneLookup", order = 1)]
public class MiniGameSceneLookup : ImageMarkerLookup<MiniGameSceneLoadData>
{}

[System.Serializable]
public class MiniGameSceneLoadData : AImageMarkerLookupData
{
    public string sceneName;
    public Sprite sceneSprite;
    public TextureToPrefabMapper prefabMapper;
    public MiniGame.MiniGameType miniGameType;
}