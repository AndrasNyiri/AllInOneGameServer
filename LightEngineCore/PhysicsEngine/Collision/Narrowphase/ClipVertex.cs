using LightEngineCore.PhysicsEngine.Collision.ContactSystem;
using LightEngineCore.PhysicsEngine.Primitives;

namespace LightEngineCore.PhysicsEngine.Collision.Narrowphase
{
    /// <summary>
    /// Used for computing contact manifolds.
    /// </summary>
    internal struct ClipVertex
    {
        public ContactID ID;
        public Vector2 V;
    }
}