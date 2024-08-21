using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FederalUnit : MonoBehaviour
{
    protected Dictionary<string, FederalUnit> subUnits = new Dictionary<string, FederalUnit>();
    public FederalUnit superUnit = null;

    public virtual bool requireHighlightAreaText { get { return true; } }

    public virtual Bounds bounds
    {
        get
        {
            Bounds bounds = new Bounds();
            if (subUnits.Values.Count >= 1)
            {
                bounds = subUnits.Values.First().bounds;
            }
            foreach (var subUnit in subUnits.Values)
            {
                bounds.Encapsulate(subUnit.bounds);
            }
            bounds.center = (bounds.max + bounds.min) * 0.5f;
            return bounds;
        }
    }

    public virtual void HighlightTarget(bool enabled)
    {
        foreach (var subUnit in subUnits.Values)
        {
            subUnit.HighlightTarget(enabled);
        }
    }

    protected virtual void GenerateFederalStructure()
    {
        var firstLevelUnits = transform.GetComponentsInFirstGenerationChildren<FederalUnit>();
        foreach (var subUnit in firstLevelUnits)
        {
            subUnits.Add(subUnit.gameObject.name, subUnit);
            if (subUnit)
            {
                subUnit.superUnit = this;
                subUnit.GenerateFederalStructure();
            }
        };
    }

    public FederalUnit GetSubUnitByID(string id)
    {
        FederalUnit unit = null;
        subUnits.TryGetValue(id, out unit);
        return unit;
    }
}