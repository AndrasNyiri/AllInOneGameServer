using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Primitives;
using LightGameServer.Game.Model;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Game.Prefabs.Units
{
    class Caster1 : Unit
    {
        public Caster1(Match myMatch, PlayerInfo playerInfo, Vector2 pos) : base(myMatch, playerInfo, Server.Get().DataStore.GetUnitSettings("Caster1"), pos)
        {
        }
    }
}
