using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    [CustomEntity("MaxAlHelper/DuplicatingTheoCrystalZone")]
    public class DuplicatingTheoCrystalZone : Entity
    {
        private static Color baseColor = Color.LightGray;
        private float flash;
        private bool flashing;

        // Track crystals that have been processed to prevent duplicate processing
        private HashSet<DuplicatingTheoCrystal> processedCrystals = new HashSet<DuplicatingTheoCrystal>();

        // Theo crystal creation settings
        private bool canDuplicateMultipleTimes;
        private bool canClonesDuplicate;
        private int maxGenerations;
        private float timeBetweenDuplications;
        private string[] spritePaths;

        // Positioning and velocity settings
        private Vector2 spawnOffset;
        private Vector2 initialSpeed;
        private bool bounceBack;
        private bool removeOriginal;

        public DuplicatingTheoCrystalZone(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, data.Height);
            AddTag(Tags.TransitionUpdate); // Ensures entity updates during transitions
            Depth = Depths.FakeWalls; // Makes it render in the right order

            // Theo crystal settings
            canDuplicateMultipleTimes = data.Bool("canDuplicateMultipleTimes", false);
            canClonesDuplicate = data.Bool("canClonesDuplicate", false);
            maxGenerations = data.Int("maxGenerations", 1);
            timeBetweenDuplications = data.Float("timeBetweenDuplications", 1f);

            // Parse sprite paths
            string spritesString = data.Attr("spritePaths", "");
            spritePaths = string.IsNullOrEmpty(spritesString) ? new string[] { } : spritesString.Split(',');

            // Positioning and velocity settings
            spawnOffset = new Vector2(
                data.Float("offsetX", 0f),
                data.Float("offsetY", 0f)
            );

            initialSpeed = new Vector2(
                data.Float("speedX", 0f),
                data.Float("speedY", 0f)
            );

            bounceBack = data.Bool("bounceBack", false);
            removeOriginal = data.Bool("removeOriginal", false);

            Add(new PlayerCollider(OnPlayer));
        }

        public override void Update()
        {
            base.Update();

            if (flashing)
            {
                flash = Calc.Approach(flash, 0f, Engine.DeltaTime * 2f);
                if (flash <= 0f)
                {
                    flashing = false;
                }
            }

            // Make collisions work with theo crystals
            Collidable = true;
            List<Entity> colliding = CollideAll<DuplicatingTheoCrystal>();
            foreach (Entity e in colliding)
            {
                if (e is DuplicatingTheoCrystal theo && !processedCrystals.Contains(theo))
                {
                    // Only process theo crystals that can be duplicated
                    if (theo.CanDuplicate())
                    {
                        DuplicateTheoCrystal(theo);

                        // Mark this crystal as processed to prevent duplicate processing
                        // Only add to processed list if we're not allowing multiple duplications
                        if (!canDuplicateMultipleTimes)
                        {
                            processedCrystals.Add(theo);
                        }
                    }
                }
            }
            Collidable = false;
        }

        private void DuplicateTheoCrystal(DuplicatingTheoCrystal theo)
        {
            // Update the original theo's duplication state
            theo.RecordDuplication();

            // Create a new duplicating theo crystal only if we haven't exceeded max generations
            if (theo.CurrentGeneration >= maxGenerations)
            {
                flashing = true;
                flash = 1f;
                return;
            }

            // Create the clone with the next generation number
            DuplicatingTheoCrystal clone = new DuplicatingTheoCrystal(
                canDuplicateMultipleTimes: canDuplicateMultipleTimes,
                hasDuplicated: false,
                canClonesDuplicate: canClonesDuplicate,
                maxGenerations: maxGenerations,
                timeBetweenDuplications: timeBetweenDuplications,
                spritePaths: spritePaths,
                position: theo.Position + spawnOffset
            )
            {
                CurrentGeneration = 0
            };

            // Remove existing sprite and create new one
            Sprite baseSprite = clone.Get<Sprite>();
            if (baseSprite != null)
            {
                baseSprite.RemoveSelf();
            }
            clone.CreateSprite();

            // Add to scene first so physics work properly
            Scene.Add(clone);

            // Apply initial velocity
            if (initialSpeed != Vector2.Zero)
            {
                clone.Speed = initialSpeed;
            }

            // Handle original theo modifications
            if (bounceBack)
            {

                 theo.Speed = -theo.Speed;

            }

            if (removeOriginal)
            {
                theo.RemoveSelf();
                processedCrystals.Remove(theo);
            }

            // Visual feedback
            flashing = true;
            flash = 1f;
        }

        public override void Render()
        {
            // Simple visualization for the zone
            Draw.Rect(Collider, baseColor * 0.25f);
            Draw.HollowRect(Collider, Color.Gray);

            if (flashing)
            {
                Draw.Rect(Collider, Color.Lerp(Color.White, baseColor, flash) * 0.5f);
            }
        }

        private void OnPlayer(Player player)
        {
            // Optional: make the zone flash on contact
            flashing = true;
            flash = 1f;
        }
    }
}