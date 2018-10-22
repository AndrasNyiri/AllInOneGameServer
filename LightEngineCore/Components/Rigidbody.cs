using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Dynamics;
using Body = LightEngineCore.PhysicsEngine.Dynamics.Body;
using BodyFactory = LightEngineCore.PhysicsEngine.Factories.BodyFactory;
using BodyType = LightEngineCore.PhysicsEngine.Dynamics.BodyType;
using Vector2 = LightEngineCore.PhysicsEngine.Primitives.Vector2;

namespace LightEngineCore.Components
{
    public class Rigidbody : Behaviour
    {
        public readonly Body body;
        private readonly GameLoop _gameLoop;

        public Rigidbody(GameLoop gameLoop, float radius, float density, Vector2 position, BodyType bType)
        {
            _gameLoop = gameLoop;
            body = BodyFactory.CreateCircle(gameLoop.physicsWorld, radius, density, position,
                bType, this);
            body.OnCollision += OnBodyCollision;
            if (bType == BodyType.Dynamic) body.IsBullet = true;
        }

        public Rigidbody(GameLoop gameLoop, float width, float height, float density, Vector2 position, BodyType bType, float rotation = 0f)
        {
            _gameLoop = gameLoop;
            body = BodyFactory.CreateRectangle(gameLoop.physicsWorld, width, height, density, position, rotation,
                bType, this);
            body.OnCollision += OnBodyCollision;
            if (bType == BodyType.Dynamic) body.IsBullet = true;
        }


        private void OnBodyCollision(Fixture myFixture, Fixture otherFixture, Contact contact)
        {
            var otherGo = _gameLoop.GetGameObjectByBody(otherFixture.Body);
            this.gameObject.InvokeCollidedDelegate(otherGo);
        }

    }
}
