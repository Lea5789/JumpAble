namespace JumpAbleMonoGame.Physics;

public class testproject
{
    _world = new World();

// Spieler: Durchmesser 1,5 m
    _playerBody = _world.CreateBody(new Vector2(0, 0.75f), 0, BodyType.Dynamic);
    Fixture pfixture = _playerBody.CreateCircle(0.75f, 1f);
    pfixture.Restitution = 0.3f;
    pfixture.Friction = 0.5f;

// Boden: 8 x 1 m
    _groundBody = _world.CreateBody(new Vector2(0, -0.5f), 0, BodyType.Static);
    Fixture gfixture = _groundBody.CreateRectangle(8f, 1f, 1f, Vector2.Zero);

// pro Update
    _world.Step(1f / 60f);
}