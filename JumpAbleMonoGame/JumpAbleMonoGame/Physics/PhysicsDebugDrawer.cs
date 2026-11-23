using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
 
namespace MyGame.Physics
{
    /// <summary>
    /// Zeichnet alle Körper als Umrisse, damit du deine Engine sofort sehen und testen kannst.
    /// Nutzung: Innerhalb von spriteBatch.Begin()/End() aufrufen.
    /// </summary>
    public sealed class PhysicsDebugDrawer : IDisposable
    {
        private readonly Texture2D _pixel;
 
        public Color StaticColor { get; set; } = Color.Gray;
        public Color KinematicColor { get; set; } = Color.CornflowerBlue;
        public Color DynamicColor { get; set; } = Color.LimeGreen;
        public Color SleepingColor { get; set; } = Color.DarkOliveGreen;
        public float LineThickness { get; set; } = 1.5f;
        public int CircleSegments { get; set; } = 24;
 
        public PhysicsDebugDrawer(GraphicsDevice graphicsDevice)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
 
        public void Draw(SpriteBatch spriteBatch, PhysicsWorld world)
        {
            foreach (var body in world.Bodies)
                DrawBody(spriteBatch, body);
        }
 
        public void DrawBody(SpriteBatch sb, IPhysicsBody body)
        {
            Color color = body.BodyType switch
            {
                BodyType.Static => StaticColor,
                BodyType.Kinematic => KinematicColor,
                _ => body.IsAwake ? DynamicColor : SleepingColor
            };
 
            switch (body.Shape)
            {
                case CircleShape c:
                    DrawCircle(sb, body.Position, c.Radius, color);
                    // Linie zeigt die Rotation
                    DrawLine(sb, body.Position,
                        body.Position + Rotate(new Vector2(c.Radius, 0f), body.Rotation), color);
                    break;
 
                case BoxShape b:
                    Vector2 h = b.HalfExtents;
                    DrawPolygon(sb, body, color,
                        new Vector2(-h.X, -h.Y), new Vector2(h.X, -h.Y),
                        new Vector2(h.X, h.Y), new Vector2(-h.X, h.Y));
                    break;
 
                case PolygonShape p:
                    DrawPolygon(sb, body, color, p.Vertices);
                    break;
            }
        }
 
        private void DrawPolygon(SpriteBatch sb, IPhysicsBody body, Color color, params Vector2[] localVertices)
        {
            for (int i = 0; i < localVertices.Length; i++)
            {
                Vector2 a = body.Position + Rotate(localVertices[i], body.Rotation);
                Vector2 b = body.Position + Rotate(localVertices[(i + 1) % localVertices.Length], body.Rotation);
                DrawLine(sb, a, b, color);
            }
        }
 
        public void DrawCircle(SpriteBatch sb, Vector2 center, float radius, Color color)
        {
            Vector2 prev = center + new Vector2(radius, 0f);
            for (int i = 1; i <= CircleSegments; i++)
            {
                float angle = MathHelper.TwoPi * i / CircleSegments;
                Vector2 next = center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                DrawLine(sb, prev, next, color);
                prev = next;
            }
        }
 
        public void DrawLine(SpriteBatch sb, Vector2 a, Vector2 b, Color color)
        {
            Vector2 d = b - a;
            float length = d.Length();
            if (length < 0.0001f) return;
            float angle = (float)Math.Atan2(d.Y, d.X);
            sb.Draw(_pixel, a, null, color, angle, Vector2.Zero,
                new Vector2(length, LineThickness), SpriteEffects.None, 0f);
        }
 
        private static Vector2 Rotate(Vector2 v, float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }
 
        public void Dispose() => _pixel.Dispose();
    }
}
 