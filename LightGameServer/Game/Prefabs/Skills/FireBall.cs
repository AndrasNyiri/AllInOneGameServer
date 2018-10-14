using LightEngineCore.Components;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightGameServer.Game.Model;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Game.Prefabs.Skills
{
    class FireBall : Skill
    {
        public FireBall(Match match, PlayerInfo player, Vector2 pos, Vector2 pushDir, short damage)
        : base(match, player, NetworkObjectType.FireBall, Server.Get().DataStore.GetSkillSettings("FireBall"), pos, pushDir, damage)
        {
            GetComponent<Rigidbody>().body.ApplyLinearImpulse(pushDir);
            //GetComponent<Rigidbody>().body.ApplyForce(pushDir);
        }

        public override void OnCollidedWithGameObject(GameObject go)
        {

        }
    }
}
