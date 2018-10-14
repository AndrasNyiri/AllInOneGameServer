using LightEngineCore.PhysicsEngine.Primitives;
using LightEngineCore.PhysicsEngine.Shared;

namespace LightEngineCore.PhysicsEngine.Dynamics.Solver
{
    public sealed class ContactVelocityConstraint
    {
        public int ContactIndex;
        public float Friction;
        public int IndexA;
        public int IndexB;
        public float InvIA, InvIB;
        public float InvMassA, InvMassB;
        public Mat22 K;
        public Vector2 Normal;
        public Mat22 NormalMass;
        public int PointCount;
        public VelocityConstraintPoint[] Points = new VelocityConstraintPoint[Settings.MaxManifoldPoints];
        public float Restitution;
        public float TangentSpeed;

        public ContactVelocityConstraint()
        {
            for (int i = 0; i < Settings.MaxManifoldPoints; i++)
            {
                Points[i] = new VelocityConstraintPoint();
            }
        }
    }
}