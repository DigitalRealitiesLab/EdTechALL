using InfinityCode.uPano.InteractiveElements;

namespace InfinityCode.uPano.Actions
{
    public class SendStepEvent : TransitionAction
    {
        public int step;

        protected override void InvokeAction(InteractiveElement element)
        {
            StartCoroutine(EventBus.PublishCoroutine(MiniGame.EventId.MiniGameEvents.VirtualTourStep, step));
        }
    }
}