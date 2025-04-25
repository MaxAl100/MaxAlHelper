using System;
using System.Collections;
using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    [CustomEntity("MaxAlHelper/AbsorbCutsceneStarter")]
    internal class AbsorbCutsceneStarter : Entity
    {
        private TalkComponent talk;
        public string targetRoom;
        public int walkAroundDistance;
        public string introType;
        public Vector2 targetAbsorbPosition;
        public float animationSpeedMult;
        public bool walkBelowTeleport;
        public Vector2 targetRoomTransition;
        public bool zoomIn;

        public AbsorbCutsceneStarter(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Depth = 2000;

            // Read values from EntityData (defined in Lönn or Ahorn)
            this.targetRoom = data.Attr("targetRoom", "a-room-name"); // default if not defined
            this.walkAroundDistance = data.Int("walkAroundDistance", 8);
            this.introType = data.Attr("introType", "None");
            this.targetAbsorbPosition = new Vector2(
                data.Float("targetAbsorbPositionX", 0f),
                data.Float("targetAbsorbPositionY", 0f)
            );
            this.targetRoomTransition = new Vector2(
                data.Float("targetRoomTransitionX", 0f),
                data.Float("targetRoomTransitionY", 0f)
                );
            this.animationSpeedMult = data.Float("animationSpeedMult", 1f);
            this.walkBelowTeleport = data.Bool("walkBelowTeleport", true);
            this.zoomIn = data.Bool("zoomIn", true);

            Add(talk = new TalkComponent(
                new Rectangle(-24, -8, 48, 40),
                new Vector2(-0.5f, -20f),
                Interact
            ));
            talk.PlayerMustBeFacing = false;
        }

        public void Interact(Player player)
        {
            Scene.Add(new AbsorbCutscene(player, this));
        }
    }
}