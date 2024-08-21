using UnityEngine;

public class Store : MonoBehaviour
{
    public StoreType storeType;

    public enum StoreType
    {
        Fridge,
        Storeroom,
        Trash
    }
}
