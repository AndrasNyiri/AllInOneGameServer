using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Primitives;
using LightGameServer.Game.Model;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Game.Prefabs.Units
{
    class Caster2 : Unit
    {
        public Caster2(Match myMatch, PlayerInfo playerInfo, Vector2 pos) : base(myMatch, playerInfo, Server.Get().DataStore.GetUnitSettings("Caster2"), pos)
        {

        }
    }
}
