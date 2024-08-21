using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace MiniGame
{
    public class PositioningQuestion : MonoBehaviour
    {
        public PositioningAnswer[] answers;
        public GameObject[] answerPositions;
        public string question;
        public string promptText = "";
        public string infoText = "";
        public Sprite sprite = null;
        public VideoClip videoClip = null;
        public AudioClip audioClip = null;
        public Sprite answerSprite = null;
        public VideoClip answerVideoClip = null;
        public AudioClip answerAudioClip = null;
        public bool useQuizText = true;

        public bool keepPositions = false;
        public bool ignorePlayerPosition = false;
        public PositioningAnswer selectedAnswer;
        private float distance;
        private Camera mainCamera;

        private void Awake()
        {
            if (answers.Length == 0)
            {
                answers = GetComponentsInChildren<PositioningAnswer>();
            }
            Debug.Assert(answers.Length > 0, "There are no answers selected for a PositioningQuestion");
            Debug.Assert(answerPositions.Length > 0, "There are no answerPositions selected for a PositioningQuestion");
            Debug.Assert(answerPositions.Length == answers.Length, "There are a different number of answers and answerPositions selected for a PositioningQuestion");

            mainCamera = Camera.main;
            selectedAnswer = null;
            distance = 0.0f;
            if (!keepPositions)
            {
                List<GameObject> answerPositionList = new List<GameObject>(answerPositions);
                foreach (PositioningAnswer answer in answers)
                {
                    int answerPosition = Random.Range(0, answerPositionList.Count);

                    if (answerPositionList[answerPosition].name.EndsWith('1'))
                    {
                        answer.order = 0;
                    }
                    else if (answerPositionList[answerPosition].name.EndsWith('2'))
                    {
                        answer.order = 1;
                    }
                    else if (answerPositionList[answerPosition].name.EndsWith('3'))
                    {
                        answer.order = 2;
                    }

                    answer.transform.parent = answerPositionList[answerPosition].transform;
                    answerPositionList.RemoveAt(answerPosition);
                    answer.transform.localPosition = Vector3.zero;
                    answer.Selected = false;
                }
            }
            else
            {
                foreach (PositioningAnswer answer in answers)
                {
                    answer.Selected = false;
                }
            }
        }

        private void Update()
        {
            if (!ignorePlayerPosition)
            {
                Vector3 referencePoint = mainCamera.transform.position;
                if (selectedAnswer)
                {
                    referencePoint.y = selectedAnswer.transform.position.y;
                    distance = Vector3.Distance(selectedAnswer.transform.position, referencePoint);
                }
                else
                {
                    distance = 0.0f;
                }
                for (int i = 0; i < answers.Length; i++)
                {
                    referencePoint.y = answers[i].transform.position.y;
                    float currentDistance = Vector3.Distance(answers[i].transform.position, referencePoint);
                    if (!selectedAnswer || currentDistance < distance)
                    {
                        distance = currentDistance;
                        answers[i].Selected = true;
                        if (!selectedAnswer || selectedAnswer != answers[i])
                        {
                            if (selectedAnswer)
                            {
                                selectedAnswer.Selected = false;
                            }
                            selectedAnswer = answers[i];
                        }
                    }
                }
                if (selectedAnswer)
                {
                    if (distance > selectedAnswer.maxDistance)
                    {
                        selectedAnswer.Selected = false;
                        selectedAnswer = null;
                    }
                }
            }
        }

        public bool AnswerQuestion()
        {
            if (selectedAnswer)
            {
                return selectedAnswer.correct;
            }
            else
            {
                return false;
            }
        }
    }
}
