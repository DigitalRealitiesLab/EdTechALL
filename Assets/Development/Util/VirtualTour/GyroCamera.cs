using InfinityCode.uPano;
using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope gyro;
    private Pano currentPano;

    private void Awake()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
        gyro.updateInterval = 0.01f;
        Pano.OnPanoEnabled += OnPanoEnabled;
    }

    private void Update()
    {
        if (currentPano && gyro != null)
        {
            Quaternion rotation = gyro.attitude;
            rotation *= Quaternion.Euler(0, 0, 180);
            rotation *= Quaternion.Inverse(rotation) * Quaternion.Euler(270, 180, 180) * rotation;

            float pan = 360 - rotation.eulerAngles.y;
            float tilt = rotation.eulerAngles.x;

            if (tilt > 180)
            {
                tilt -= 360;
            }
            else if (tilt < -180)
            {
                tilt += 360;
            }

            currentPano.pan = pan;
            currentPano.tilt = tilt;
        }
    }

    private void OnPanoEnabled(Pano pano)
    {
        currentPano = pano;
    }

    private void OnDestroy()
    {
        Pano.OnPanoEnabled -= OnPanoEnabled;
    }
}
