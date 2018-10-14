using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Primitives;
using LightGameServer.Game.Model;
using LightGameServer.Game.Prefabs.Skills;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Game.Prefabs.Units
{
    class Caster2 : Unit
    {
        public Caster2(Match myMatch, PlayerInfo playerInfo, Vector2 pos) : base(myMatch, playerInfo, Server.Get().DataStore.GetUnitSettings("Caster2"), pos)
        {

        }

        public override void PlayAbility(Vector2 direction)
        {
            direction.Normalize();
            Vector2 pos = Pos + direction * (Settings.Radius +
                                             Server.Get().DataStore.GetSkillSettings("FireBall").Radius + 0.1f);
            new FireBall(MyMatch, Player, pos, direction * Settings.PushForce, Settings.ProjectileDamage);
        }
    }
}
