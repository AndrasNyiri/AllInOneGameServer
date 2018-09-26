using System;

namespace LightEngine.PhysicsEngine.Shared.Contracts
{
    public class RequiredException : Exception
    {
        public RequiredException(string message) : base(message) { }
    }
}