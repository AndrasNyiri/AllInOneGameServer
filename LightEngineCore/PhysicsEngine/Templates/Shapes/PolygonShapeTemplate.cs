using LightEngineCore.PhysicsEngine.Collision.Shapes;
using LightEngineCore.PhysicsEngine.Shared;

namespace LightEngineCore.PhysicsEngine.Templates.Shapes
{
    public class PolygonShapeTemplate : ShapeTemplate
    {
        public PolygonShapeTemplate() : base(ShapeType.Polygon) { }

        public Vertices Vertices { get; set; }
    }
}