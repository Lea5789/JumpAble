using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
 
namespace MyGame.Physics
{
    /// <summary>
    /// Einzige Klasse, die der Rest des Programms kennen muss.
    /// Kapselt eine beliebige <see cref="IPhysicsEngine"/> und kümmert sich um
    /// Fixed-Timestep, Pause und Zeitlupe.
    /// </summary>
    public sealed class PhysicsWorld
    {
        private readonly IPhysicsEngine _engine;
        private float _accumulator;
 
        public IPhysicsEngine Engine => _engine;
 
        /// <summary>Dauer eines Physik-Schritts in Sekunden (Standard 1/60).</summary>
        public float FixedTimeStep { get; set; } = 1f / 60f;
 
        /// <summary>Maximale Schritte pro Frame, verhindert die "Spiral of Death".</summary>
        public int MaxStepsPerFrame { get; set; } = 5;
 
        public float TimeScale { get; set; } = 1f;
        public bool Paused { get; set; }
 
        /// <summary>0..1: Anteil des nächsten Schritts, nützlich für interpoliertes Rendern.</summary>
        public float InterpolationAlpha => FixedTimeStep > 0f ? _accumulator / FixedTimeStep : 0f;
 
        public Vector2 Gravity
        {
            get => _engine.Gravity;
            set => _engine.Gravity = value;
        }
 
        public IReadOnlyList<IPhysicsBody> Bodies => _engine.Bodies;
 
        public event Action<CollisionEvent> CollisionBegan;
        public event Action<CollisionEvent> CollisionEnded;
 
        public PhysicsWorld(IPhysicsEngine engine, Vector2? gravity = null)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _engine.Gravity = gravity ?? new Vector2(0f, 980f);
            _engine.CollisionBegan += e => CollisionBegan?.Invoke(e);
            _engine.CollisionEnded += e => CollisionEnded?.Invoke(e);
            _engine.Initialize();
        }
 
        public void Update(GameTime gameTime) => Update((float)gameTime.ElapsedGameTime.TotalSeconds);
 
        public void Update(float deltaSeconds)
        {
            if (Paused) return;
 
            _accumulator += deltaSeconds * TimeScale;
 
            int steps = 0;
            while (_accumulator >= FixedTimeStep && steps < MaxStepsPerFrame)
            {
                _engine.Step(FixedTimeStep);
                _accumulator -= FixedTimeStep;
                steps++;
            }
 
            // Wenn das Limit erreicht wurde: Rest verwerfen.
            if (steps == MaxStepsPerFrame && _accumulator >= FixedTimeStep)
                _accumulator = 0f;
        }
 
        /// <summary>Genau einen Schritt ausführen (z. B. für Debugging im Pausenmodus).</summary>
        public void StepOnce() => _engine.Step(FixedTimeStep);
 
        public IPhysicsBody CreateBody(BodyDefinition definition) => _engine.CreateBody(definition);
 
        public IPhysicsBody CreateBox(Vector2 position, float width, float height,
            BodyType type = BodyType.Dynamic, object userData = null)
        {
            return _engine.CreateBody(new BodyDefinition
            {
                Position = position,
                BodyType = type,
                Shape = new BoxShape(width, height),
                UserData = userData
            });
        }
 
        public IPhysicsBody CreateCircle(Vector2 position, float radius,
            BodyType type = BodyType.Dynamic, object userData = null)
        {
            return _engine.CreateBody(new BodyDefinition
            {
                Position = position,
                BodyType = type,
                Shape = new CircleShape(radius),
                UserData = userData
            });
        }
 
        public void RemoveBody(IPhysicsBody body) => _engine.RemoveBody(body);
        public void Clear() => _engine.Clear();
 
        public bool Raycast(Vector2 origin, Vector2 direction, float maxDistance, out RaycastHit hit)
            => _engine.Raycast(origin, direction, maxDistance, out hit);
 
        public IEnumerable<IPhysicsBody> QueryArea(Rectangle area) => _engine.QueryArea(area);
    }
}
 