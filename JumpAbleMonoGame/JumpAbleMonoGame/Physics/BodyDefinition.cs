using Microsoft.Xna.Framework;
 
namespace MyGame.Physics
{
    public enum BodyType
    {
        /// <summary>Bewegt sich nie (Boden, Wände).</summary>
        Static,
        /// <summary>Wird per Code bewegt (Velocity), reagiert nicht auf Kräfte/Kollisionen.</summary>
        Kinematic,
        /// <summary>Voll simuliert.</summary>
        Dynamic
    }
 
    /// <summary>
    /// Beschreibung eines Körpers. Die anderen Programmierer füllen diese Klasse aus,
    /// deine Engine erzeugt daraus einen <see cref="IPhysicsBody"/>.
    /// </summary>
    public sealed class BodyDefinition
    {
        public BodyType BodyType { get; set; } = BodyType.Dynamic;
        public PhysicsShape Shape { get; set; }
 
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }              // Radiant
        public Vector2 LinearVelocity { get; set; }
        public float AngularVelocity { get; set; }       // Radiant/Sekunde
 
        public float Density { get; set; } = 1f;
        public float Restitution { get; set; } = 0.2f;   // Bounciness 0..1
        public float Friction { get; set; } = 0.4f;
        public float LinearDamping { get; set; } = 0f;
        public float AngularDamping { get; set; } = 0f;
        public float GravityScale { get; set; } = 1f;
 
        public bool FixedRotation { get; set; }
        /// <summary>Sensor = löst Collision-Events aus, kollidiert aber nicht physikalisch (Trigger).</summary>
        public bool IsSensor { get; set; }
 
        /// <summary>Kollisionsfilter: Zu welcher Kategorie gehört der Körper / mit welchen kollidiert er.</summary>
        public uint CategoryBits { get; set; } = 0x0001;
        public uint MaskBits { get; set; } = 0xFFFFFFFF;
 
        /// <summary>Frei nutzbar, z. B. Referenz auf das zugehörige GameObject.</summary>
        public object UserData { get; set; }
    }
}