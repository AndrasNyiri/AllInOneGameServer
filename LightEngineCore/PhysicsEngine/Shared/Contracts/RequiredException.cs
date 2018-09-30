using System;

namespace LightEngineCore.PhysicsEngine.Shared.Contracts
{
    public class RequiredException : Exception
    {
        public RequiredException(string message) : base(message) { }
    }
}