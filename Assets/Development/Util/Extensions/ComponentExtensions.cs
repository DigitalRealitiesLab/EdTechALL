using UnityEngine;
using System.Collections.Generic;

public static class ComponentExtensions
{
    public static List<T> GetComponentsInFirstGenerationChildren<T>(this Component component, bool includeInactive = false) where T : Component
    {
        List<T> components = new List<T>();
        foreach (var childComponent in component.transform.GetComponentsInChildren<T>(includeInactive))
        {
            if (childComponent.transform.parent == component.transform)
            {
                components.Add(childComponent);
            }
        }
        return components;
    }
}