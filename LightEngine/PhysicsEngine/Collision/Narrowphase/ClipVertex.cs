using LightEngine.PhysicsEngine.Collision.ContactSystem;
using LightEngine.PhysicsEngine.Primitives;

namespace LightEngine.PhysicsEngine.Collision.Narrowphase
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