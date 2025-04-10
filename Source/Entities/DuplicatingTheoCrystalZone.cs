using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    [CustomEntity("MaxAlHelper/DuplicatingTheoCrystalZone")]
    public class DuplicatingTheoCrystalZone : Entity
    {
        private static Color baseColor = Color.LightGray;
        private float flash;
        private bool flashing;

        public DuplicatingTheoCrystalZone(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, data.Height);
            AddTag(Tags.TransitionUpdate); // Ensures entity updates during transitions
            Depth = Depths.FakeWalls; // Makes it render in the right order
            Add(new PlayerCollider(OnPlayer));
        }

        public override void Update()
        {
            base.Update();
            Collidable = true;

            List<Entity> colliding = CollideAll<DuplicatingTheoCrystal>();
            foreach (Entity e in colliding)
            {
                if (e is DuplicatingTheoCrystal theo && theo.CanDuplicate())
                {
                    theo.DuplicateTheoCrystal();
                }
            }

            Collidable = false;
        }

        public override void Render()
        {
            // Simple particle look for clarity
            Draw.Rect(Collider, baseColor * 0.25f);
            Draw.HollowRect(Collider, Color.Gray);

            if (flashing)
            {
                Draw.Rect(Collider, Color.Lerp(Color.White, baseColor, flash) * 0.5f);
            }
        }

        private void OnPlayer(Player player)
        {
            // Optional: make the zone flash on contact or some other effect
            flashing = true;
            flash = 1f;
        }
    }
}
