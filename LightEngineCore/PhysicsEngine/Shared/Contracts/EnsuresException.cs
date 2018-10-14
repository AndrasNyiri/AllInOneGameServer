using System;

namespace LightEngineCore.PhysicsEngine.Shared.Contracts
{
    public class EnsuresException : Exception
    {
        public EnsuresException(string message) : base(message) { }
    }
}