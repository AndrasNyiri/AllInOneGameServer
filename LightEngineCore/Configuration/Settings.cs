using Vector2 = LightEngineCore.PhysicsEngine.Primitives.Vector2;

namespace LightEngineCore.Configuration
{
    public static class Settings
    {
        public static float targetFrameRate = 500f;
        public static float gravity = 9.8f;
        public static float velocityLimit = 50f;
        public static float belowGround = -35f;
        public static Vector2 drawOrigin = new Vector2(10, 10);
    }
}
