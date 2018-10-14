using LightEngineCore.PhysicsEngine.Dynamics;

namespace LightEngineCore.PhysicsEngine.Collision.Handlers
{
    public delegate void BroadphaseHandler(ref FixtureProxy proxyA, ref FixtureProxy proxyB);
}