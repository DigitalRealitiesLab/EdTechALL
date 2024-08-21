using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace MiniGame
{
    public class PositioningQuiz : MonoBehaviour
    {
        public AMiniGame miniGame;
        public PositioningQuestion[] questions;
        private int currentQuestion = 0;
        public string quizText = "Stelle dich auf die richtige Antwort und drücke den Bestätigungsbutton.\n{0}";
        public string wrongAnswerText = "Versuche es noch einmal.";
        private bool nextStepOnClose = false;
        private bool popUpActive = false;
        private bool startedfromStartWindow = false;

        private void Awake()
        {
            Debug.Assert(miniGame, "PositioningQuiz is missing a reference to a MiniGame");

            if (questions.Length == 0)
            {
                questions = GetComponentsInChildren<PositioningQuestion>();
            }
            Debug.Assert(questions.Length > 0, "There are no questions selected for the PositioningQuiz");

            foreach (PositioningQuestion question in questions)
            {
                foreach (PositioningAnswer answer in question.answers)
                {
                    answer.gameObject.SetActive(false);
                }
            }
        }

        public IEnumerator StartPositioningQuizCoroutine(int pCurrentQuestion = 0, bool fromStartWindow = false)
        {
            yield return new WaitForSeconds(0.1f);

            currentQuestion = pCurrentQuestion;
            startedfromStartWindow = fromStartWindow;
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PositioningQuizConfirm, "OnPositioningQuizConfirm");
            EventBus.SaveRegisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveRegisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
            foreach (PositioningAnswer answer in questions[currentQuestion].answers)
            {
                answer.gameObject.SetActive(true);
            }
            string text;
            if (questions[currentQuestion].useQuizText)
            {
                text = string.Format(quizText, questions[currentQuestion].question);
            }
            else
            {
                text = questions[currentQuestion].question;
            }
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.OverrideText, string.IsNullOrEmpty(questions[currentQuestion].infoText) ? text : questions[currentQuestion].infoText));
            StartCoroutine(RequestFullScreenPromptPanel(text, 0.0f, questions[currentQuestion].sprite, questions[currentQuestion].videoClip, questions[currentQuestion].audioClip));
            StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.StartPositioningQuiz));
        }

        public void OnPositioningQuizEnded()
        {
            foreach (PositioningAnswer answer in questions[currentQuestion].answers)
            {
                answer.gameObject.SetActive(false);
            }

            StartCoroutine(DeregisterEventsCoroutine());
        }

        public void OnAbortPositioningQuiz()
        {
            foreach (PositioningAnswer answer in questions[currentQuestion].answers)
            {
                answer.gameObject.SetActive(false);
            }

            StartCoroutine(DeregisterEventsCoroutine());
        }

        private IEnumerator DeregisterEventsCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizConfirm, "OnPositioningQuizConfirm");
            EventBus.SaveDeregisterCallback(this, FullScreen.EventId.UIEvents.FullScreenPromptPanelClosed, "OnFullScreenPromptPanelClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.ReplayInstruction, "OnReplayInstruction");
        }

        private IEnumerator RequestFullScreenPromptPanel(string text = "", float buttonTime = 0.0f, Sprite sprite = null, VideoClip videoClip = null, AudioClip audioClip = null)
        {
            yield return new WaitForSeconds(0.1f);
            popUpActive = true;
            FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel(text, buttonTime, sprite, videoClip, audioClip);
        }

        public void OnPositioningQuizConfirm()
        {
            if (questions[currentQuestion].AnswerQuestion())
            {
                if (string.IsNullOrEmpty(questions[currentQuestion].selectedAnswer.promptText))
                {
                    if (string.IsNullOrEmpty(questions[currentQuestion].promptText))
                    {
                        StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
                    }
                    else
                    {
                        nextStepOnClose = true;
                        StartCoroutine(RequestFullScreenPromptPanel(questions[currentQuestion].promptText, 2.0f, questions[currentQuestion].answerSprite, questions[currentQuestion].answerVideoClip, questions[currentQuestion].answerAudioClip));
                    }
                }
                else
                {
                    nextStepOnClose = true;
                    StartCoroutine(RequestFullScreenPromptPanel(questions[currentQuestion].selectedAnswer.promptText, 2.0f, questions[currentQuestion].selectedAnswer.sprite, questions[currentQuestion].selectedAnswer.videoClip, questions[currentQuestion].selectedAnswer.audioClip));
                }
            }
            else if (questions[currentQuestion].selectedAnswer)
            {
                string text;
                if (!string.IsNullOrEmpty(questions[currentQuestion].selectedAnswer.wrongAnswerText))
                {
                    text = wrongAnswerText;
                }
                else
                {
                    text = questions[currentQuestion].selectedAnswer.wrongAnswerText;
                }
                StartCoroutine(RequestFullScreenPromptPanel(text, 2.0f, questions[currentQuestion].selectedAnswer.sprite, questions[currentQuestion].selectedAnswer.videoClip, questions[currentQuestion].selectedAnswer.audioClip));
            }
        }

        public void OnFullScreenPromptPanelClosed()
        {
            popUpActive = false;
            startedfromStartWindow = false;
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
            }
        }

        public void OnMiniGameResumed()
        {
            if (nextStepOnClose)
            {
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.NextStep));
            }
            else
            {
                if (!startedfromStartWindow)
                {
                    string text;
                    if (questions[currentQuestion].useQuizText)
                    {
                        text = string.Format(quizText, questions[currentQuestion].question);
                    }
                    else
                    {
                        text = questions[currentQuestion].question;
                    }
                    StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.OverrideText, string.IsNullOrEmpty(questions[currentQuestion].infoText) ? text : questions[currentQuestion].infoText));
                    StartCoroutine(RequestFullScreenPromptPanel(text, 0.0f, questions[currentQuestion].sprite, questions[currentQuestion].videoClip, questions[currentQuestion].audioClip));
                }
            }
        }

        public void OnNextStep()
        {
            popUpActive = false;
            nextStepOnClose = false;
            foreach (PositioningAnswer answer in questions[currentQuestion].answers)
            {
                answer.gameObject.SetActive(false);
            }

            currentQuestion++;

            if (currentQuestion >= questions.Length)
            {
                currentQuestion = questions.Length - 1;
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.PositioningQuizEnded));
            }
            else
            {
                foreach (PositioningAnswer answer in questions[currentQuestion].answers)
                {
                    answer.gameObject.SetActive(true);
                }
                string text;
                if (questions[currentQuestion].useQuizText)
                {
                    text = string.Format(quizText, questions[currentQuestion].question);
                }
                else
                {
                    text = questions[currentQuestion].question;
                }
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.OverrideText, string.IsNullOrEmpty(questions[currentQuestion].infoText) ? text : questions[currentQuestion].infoText));
                StartCoroutine(RequestFullScreenPromptPanel(text, 0.0f, questions[currentQuestion].sprite, questions[currentQuestion].videoClip, questions[currentQuestion].audioClip));
            }
        }

        public void OnPreviousStep()
        {
            popUpActive = false;
            nextStepOnClose = false;
            if (currentQuestion == 0)
            {
                EventBus.Publish(EventId.MiniGameEvents.AbortPositioningQuiz);
            }
            else
            {
                foreach (PositioningAnswer answer in questions[currentQuestion].answers)
                {
                    answer.gameObject.SetActive(false);
                }

                currentQuestion--;

                foreach (PositioningAnswer answer in questions[currentQuestion].answers)
                {
                    answer.gameObject.SetActive(true);
                }
                string text;
                if (questions[currentQuestion].useQuizText)
                {
                    text = string.Format(quizText, questions[currentQuestion].question);
                }
                else
                {
                    text = questions[currentQuestion].question;
                }
                StartCoroutine(EventBus.PublishCoroutine(EventId.MiniGameEvents.OverrideText, string.IsNullOrEmpty(questions[currentQuestion].infoText) ? text : questions[currentQuestion].infoText));
                StartCoroutine(RequestFullScreenPromptPanel(text, 0.0f, questions[currentQuestion].sprite, questions[currentQuestion].videoClip, questions[currentQuestion].audioClip));
            }
        }

        public void OnReplayInstruction()
        {
            if (!popUpActive)
            {
                string text;
                if (questions[currentQuestion].useQuizText)
                {
                    text = string.Format(quizText, questions[currentQuestion].question);
                }
                else
                {
                    text = questions[currentQuestion].question;
                }
                StartCoroutine(RequestFullScreenPromptPanel(text, 0.0f, questions[currentQuestion].sprite, questions[currentQuestion].videoClip, questions[currentQuestion].audioClip));
            }
        }

        private void OnDestroy()
        {
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizEnded, "OnPositioningQuizEnded");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.AbortPositioningQuiz, "OnAbortPositioningQuiz");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PositioningQuizConfirm, "OnPositioningQuizConfirm");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PromptWindowClosed, "OnPromptWindowClosed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.MiniGameResumed, "OnMiniGameResumed");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.NextStep, "OnNextStep");
            EventBus.SaveDeregisterCallback(this, EventId.MiniGameEvents.PreviousStep, "OnPreviousStep");
        }
    }
}
