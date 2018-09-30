using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Collision.Narrowphase;

namespace LightEngineCore.PhysicsEngine.Dynamics.Handlers
{
    public delegate void PreSolveHandler(Contact contact, ref Manifold oldManifold);
}