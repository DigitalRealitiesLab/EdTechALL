using UnityEngine;
using System.Collections.Generic;

public class Salzburg : FederalUnit
{
    public VisualGuidanceTarget visualGuidanceTarget;
    public static bool SalzburgInstantiated = false;

    protected void Awake()
    {
        Debug.Assert(visualGuidanceTarget, "Salzburg is missing a reference to a VisualGuidanceTarget");
        GenerateFederalStructure();
        SalzburgInstantiated = true;
    }

    protected override void GenerateFederalStructure()
    {
        Dictionary<string, GameObject> municipalities = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> districts = new Dictionary<string, GameObject>();

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child != transform)
            {
                string municipalCode = child.gameObject.name.Substring(0, 5);
                string district = municipalCode.Substring(0, 3);

                if (!districts.ContainsKey(district))
                {
                    GameObject districtObject = new GameObject();
                    districtObject.name = district;
                    districtObject.transform.SetParent(transform);
                    districtObject.AddComponent<FederalUnit>();
                    districts.Add(district, districtObject);
                }
                if (!municipalities.ContainsKey(municipalCode))
                {
                    GameObject municipalityObject = new GameObject();
                    municipalityObject.name = municipalCode;
                    municipalityObject.transform.SetParent(districts[district].transform);
                    municipalityObject.AddComponent<FederalUnit>();
                    municipalities.Add(municipalCode, municipalityObject);
                }
                child.SetParent(municipalities[municipalCode].transform);
            }
        }

        base.GenerateFederalStructure();
    }

    public FederalUnit GetDistrictByMunicipalCode(string municipalCode)
    {
        municipalCode = municipalCode.Substring(0, 3);
        if (subUnits.ContainsKey(municipalCode))
        {
            return subUnits[municipalCode];
        }
        return null;
    }

    public FederalUnit GetMunicipalityByMunicipalCode(string municipalCode)
    {
        FederalUnit district = GetDistrictByMunicipalCode(municipalCode);
        if (district)
        {
            municipalCode = municipalCode.Substring(0, 5);
            return district.GetSubUnitByID(municipalCode);
        }
        return null;
    }

    private void OnDestroy()
    {
        SalzburgInstantiated = false;
    }
}
