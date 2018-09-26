using LightEngine.PhysicsEngine.Collision.Shapes;
using LightEngine.PhysicsEngine.Shared;

namespace LightEngine.PhysicsEngine.Templates.Shapes
{
    public class PolygonShapeTemplate : ShapeTemplate
    {
        public PolygonShapeTemplate() : base(ShapeType.Polygon) { }

        public Vertices Vertices { get; set; }
    }
}