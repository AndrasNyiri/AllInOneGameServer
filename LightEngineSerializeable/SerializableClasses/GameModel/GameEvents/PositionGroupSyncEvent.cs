using System.Collections.Generic;
using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class PositionGroupSyncEvent : GameEvent
    {
        public PositionGroupSyncEvent() : base(GameEventType.PositionGroupSync)
        {
        }

        public List<PositionSyncEvent> PositionSyncs { get; set; }
    }
}
