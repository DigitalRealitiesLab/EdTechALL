using UnityEngine;

namespace MiniGame.MapLegend
{
    [CreateAssetMenu(fileName = "MapLegendLookup", menuName = "ScriptableObjects/MapLegendLookup", order = 1)]
    public class MapLegendLookup : ImageMarkerLookup<MapLegendLookupData> { }

    [System.Serializable]
    public class MapLegendLookupData : AImageMarkerLookupData
    {
        public string municipalCode;
    }
}
