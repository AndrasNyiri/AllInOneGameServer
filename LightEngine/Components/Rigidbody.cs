using Body = LightEngine.PhysicsEngine.Dynamics.Body;
using BodyFactory = LightEngine.PhysicsEngine.Factories.BodyFactory;
using BodyType = LightEngine.PhysicsEngine.Dynamics.BodyType;
using Vector2 = LightEngine.PhysicsEngine.Primitives.Vector2;
using World = LightEngine.PhysicsEngine.Dynamics.World;

namespace LightEngine.Components
{
    public class Rigidbody : Behaviour
    {
        public readonly Body body;
        public Rigidbody(World world, float radius, float density, Vector2 position, BodyType bType)
        {
            body = BodyFactory.CreateCircle(world, radius, density, position,
                bType);
        }

        public Rigidbody(World world, float width, float height, float density, Vector2 position, BodyType bType)
        {
            body = BodyFactory.CreateRectangle(world, width, height, density, position, 0f,
                bType);
        }
    }
}
