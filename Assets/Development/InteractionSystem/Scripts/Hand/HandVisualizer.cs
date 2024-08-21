using System.Collections;
using UnityEngine;

namespace InteractionSystem.Hand
{
    public class HandVisualizer : MonoBehaviour
    {
        public Transform scalingTarget;
        public float textureDimension = 1024;
        public float ringWidth = 1024;
        public float scalingTime = 1.0f;

        private float oldSize = 0.0f;
        private float desiredSize = 0.0f;

        public void ScaleToSensable(Sensable sensable)
        {
            StopAllCoroutines();
            if (sensable)
            {
                Collider collider = sensable.GetComponentInChildren<Collider>();
                if (collider)
                {
                    Vector2 planarExtents = new Vector2(collider.bounds.extents.x, collider.bounds.extents.z);
                    ScaleVisualizerToRadius(planarExtents.magnitude);
                }
            }
        }

        private void ScaleVisualizerToRadius(float radius)
        {
            float ringCenterPercentage = (textureDimension - 2 * ringWidth) / textureDimension;
            float scale = radius * 2 / ringCenterPercentage;
            desiredSize = scale;
            oldSize = desiredSize * 1.5f;
            StartCoroutine(ScalingCoroutine());
        }

        private IEnumerator ScalingCoroutine()
        {
            float currentTime = 0.0f;
            while (currentTime < scalingTime)
            {
                float scale = Mathf.Lerp(oldSize, desiredSize, currentTime / scalingTime);
                scalingTarget.localScale = new Vector3(scale, scale, scale);
                currentTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            scalingTarget.localScale = new Vector3(desiredSize, desiredSize, desiredSize);
        }
    }
}