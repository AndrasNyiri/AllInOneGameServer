using Body = LightEngineCore.PhysicsEngine.Dynamics.Body;
using BodyFactory = LightEngineCore.PhysicsEngine.Factories.BodyFactory;
using BodyType = LightEngineCore.PhysicsEngine.Dynamics.BodyType;
using Vector2 = LightEngineCore.PhysicsEngine.Primitives.Vector2;
using World = LightEngineCore.PhysicsEngine.Dynamics.World;

namespace LightEngineCore.Components
{
    public class Rigidbody : Behaviour
    {
        public readonly Body body;
        public Rigidbody(World world, float radius, float density, Vector2 position, BodyType bType)
        {
            body = BodyFactory.CreateCircle(world, radius, density, position,
                bType, this);
        }

        public Rigidbody(World world, float width, float height, float density, Vector2 position, BodyType bType, float rotation = 0f)
        {
            body = BodyFactory.CreateRectangle(world, width, height, density, position, rotation,
                bType, this);
        }
    }
}
