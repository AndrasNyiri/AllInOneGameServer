using LightEngine.PhysicsEngine.Collision.ContactSystem;
using LightEngine.PhysicsEngine.Collision.Narrowphase;

namespace LightEngine.PhysicsEngine.Dynamics.Handlers
{
    public delegate void PreSolveHandler(Contact contact, ref Manifold oldManifold);
}