using LightEngine.PhysicsEngine.Primitives;

namespace LightEngine.PhysicsEngine.Dynamics.Solver
{
    public sealed class VelocityConstraintPoint
    {
        public float NormalImpulse;
        public float NormalMass;
        public Vector2 rA;
        public Vector2 rB;
        public float TangentImpulse;
        public float TangentMass;
        public float VelocityBias;
    }
}