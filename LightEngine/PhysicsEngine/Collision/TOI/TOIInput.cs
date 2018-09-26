using LightEngine.PhysicsEngine.Collision.Distance;

namespace LightEngine.PhysicsEngine.Collision.TOI
{
    /// <summary>
    /// Input parameters for CalculateTimeOfImpact
    /// </summary>
    public struct TOIInput
    {
        public DistanceProxy ProxyA;
        public DistanceProxy ProxyB;
        public Sweep SweepA;
        public Sweep SweepB;
        public float TMax; // defines sweep interval [0, tMax]
    }
}