using LightEngine.PhysicsEngine.Collision.ContactSystem;
using LightEngine.PhysicsEngine.Dynamics;

namespace LightEngine.PhysicsEngine.Collision.Handlers
{
    public delegate void OnSeparationHandler(Fixture fixtureA, Fixture fixtureB, Contact contact);
}