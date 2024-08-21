using InfinityCode.uPano.HotSpots;
using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    [AddComponentMenu("uPano/Actions/For HotSpots/StartSubMiniGame")]
    public class StartSubMiniGame : HotSpotAction
    {
        public override void Invoke(HotSpot hotSpot)
        {
            EventBus.Publish(MiniGame.EventId.MiniGameEvents.SubMiniGameStarted, hotSpot);
        }
    }
}