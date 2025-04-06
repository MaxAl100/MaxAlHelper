namespace MaxAlHelper;

public class OmniDirectionalSnowball : Entity
{
    public float Speed;
    public float ResetTime;
    public bool DrawOutline;
    public float appearAngle;
    public float SafeZoneSize;
    public float Offset;
    private Vector2 moveDir;


    private bool leaving;

    public OmniDirectionalSnowball(string spritePath = "snowball", float speed = 200f, float resetTime = 0.8f,
                          float sineWaveFrequency = 0.5f, bool drawOutline = true,
                          float ang = 0f,
                          float safeZoneSize = 64f, float offset = 0)
    {
        appearAngle = ang;
        moveDir = Calc.AngleToVector(MathHelper.ToRadians(appearAngle), 1.0f);
        Speed = speed;

        ResetTime = resetTime;
        DrawOutline = drawOutline;
        Depth = -12500;

        Collider = new Hitbox(12f, 9f, -5f, -2f);
        bounceCollider = new Hitbox(16f, 6f, -6f, -8f);

        Add(new PlayerCollider(OnPlayer, null, null));
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider, null));
        Add(Sine = new SineWave(sineWaveFrequency, 0f));

        CreateSprite(spritePath);

        Sprite!.Play("spin", false, false);
        Add(spawnSfx = new SoundSource());
        SafeZoneSize = safeZoneSize;
        Offset = offset;
    }

    public void StartLeaving()
    {
        leaving = true;
    }

    public void CreateSprite(string path)
    {
        Sprite?.RemoveSelf();
        Add(Sprite = GFX.SpriteBank.Create(path));

    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        ResetPosition();
    }

    private void ResetPosition()
    {
        Player player = level.Tracker.GetEntity<Player>();
        if (player == null || !CheckIfPlayerOutOfBounds(player))
        {
            resetTimer = 0.05f;
            return;
        }

        spawnSfx.Play("event:/game/04_cliffside/snowball_spawn");

        Collidable = Visible = true;
        resetTimer = 0f;
        Sine.Reset();
        Sprite.Play("spin", false, false);

        Vector2 playerPos = player.Center;
        Vector2 offsetDir = -moveDir.SafeNormalize(); 
        Position = playerPos + offsetDir * (SafeZoneSize + 10f); 
    }



    private bool CheckIfPlayerOutOfBounds(Player player)
    {
        if (player is null)
            return false;

        // Calculate the direction *toward* the player
        Vector2 toPlayer = (player.Center - Position).SafeNormalize();
        float dot = Vector2.Dot(moveDir, toPlayer);

        // Only spawn if the snowball would move roughly toward the player
        return dot > 0.5f;
    }



    private bool IsOutOfBounds()
    {
        Vector2 screenSize = new Vector2(Engine.Width, Engine.Height);
        Vector2 screenCenter = level.Camera.Position + screenSize / 2f;
        float maxDistance = 500f; 
        return Vector2.DistanceSquared(Position, screenCenter) > maxDistance * maxDistance;
    }


    private void Destroy()
    {
        Collidable = false;
        Sprite.Play("break", false, false);
    }

    private void OnPlayer(Player player)
    {
        player.Die(new Vector2(-1f, 0f), false, true);
        Destroy();
        Audio.Play("event:/game/04_cliffside/snowball_impact", Position);
    }

    private void OnPlayerBounce(Player player)
    {
        if (!CollideCheck(player))
        {
            Celeste.Celeste.Freeze(0.1f);
            player.Bounce(Top - 2f);
            Destroy();
            Audio.Play("event:/game/general/thing_booped", Position);
        }
    }

    public override void Update()
    {
        base.Update();

        Position += moveDir * Speed * Engine.DeltaTime;


        if (IsOutOfBounds())
        {
            if (leaving)
            {
                RemoveSelf();
                return;
            }
            resetTimer += Engine.DeltaTime;
            if (resetTimer >= ResetTime)
            {
                ResetPosition();
            }
        }
    }

    public override void Render()
    {
        if (DrawOutline)
            Sprite.DrawOutline(1);
        base.Render();
    }

    public Sprite Sprite;
    private float resetTimer;
    private Level level;
    public SineWave Sine;
    private SoundSource spawnSfx;
    private Collider bounceCollider;

}