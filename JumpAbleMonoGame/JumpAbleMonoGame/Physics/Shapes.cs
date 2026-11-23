using Microsoft.Xna.Framework;
 
namespace MyGame.Physics
{
    public enum ShapeType
    {
        Circle,
        Box,
        Polygon
    }
 
    /// <summary>
    /// Basisklasse aller Kollisionsformen. Alle Maße sind im lokalen Koordinatensystem
    /// des Körpers (Ursprung = Körperposition, Einheit = Pixel).
    /// </summary>
    public abstract class PhysicsShape
    {
        public abstract ShapeType Type { get; }
    }
 
    public sealed class CircleShape : PhysicsShape
    {
        public float Radius { get; }
        public override ShapeType Type => ShapeType.Circle;
 
        public CircleShape(float radius)
        {
            Radius = radius;
        }
    }
 
    /// <summary>Achsenparallele Box im lokalen Raum, zentriert auf den Körperursprung.</summary>
    public sealed class BoxShape : PhysicsShape
    {
        public float Width { get; }
        public float Height { get; }
        public Vector2 HalfExtents => new Vector2(Width * 0.5f, Height * 0.5f);
        public override ShapeType Type => ShapeType.Box;
 
        public BoxShape(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
 
    /// <summary>Konvexes Polygon, Eckpunkte gegen den Uhrzeigersinn, lokale Koordinaten.</summary>
    public sealed class PolygonShape : PhysicsShape
    {
        public Vector2[] Vertices { get; }
        public override ShapeType Type => ShapeType.Polygon;
 
        public PolygonShape(params Vector2[] vertices)
        {
            Vertices = vertices;
        }
    }
}