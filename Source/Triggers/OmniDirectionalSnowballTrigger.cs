using Celeste.Mod.Entities;
using Celeste.Mod.MaxAlHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.MaxAlHelper.Triggers;

[CustomEntity("MaxAlHelper/OmniDirectionalSnowballTrigger")]
public class OmniDirectionalSnowballTrigger : Trigger
{
    public float Speed;
    public float ResetTime;
    public bool DrawOutline;
    public string SpritePath;
    public float SineWaveFrequency;
    public float AppearAngle;
    public bool ReplaceExisting;
    public float SafeZoneSize;
    public float Offset;

    public OmniDirectionalSnowballTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        SpritePath = data.Attr("spritePath", "snowball");
        Speed = data.Float("speed", 200f);
        ResetTime = data.Float("resetTime", 0.8f);
        SineWaveFrequency = data.Float("ySineWaveFrequency", 0.5f);
        DrawOutline = data.Bool("drawOutline", true);
        AppearAngle = data.Float("appearAngle");
        ReplaceExisting = data.Bool("replaceExisting", true);
        SafeZoneSize = data.Float("safeZoneSize", 64f);
        Offset = data.Float("offset");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

        // hint: make your entities [Tracked] (see the snowball entity), and use Scene.Tracker.GetEntity
        // otherwise you'd be iterating through *every entity in the scene* which is slow
        // the tracker makes things faster, as each entity type is kept track of
        OmniDirectionalSnowball snowball = Scene.Tracker.GetEntity<OmniDirectionalSnowball>();

        if (ReplaceExisting && snowball is not null)
        {
            snowball.Speed = Speed;
            snowball.ResetTime = ResetTime;
            snowball.Sine.Frequency = SineWaveFrequency;
            if (snowball.Sprite.Path != SpritePath)
            {
                snowball.CreateSprite(SpritePath);
            }
            snowball.DrawOutline = DrawOutline;
            snowball.appearAngle = AppearAngle;
            snowball.SafeZoneSize = SafeZoneSize;
        }
        else
        {
            Scene.Add(new OmniDirectionalSnowball(
                SpritePath, Speed, ResetTime, SineWaveFrequency, DrawOutline, AppearAngle, SafeZoneSize, Offset));
        }
        RemoveSelf();
    }
}
