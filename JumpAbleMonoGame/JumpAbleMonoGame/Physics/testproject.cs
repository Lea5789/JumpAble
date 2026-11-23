using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;

namespace JumpAbleMonoGame;


public class Testproject : Game
{
    private const float PixelsPerMeter = 64f;
    private const float BallRadius = 0.75f;          // Meter
    private static readonly Vector2 GroundSize = new(8f, 1f);

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private World _world;
    private Body _playerBody;
    private Body _groundBody;

    private Texture2D _pixel;     // 1x1 weiß, für Rechtecke
    private Texture2D _circle;    // weißer Kreis, für den Ball

    public Testproject()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Y zeigt in der Physik nach OBEN, beim Zeichnen wird umgerechnet
        _world = new World(new Vector2(0f, -9.8f));

        // Ball startet 5 m über dem Boden
        _playerBody = _world.CreateBody(new Vector2(0f, 5f), 0f, BodyType.Dynamic);
        Fixture pfixture = _playerBody.CreateCircle(BallRadius, 1f);
        pfixture.Restitution = 0.3f;
        pfixture.Friction = 0.5f;

        // Boden: Oberkante bei y = 0
        _groundBody = _world.CreateBody(new Vector2(0f, -GroundSize.Y / 2f), 0f, BodyType.Static);
        Fixture gfixture = _groundBody.CreateRectangle(GroundSize.X, GroundSize.Y, 1f, Vector2.Zero);
        gfixture.Restitution = 0.3f;
        gfixture.Friction = 0.5f;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        const int size = 64;
        var data = new Color[size * size];
        float r = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - r, dy = y + 0.5f - r;
                data[y * size + x] = dx * dx + dy * dy <= r * r ? Color.White : Color.Transparent;
            }
        _circle = new Texture2D(GraphicsDevice, size, size);
        _circle.SetData(data);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Microsoft.Xna.Framework.Input.Keyboard.GetState()
                .IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            Exit();

        // Standardmäßig läuft MonoGame mit festem Zeitschritt (60 Hz)
        _world.Step(1f / 60f);

        base.Update(gameTime);
    }

    // Physik (Meter, Y oben) -> Bildschirm (Pixel, Y unten)
    private Vector2 ToScreen(Vector2 meters)
    {
        var vp = GraphicsDevice.Viewport;
        var origin = new Vector2(vp.Width / 2f, vp.Height * 0.75f);
        return origin + new Vector2(meters.X, -meters.Y) * PixelsPerMeter;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        // Boden
        _spriteBatch.Draw(_pixel, ToScreen(_groundBody.Position), null, Color.SaddleBrown,
            0f, new Vector2(0.5f), GroundSize * PixelsPerMeter, SpriteEffects.None, 0f);

        // Ball
        float scale = BallRadius * 2f * PixelsPerMeter / _circle.Width;
        _spriteBatch.Draw(_circle, ToScreen(_playerBody.Position), null, Color.OrangeRed,
            -_playerBody.Rotation, new Vector2(_circle.Width / 2f), scale, SpriteEffects.None, 0f);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}