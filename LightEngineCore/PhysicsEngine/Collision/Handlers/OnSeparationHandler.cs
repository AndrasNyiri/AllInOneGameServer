using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Dynamics;

namespace LightEngineCore.PhysicsEngine.Collision.Handlers
{
    public delegate void OnSeparationHandler(Fixture fixtureA, Fixture fixtureB, Contact contact);
}