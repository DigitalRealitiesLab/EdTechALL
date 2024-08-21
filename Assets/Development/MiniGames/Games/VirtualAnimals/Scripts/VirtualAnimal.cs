using UnityEngine;

public class VirtualAnimal : MonoBehaviour
{
    public AnimalType type;
    public Collider touchCollider;
    public VisualGuidanceTarget visualGuidanceTarget;

    private void Awake()
    {
        if (!touchCollider)
        {
            touchCollider = GetComponent<Collider>();
        }

        if (!visualGuidanceTarget)
        {
            visualGuidanceTarget = GetComponentInChildren<VisualGuidanceTarget>();
        }

        Debug.Assert(touchCollider, "VirtualAnimal is missing a reference to a Collider");
    }

    public enum AnimalType
    {
        Cow,
        Goat,
        Sheep,
        Calf,
        Bull
    }
}
