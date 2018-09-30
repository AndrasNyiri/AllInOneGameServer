using LightEngineCore.PhysicsEngine.Collision.ContactSystem;

namespace LightEngineCore.PhysicsEngine.Collision.Handlers
{
    /// <summary>
    /// This delegate is called when a contact is created
    /// </summary>
    public delegate bool BeginContactHandler(Contact contact);
}