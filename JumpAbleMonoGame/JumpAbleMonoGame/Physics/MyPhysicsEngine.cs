using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
 
namespace MyGame.Physics
{
    /// <summary>
    /// HIER kommt DEINE Physics-Engine hinein.
    /// Die Klasse kompiliert und läuft bereits (es passiert nur noch nichts),
    /// du musst nur die mit TODO markierten Stellen füllen.
    /// </summary>
    public sealed class MyPhysicsEngine : IPhysicsEngine
    {
        private readonly List<Body> _bodies = new List<Body>();
        private readonly List<IPhysicsBody> _bodyView = new List<IPhysicsBody>();
        private int _nextId = 1;
 
        public Vector2 Gravity { get; set; }
        public IReadOnlyList<IPhysicsBody> Bodies => _bodyView;
 
        public event Action<CollisionEvent> CollisionBegan;
        public event Action<CollisionEvent> CollisionEnded;
 
        public void Initialize()
        {
            // TODO: Einmalige Initialisierung (Broadphase, Spatial Hash, ...)
        }
 
        public IPhysicsBody CreateBody(BodyDefinition def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (def.Shape == null) throw new ArgumentException("BodyDefinition.Shape darf nicht null sein.");
 
            var body = new Body(_nextId++, def);
            _bodies.Add(body);
            _bodyView.Add(body);
            return body;
        }
 
        public void RemoveBody(IPhysicsBody body)
        {
            if (body is Body b)
            {
                _bodies.Remove(b);
                _bodyView.Remove(b);
            }
        }
 
        public void Clear()
        {
            _bodies.Clear();
            _bodyView.Clear();
        }
 
        public void Step(float dt)
        {
            foreach (var b in _bodies)
            {
                b.PreviousPosition = b.Position;
                b.PreviousRotation = b.Rotation;
            }
 
            // TODO: Deine Simulation:
            //   1. Kräfte (Gravity * b.GravityScale, b.Force) in Geschwindigkeit integrieren
            //   2. Broadphase / Narrowphase (Kollisionserkennung anhand von b.Shape)
            //   3. Kollisionsauflösung (Impulse, Restitution, Friction)
            //   4. Position/Rotation integrieren
            //   5. Bei neuer / beendeter Berührung RaiseCollisionBegan / RaiseCollisionEnded aufrufen
 
            foreach (var b in _bodies)
                b.ClearForces();
        }
 
        public bool Raycast(Vector2 origin, Vector2 direction, float maxDistance, out RaycastHit hit)
        {
            // TODO (optional)
            hit = default;
            return false;
        }
 
        public IEnumerable<IPhysicsBody> QueryArea(Rectangle worldArea)
        {
            // TODO (optional)
            yield break;
        }
 
        private void RaiseCollisionBegan(CollisionEvent e) => CollisionBegan?.Invoke(e);
        private void RaiseCollisionEnded(CollisionEvent e) => CollisionEnded?.Invoke(e);
 
        // ------------------------------------------------------------------
        // Interne Körper-Klasse. Felder bei Bedarf erweitern (Inertia, AABB, ...).
        // ------------------------------------------------------------------
        private sealed class Body : IPhysicsBody
        {
            public int Id { get; }
            public BodyType BodyType { get; }
            public PhysicsShape Shape { get; }
 
            public Vector2 Position { get; set; }
            public float Rotation { get; set; }
            public Vector2 LinearVelocity { get; set; }
            public float AngularVelocity { get; set; }
 
            public float Mass { get; }
            public float InverseMass { get; }
            public float Restitution { get; set; }
            public float Friction { get; set; }
            public float LinearDamping { get; }
            public float AngularDamping { get; }
            public float GravityScale { get; }
            public bool FixedRotation { get; }
            public bool IsSensor { get; set; }
            public bool IsAwake { get; set; } = true;
            public uint CategoryBits { get; }
            public uint MaskBits { get; }
 
            public Vector2 PreviousPosition { get; set; }
            public float PreviousRotation { get; set; }
            public object UserData { get; set; }
 
            public Vector2 Force;
            public float Torque;
 
            public Body(int id, BodyDefinition def)
            {
                Id = id;
                BodyType = def.BodyType;
                Shape = def.Shape;
                Position = PreviousPosition = def.Position;
                Rotation = PreviousRotation = def.Rotation;
                LinearVelocity = def.LinearVelocity;
                AngularVelocity = def.AngularVelocity;
                Restitution = def.Restitution;
                Friction = def.Friction;
                LinearDamping = def.LinearDamping;
                AngularDamping = def.AngularDamping;
                GravityScale = def.GravityScale;
                FixedRotation = def.FixedRotation;
                IsSensor = def.IsSensor;
                CategoryBits = def.CategoryBits;
                MaskBits = def.MaskBits;
                UserData = def.UserData;
 
                // TODO: Masse aus Shape-Fläche * Density berechnen (hier nur Platzhalter).
                Mass = BodyType == BodyType.Dynamic ? Math.Max(def.Density, 0.0001f) : 0f;
                InverseMass = Mass > 0f ? 1f / Mass : 0f;
            }
 
            public void ApplyForce(Vector2 force) => Force += force;
 
            public void ApplyForce(Vector2 force, Vector2 worldPoint)
            {
                Force += force;
                Vector2 r = worldPoint - Position;
                Torque += r.X * force.Y - r.Y * force.X;
            }
 
            public void ApplyImpulse(Vector2 impulse) => LinearVelocity += impulse * InverseMass;
 
            public void ApplyImpulse(Vector2 impulse, Vector2 worldPoint)
            {
                LinearVelocity += impulse * InverseMass;
                // TODO: AngularVelocity += InverseInertia * Cross(r, impulse);
            }
 
            public void ApplyTorque(float torque) => Torque += torque;
 
            public void ClearForces()
            {
                Force = Vector2.Zero;
                Torque = 0f;
            }
        }
    }
}
 