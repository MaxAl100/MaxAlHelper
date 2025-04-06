namespace MaxAlHelper;

[CustomEntity("MaxAlHelper/OmniDirectionalSnowballTrigger")]
public class StopOmniDirectionalSnowballTrigger : Trigger {
    public StopOmniDirectionalSnowballTrigger(EntityData data, Vector2 offset) : base(data, offset) {
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        foreach (OmniDirectionalSnowball snowball in Scene.Entities.FindAll<OmniDirectionalSnowball>()) {
            snowball.StartLeaving();
        }
    }
    
}