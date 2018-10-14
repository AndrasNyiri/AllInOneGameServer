using LightEngineCore.Components;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;

namespace LightGameServer.Game.Prefabs.Skills
{
    class FireBall : GameObject
    {
        public FireBall(GameLoop gameLoop, Vector2 pos, Vector2 pushDir) : base(gameLoop, "FireBall", new Rigidbody(gameLoop, 0.5f, 1.2f, pos, BodyType.Dynamic))
        {
            this.GetComponent<Rigidbody>().body.ApplyLinearImpulse(pushDir * pushDir);
        }
    }
}
