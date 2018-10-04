using LightEngineCore.Components;
using LightEngineCore.PhysicsEngine.Primitives;
using MathHelper = LightEngineCore.Common.MathHelper;

namespace LightGameServer.Game.Components.Scripts
{
    class Jump : Behaviour
    {
        public float jumpDelay = 1f;

        private float _jumpTime;

        public override void Update()
        {
            if (this.gameObject.gameLoop.Time > _jumpTime && this.gameObject.GetComponent<Rigidbody>().body.LinearVelocity.Length() < 0.3f)
            {
                _jumpTime = this.gameObject.gameLoop.Time + jumpDelay;
                this.gameObject.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(MathHelper.NextFloat(-15f, 15f), MathHelper.NextFloat(-15f, 15f)));
            }
        }
    }
}
