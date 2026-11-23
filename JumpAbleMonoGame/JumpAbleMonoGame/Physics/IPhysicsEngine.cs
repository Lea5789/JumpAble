using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
 
namespace MyGame.Physics
{
    /// <summary>Ein Körper in der Physik-Welt. Wird von deiner Engine implementiert.</summary>
    public interface IPhysicsBody
    {
        int Id { get; }
        BodyType BodyType { get; }
        PhysicsShape Shape { get; }
 
        Vector2 Position { get; set; }
        float Rotation { get; set; }
        Vector2 LinearVelocity { get; set; }
        float AngularVelocity { get; set; }
 
        float Mass { get; }
        float Restitution { get; set; }
        float Friction { get; set; }
        bool IsSensor { get; set; }
        bool IsAwake { get; set; }
 
        /// <summary>Position/Rotation des vorherigen Physik-Schritts (für Render-Interpolation).</summary>
        Vector2 PreviousPosition { get; }
        float PreviousRotation { get; }
 
        object UserData { get; set; }
 
        void ApplyForce(Vector2 force);
        void ApplyForce(Vector2 force, Vector2 worldPoint);
        void ApplyImpulse(Vector2 impulse);
        void ApplyImpulse(Vector2 impulse, Vector2 worldPoint);
        void ApplyTorque(float torque);
    }
 
    public readonly struct CollisionEvent
    {
        public IPhysicsBody BodyA { get; }
        public IPhysicsBody BodyB { get; }
        /// <summary>Normale, zeigt von A nach B.</summary>
        public Vector2 Normal { get; }
        public Vector2 ContactPoint { get; }
        public float Penetration { get; }
 
        public CollisionEvent(IPhysicsBody a, IPhysicsBody b, Vector2 normal, Vector2 contactPoint, float penetration)
        {
            BodyA = a;
            BodyB = b;
            Normal = normal;
            ContactPoint = contactPoint;
            Penetration = penetration;
        }
    }
 
    public readonly struct RaycastHit
    {
        public IPhysicsBody Body { get; }
        public Vector2 Point { get; }
        public Vector2 Normal { get; }
        public float Distance { get; }
 
        public RaycastHit(IPhysicsBody body, Vector2 point, Vector2 normal, float distance)
        {
            Body = body;
            Point = point;
            Normal = normal;
            Distance = distance;
        }
    }
 
    /// <summary>
    /// Die Schnittstelle, die DEINE Physics-Engine implementieren muss.
    /// Der Rest des Programms spricht ausschließlich über diese Schnittstelle bzw.
    /// über <see cref="PhysicsWorld"/> mit der Physik.
    /// </summary>
    public interface IPhysicsEngine
    {
        /// <summary>Schwerkraft in Pixel/s² (y nach unten positiv, wie bei MonoGame).</summary>
        Vector2 Gravity { get; set; }
 
        IReadOnlyList<IPhysicsBody> Bodies { get; }
 
        /// <summary>Wird einmal von <see cref="PhysicsWorld"/> beim Erstellen aufgerufen.</summary>
        void Initialize();
 
        IPhysicsBody CreateBody(BodyDefinition definition);
        void RemoveBody(IPhysicsBody body);
        void Clear();
 
        /// <summary>Simuliert exakt einen Schritt mit fester Zeitdauer (Sekunden).</summary>
        void Step(float deltaTime);
 
        bool Raycast(Vector2 origin, Vector2 direction, float maxDistance, out RaycastHit hit);
        IEnumerable<IPhysicsBody> QueryArea(Rectangle worldArea);
 
        event Action<CollisionEvent> CollisionBegan;
        event Action<CollisionEvent> CollisionEnded;
    }
}
 