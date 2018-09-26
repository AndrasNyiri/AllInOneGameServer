using System;

namespace LightEngine.PhysicsEngine.Shared.Optimization
{
    public interface IPoolable<T> : IDisposable where T : IPoolable<T>
    {
        void Reset();

        Pool<T> Pool { set; }
    }
}
