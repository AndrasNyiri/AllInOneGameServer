using System;
using LightEngineCore.Components;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightGameServer.Game.Model;
using LightGameServer.Game.Prefabs.Units;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Game.Prefabs.Skills
{
    class FireBall : Skill
    {
        //Value1: ExplodeRadius
        //Value2: ExplodeDamage Multiplier
        //Value3: PushForce

        public FireBall(Match match, PlayerInfo player, Vector2 pos, Vector2 pushDir, short damage)
        : base(match, player, NetworkObjectType.FireBall, Server.Get().DataStore.GetSkillSettings("FireBall"), pos, damage)
        {
            GetComponent<Rigidbody>().body.ApplyLinearImpulse(pushDir);
        }

        public override void OnCollided(GameObject go)
        {
            if (Destroyed) return;
            if (go is Unit)
            {
                Unit unit = (Unit)go;
                if (Player.IsInDeck(unit)) return;
                unit.TakeDamage(Damage);
                this.Destroy();
            }
        }

        public override void Destroy()
        {
            Explode();
            base.Destroy();
        }

        private void Explode()
        {
            var affectedUnits = MyMatch.gameLoop.GetInOverlapCircle<Unit>(Pos, Settings.Value1);
            foreach (var affectedUnit in affectedUnits)
            {
                var pushDir = affectedUnit.Pos - Pos;
                pushDir.Normalize();
                affectedUnit.GetComponent<Rigidbody>().body.ApplyLinearImpulse(pushDir * Settings.Value3);
                if (Player.IsInDeck(affectedUnit)) continue;
                affectedUnit.TakeDamage((int)(Damage * Settings.Value2));
            }
        }

    }
}
