using LightEngine.PhysicsEngine.Dynamics;

namespace LightEngine.PhysicsEngine.Collision.Handlers
{
    public delegate void BroadphaseHandler(ref FixtureProxy proxyA, ref FixtureProxy proxyB);
}