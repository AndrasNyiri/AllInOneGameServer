using LightEngine.PhysicsEngine.Collision.ContactSystem;
using LightEngine.PhysicsEngine.Dynamics;
using LightEngine.PhysicsEngine.Dynamics.Solver;

namespace LightEngine.PhysicsEngine.Collision.Handlers
{
    public delegate void AfterCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse);
}