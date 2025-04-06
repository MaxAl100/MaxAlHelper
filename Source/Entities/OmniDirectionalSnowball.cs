using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper.Entities;

// make the type tracked, so that we can find it easily
// there's no real benefit to *not* having it tracked
[Tracked]
public class OmniDirectionalSnowball : Entity
{
    // note: i haven't had time to actually look into the logic. it's almost 2 am.

    public float Speed;
    public float ResetTime;
    public bool DrawOutline;
    public float appearAngle;
    public float SafeZoneSize;
    public float Offset;
    private Vector2 moveDir;

    public Sprite Sprite;
    private float resetTimer;
    private Level level;
    public SineWave Sine;
    private SoundSource spawnSfx;
    private Collider bounceCollider;

    private bool leaving;

    // tip: calculate the camera center offset once and store it for later
    // plus, you were using the wrong fields - you meant GameWidth/GameHeight constants
    private static readonly Vector2 CameraCenterOffset = new Vector2(Celeste.GameWidth, Celeste.GameHeight) / 2;

    public OmniDirectionalSnowball(
        string spritePath = "snowball",
        float speed = 200f,
        float resetTime = 0.8f,
        float sineWaveFrequency = 0.5f,
        bool drawOutline = true,
        float ang = 0f,
        float safeZoneSize = 64f,
        float offset = 0)
    {
        appearAngle = ang;
        moveDir = Calc.AngleToVector(appearAngle.ToRad(), 1.0f);
        Speed = speed;

        ResetTime = resetTime;
        DrawOutline = drawOutline;

        // hint: use the constants in the Depths class, then subtract/add based on whether
        // you want to update or render *before* or *after* a certain depth
        // - the player has depth 0 - you generally don't want your entities to have depth 0
        // - lower depths mean "update earlier" / "closer in front"
        // - higher depths mean "update later" / "further in back"
        Depth = Depths.Enemy;

        Collider = new Hitbox(12f, 9f, -5f, -2f);
        bounceCollider = new Hitbox(16f, 6f, -6f, -8f);

        Add(new PlayerCollider(OnPlayer));
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
        Add(Sine = new SineWave(sineWaveFrequency, 0f));

        CreateSprite(spritePath);

        Sprite.Play("spin");
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
        if (player is null || !CheckIfPlayerOutOfBounds(player))
        {
            resetTimer = 0.05f;
            return;
        }

        // hint: you can use the game's SFX class for the game's sound effects and stuff,
        // using strings directly is fine as well, but prone to typos
        // this event can be found in SFX.game_04_snowball_spawn
        spawnSfx.Play("event:/game/04_cliffside/snowball_spawn");

        Collidable = Visible = true;
        resetTimer = 0f;
        Sine.Reset();
        Sprite.Play("spin");

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
        // tip: make this a constant to optimize it a bit
        // delete the const keyword if you want to turn it into a variable again
        const float maxDistance = 500f;
        Vector2 screenCenter = level.Camera.Position + CameraCenterOffset;
        return Vector2.DistanceSquared(Position, screenCenter) > maxDistance * maxDistance;
    }

    private void Destroy()
    {
        Collidable = false;
        Sprite.Play("break");
    }

    private void OnPlayer(Player player)
    {
        player.Die(-Vector2.UnitX);
        Destroy();

        // hint: this event can be found in SFX.game_04_snowball_impact
        Audio.Play("event:/game/04_cliffside/snowball_impact", Position);
    }

    private void OnPlayerBounce(Player player)
    {
        // hint: invert the condition and return early, since there's nothing more to do
        // this is called a guard clause
        if (CollideCheck(player))
            return;

        Celeste.Freeze(0.1f);
        player.Bounce(Top - 2f);
        Destroy();

        // hint: this event can be found in SFX.game_gen_thing_booped
        Audio.Play("event:/game/general/thing_booped", Position);
    }

    public override void Update()
    {
        base.Update();

        Position += moveDir * Speed * Engine.DeltaTime;

        // hint: invert the condition and return early, since there's nothing more to do
        if (!IsOutOfBounds())
            return;

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

    public override void Render()
    {
        if (DrawOutline)
            Sprite.DrawOutline();

        base.Render();
    }
}
