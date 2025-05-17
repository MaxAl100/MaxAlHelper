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

        private int triggerCount = 0;
        private int maxTriggers = 0;

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

        // Duplication cooldown to prevent multiple duplications in one frame
        private float duplicationCooldown = 0f;
        private const float MinDuplicationCooldown = 0.1f;

        public DuplicatingTheoCrystalZone(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, data.Height);
            AddTag(Tags.TransitionUpdate);
            Depth = Depths.FakeWalls;

            // Theo crystal settings
            canDuplicateMultipleTimes = data.Bool("canDuplicateMultipleTimes", false);
            canClonesDuplicate = data.Bool("canClonesDuplicate", false);
            maxGenerations = data.Int("maxGenerations", 1);
            timeBetweenDuplications = data.Float("timeBetweenDuplications", 1f);
            maxTriggers = data.Int("maxTriggers", 0); // 0 means infinite
            triggerCount = 0;

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

            // Update flashing effect
            if (flashing)
            {
                flash = Calc.Approach(flash, 0f, Engine.DeltaTime * 2f);
                if (flash <= 0f)
                {
                    flashing = false;
                }
            }

            // Update duplication cooldown
            if (duplicationCooldown > 0)
            {
                duplicationCooldown -= Engine.DeltaTime;
                return; // Skip duplication logic during cooldown
            }

            // Check if we've reached maximum triggers
            if (maxTriggers > 0 && triggerCount >= maxTriggers)
            {
                RemoveSelf();
                return;
            }

            // Look for theo crystals to duplicate
            Collidable = true;
            
            // Store crystals to duplicate - don't modify during enumeration
            List<DuplicatingTheoCrystal> crystalsToDuplicate = new List<DuplicatingTheoCrystal>();
            
            foreach (Entity e in Scene.Tracker.GetEntities<DuplicatingTheoCrystal>())
            {
                if (e is DuplicatingTheoCrystal theo && Collide.Check(this, theo))
                {
                    // Check if this crystal can be duplicated by the zone
                    if (theo.CanDuplicate() && (canDuplicateMultipleTimes || !theo.HasDuplicated))
                    {
                        crystalsToDuplicate.Add(theo);
                    }
                }
            }
            
            // Only duplicate one crystal per frame to prevent issues
            if (crystalsToDuplicate.Count > 0)
            {
                DuplicateTheoCrystal(crystalsToDuplicate[0]);
                
                // Set a cooldown to prevent duplicating too many crystals at once
                duplicationCooldown = MinDuplicationCooldown;
                
                // Increment trigger count
                triggerCount++;
                
                // Check if we should remove ourselves after this duplication
                if (maxTriggers > 0 && triggerCount >= maxTriggers)
                {
                    RemoveSelf();
                    return;
                }
            }

            Collidable = false;
        }

        private void DuplicateTheoCrystal(DuplicatingTheoCrystal theo)
        {
            // Don't duplicate if original would exceed max generation limit
            if (theo.CurrentGeneration >= maxGenerations)
            {
                flashing = true;
                flash = 1f;
                return;
            }

            try
            {
                // Create a clone with proper configuration
                Vector2 newPosition = theo.Position + spawnOffset;
                
                // Create the new crystal first
                DuplicatingTheoCrystal clone = new DuplicatingTheoCrystal(
                    newPosition,
                    canDuplicateMultipleTimes,
                    false,
                    canClonesDuplicate,
                    maxGenerations,
                    timeBetweenDuplications,
                    spritePaths,
                    theo.CurrentGeneration + 1 // Increment generation to avoid infinite duplication
                );

                // Add to scene 
                Scene.Add(clone);

                // Apply initial speed if specified
                if (initialSpeed != Vector2.Zero)
                {
                    clone.Speed = initialSpeed;
                }

                // Bounce original crystal if requested
                if (bounceBack)
                {
                    Vector2 currentSpeed = theo.Speed;
                    if (currentSpeed != Vector2.Zero)
                    {
                        theo.Speed = -currentSpeed;
                    }
                    else
                    {
                        // If no speed, bounce in a sensible direction based on zone
                        Vector2 direction = theo.Position - (Position + Collider.Center);
                        if (direction != Vector2.Zero)
                        {
                            direction.Normalize();
                            theo.Speed = direction * 100f; // Give a small bounce
                        }
                    }
                }

                // Record that the original crystal has duplicated
                // This increments its generation and resets the duplication timer
                theo.RecordDuplication();

                // Remove original if requested - do this last to avoid null reference issues
                if (removeOriginal)
                {
                    // Schedule for removal to avoid issues during iteration
                    Engine.Scene.OnEndOfFrame += () => {
                        theo.RemoveSelf();
                    };
                }

                // Flash effect
                flashing = true;
                flash = 1f;
            }
            catch (Exception ex)
            {
                // Log the error but don't crash
                Logger.Log(LogLevel.Error, "MaxAlHelper", $"Error during duplication: {ex.Message}");
                
                // Flash to indicate failure
                flashing = true;
                flash = 1f;
            }
        }

        public override void Render()
        {
            Draw.Rect(Collider, baseColor * 0.25f);
            Draw.HollowRect(Collider, Color.Gray);

            if (flashing)
            {
                Draw.Rect(Collider, Color.Lerp(Color.White, baseColor, flash) * 0.5f);
            }
        }

        private void OnPlayer(Player player)
        {
            // Visual feedback when player enters
            flashing = true;
            flash = 1f;
        }
    }
}