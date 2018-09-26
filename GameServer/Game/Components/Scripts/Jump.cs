using LightEngine.Components;
using LightEngine.PhysicsEngine.Primitives;

namespace AllInOneGameServer.Game.Components.Scripts
{
    class Jump : Component
    {
        public float jumpDelay = 1f;
        public float jumpFoce = 15f;


        private float _jumpTime;

        public override void Update()
        {
            if (this.gameObject.gameLoop.Time > _jumpTime)
            {
                _jumpTime = this.gameObject.gameLoop.Time + jumpDelay;
                this.gameObject.GetComponent<Rigidbody>().body.ApplyLinearImpulse(new Vector2(0, -jumpFoce));
            }
        }
    }
}
