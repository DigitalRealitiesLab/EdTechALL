using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace InteractionSystem.Centrifuge
{
    using Interactable;
    using MiniGame.DairyTour;

    public class Centrifuge : Interactable
    {
        public Centrifugation centrifugation;
        public Image indicatorImage;
        public TextMeshProUGUI indicatorText;
        public int numRotations = 3;
        public float angleThreshold = 179.0f;

        private float lastAngle = 0.0f;
        private int currentRotation = 0;

        private float resetTimer = 0.2f;
        private float resetTime = 0.2f;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(centrifugation, "Centrifuge is missing a reference to a Centrifugation");
            Debug.Assert(indicatorImage, "Centrifuge is missing a reference to a Image");
            Debug.Assert(indicatorText, "Centrifuge is missing a reference to a TextMeshProUGUI");
        }

        private void Update()
        {
            if (resetTime < resetTimer)
            {
                resetTime += Time.deltaTime;
            }
            else if (currentRotation < numRotations)
            {
                lastAngle = 0.0f;

                indicatorImage.transform.position = transform.position + new Vector3(0.0f, 200.0f, 0.0f);
            }
        }

        public void OnTouch(Vector3 touchPosition)
        {
            float currentAngle;

            if (touchPosition.x < transform.position.x)
            {
                currentAngle = 180.0f + (180.0f - Vector3.Angle(transform.up, touchPosition - transform.position));
            }
            else
            {
                currentAngle = Vector3.Angle(transform.up, touchPosition - transform.position);
            }

            if (Mathf.Abs(currentAngle - (lastAngle - 360.0f)) <= angleThreshold)
            {
                if (currentRotation < numRotations)
                {
                    currentRotation++;
                }
                lastAngle = currentAngle;
                resetTime = 0.0f;

                indicatorImage.transform.position = transform.position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), Mathf.Cos(Mathf.Deg2Rad * currentAngle), 0.0f) * 200.0f;
                indicatorText.text = currentRotation.ToString() + "/" + numRotations.ToString() + "\nDrehungen";
            }
            else if (Mathf.Abs(currentAngle - lastAngle) <= angleThreshold && currentAngle > lastAngle)
            {
                lastAngle = currentAngle;
                resetTime = 0.0f;

                indicatorImage.transform.position = transform.position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), Mathf.Cos(Mathf.Deg2Rad * currentAngle), 0.0f) * 200.0f;
                indicatorText.text = currentRotation.ToString() + "/" + numRotations.ToString() + "\nDrehungen";
            }
        }

        public void CheckCentrifuge()
        {
            if (currentRotation >= numRotations)
            {
                centrifugation.StartVideo();
            }
            else
            {
                lastAngle = 0.0f;

                indicatorImage.transform.position = transform.position + new Vector3(0.0f, 200.0f, 0.0f);
            }
        }
    }
}
