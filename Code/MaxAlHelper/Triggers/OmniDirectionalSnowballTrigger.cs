namespace MaxAlHelper;


[CustomEntity("MaxAlHelper/OmniDirectionalSnowballTrigger")]
public class OmniDirectionalSnowballTrigger : Trigger {
    public float Speed;
    public float ResetTime;
    public bool DrawOutline;
    public string SpritePath;
    public float SineWaveFrequency;
    public float AppearAngle;
    public bool ReplaceExisting;
    public float SafeZoneSize;
    public float Offset;

    public OmniDirectionalSnowballTrigger(EntityData data, Vector2 offset) : base(data, offset) {
        SpritePath = data.Attr("spritePath", "snowball");
        Speed = data.Float("speed", 200f);
        ResetTime = data.Float("resetTime", 0.8f);
        SineWaveFrequency = data.Float("ySineWaveFrequency", 0.5f);
        DrawOutline = data.Bool("drawOutline", true);
        AppearAngle = data.Float("appearAngle", 0f);
        ReplaceExisting = data.Bool("replaceExisting", true);
        SafeZoneSize = data.Float("safeZoneSize", 64f);
        Offset = data.Float("offset", 0f);
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        OmniDirectionalSnowball snowball;
        if (ReplaceExisting && (snowball = Scene.Entities.FindFirst<OmniDirectionalSnowball>()) != null) {
            snowball.Speed = Speed;
            snowball.ResetTime = ResetTime;
            snowball.Sine.Frequency = SineWaveFrequency;
            if (snowball.Sprite.Path != SpritePath) {
                snowball.CreateSprite(SpritePath);
            }
            snowball.DrawOutline = DrawOutline;
            snowball.appearAngle = AppearAngle;
            snowball.SafeZoneSize = SafeZoneSize;
        } else {
            Scene.Add(new OmniDirectionalSnowball(SpritePath, Speed, ResetTime, SineWaveFrequency, DrawOutline, AppearAngle, SafeZoneSize, Offset));
        }
        RemoveSelf();
    }
}