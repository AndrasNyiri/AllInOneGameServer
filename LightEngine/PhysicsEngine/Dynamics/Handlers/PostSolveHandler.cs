using LightEngine.PhysicsEngine.Collision.ContactSystem;
using LightEngine.PhysicsEngine.Dynamics.Solver;

namespace LightEngine.PhysicsEngine.Dynamics.Handlers
{
    public delegate void PostSolveHandler(Contact contact, ContactVelocityConstraint impulse);
}