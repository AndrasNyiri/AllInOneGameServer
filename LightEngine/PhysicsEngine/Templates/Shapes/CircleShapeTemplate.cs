using LightEngine.PhysicsEngine.Collision.Shapes;
using LightEngine.PhysicsEngine.Primitives;

namespace LightEngine.PhysicsEngine.Templates.Shapes
{
    public class CircleShapeTemplate : ShapeTemplate
    {
        public CircleShapeTemplate() : base(ShapeType.Circle) { }

        /// <summary>
        /// Get or set the position of the circle
        /// </summary>
        public Vector2 Position { get; set; }
    }
}