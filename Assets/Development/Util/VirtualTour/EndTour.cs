using InfinityCode.uPano.HotSpots;
using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    [AddComponentMenu("uPano/Actions/For HotSpots/EndTour")]
    public class EndTour : HotSpotAction
    {
        public override void Invoke(HotSpot hotSpot)
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.NextStep);
        }
    }
}