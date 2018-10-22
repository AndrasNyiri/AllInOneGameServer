using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class EndGameEvent : GameEvent
    {
        public byte WinnerPlayerType { get; set; }

        public EndGameEvent() : base(GameEventType.EndGame)
        {

        }
    }
}
