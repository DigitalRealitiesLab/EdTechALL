using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem.County
{
    public class CountyPositioning : MonoBehaviour
    {
        public County[] counties;
        public GameObject[] countyPositions;

        private void Awake()
        {
            if (counties.Length == 0)
            {
                counties = GetComponentsInChildren<County>();
            }
            Debug.Assert(counties.Length > 0, "There are no countye selected for CountyPositioning");
            Debug.Assert(countyPositions.Length > 0, "There are no countyPositions selected for CountyPositioning");
            Debug.Assert(countyPositions.Length == counties.Length, "There are a different number of answers and countyPositions selected for CountyPositioning");

            List<GameObject> countyPositionList = new List<GameObject>(countyPositions);
            foreach (County county in counties)
            {
                int countyPosition = Random.Range(0, countyPositionList.Count);
                county.transform.position = countyPositionList[countyPosition].transform.position;
                county.startPosition = county.transform.localPosition;
                countyPositionList.RemoveAt(countyPosition);
            }
        }
    }
}
