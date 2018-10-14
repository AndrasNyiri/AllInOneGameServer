using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Primitives;
using LightGameServer.Game.Model;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Game.Prefabs.Units
{
    class Caster3 : Unit
    {
        public Caster3(Match myMatch, PlayerInfo playerInfo, Vector2 pos) : base(myMatch, playerInfo, Server.Get().DataStore.GetUnitSettings("Caster3"), pos)
        {

        }
    }
}
