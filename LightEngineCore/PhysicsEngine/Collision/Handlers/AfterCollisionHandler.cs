using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Dynamics;
using LightEngineCore.PhysicsEngine.Dynamics.Solver;

namespace LightEngineCore.PhysicsEngine.Collision.Handlers
{
    public delegate void AfterCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse);
}