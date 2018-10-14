using LightEngineCore.Components;
using LightEngineCore.Loop;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineSerializeable.Utils;

namespace LightGameServer.Game.Prefabs.Static
{
    class Wall : GameObject
    {
        public Wall(GameLoop match, string name, float width, float height, Vector2 pos, float rot = 0)
            : base(match, name, new Rigidbody(match, width, height, 1f, pos, BodyType.Static, rot.ToRadians()))
        {
        }
    }
}
