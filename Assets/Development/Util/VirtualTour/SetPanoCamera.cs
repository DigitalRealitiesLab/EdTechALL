using InfinityCode.uPano;
using UnityEngine;

public class SetPanoCamera : MonoBehaviour
{
    public Pano pano;

    private void Awake()
    {
        if (!pano)
        {
            pano = GetComponent<Pano>();
        }

        pano.existingCamera = Camera.main;
    }
}
