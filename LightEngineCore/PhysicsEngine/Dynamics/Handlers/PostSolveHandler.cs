using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Dynamics.Solver;

namespace LightEngineCore.PhysicsEngine.Dynamics.Handlers
{
    public delegate void PostSolveHandler(Contact contact, ContactVelocityConstraint impulse);
}