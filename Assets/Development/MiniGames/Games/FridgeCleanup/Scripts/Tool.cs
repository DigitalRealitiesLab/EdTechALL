using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolType toolType;

    public enum ToolType
    {
        See,
        Smell,
        Taste,
    }
}
