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
        private static Color baseColor = Color.LightBlue * 0.5f;
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

        // Positioning and velocity settings for clones
        private Vector2 spawnOffset;
        private Vector2 initialSpeed;
        private bool removeOriginal;

        // Original theo crystal modification settings
        private Vector2 originalOffset;
        private Vector2 originalSpeed;
        private bool bounceBack;
        private bool bounceBackUseDirection; // If true, bounce in direction away from zone center

        // Duplication cooldown to prevent multiple duplications in one frame
        private float duplicationCooldown = 0f;
        private const float MinDuplicationCooldown = 0.05f;

        private List<ParticleData> particles = new();
        private Color particleColor = Color.White * 0.5f;
        private static MTexture myParticle = GFX.Game["MaxAlHelper/particles/DuplicatingTheoCrystalParticle"];
        
        private struct ParticleData
        {
            public Vector2 Position;
            public Vector2 Velocity;
            
            public ParticleData(Vector2 position, Vector2 velocity)
            {
                Position = position;
                Velocity = velocity;
            }
        }

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

            // Clone positioning and velocity settings
            spawnOffset = new Vector2(
                data.Float("offsetX", 0f),
                data.Float("offsetY", 0f)
            );

            initialSpeed = new Vector2(
                data.Float("speedX", 0f),
                data.Float("speedY", 0f)
            );

            removeOriginal = data.Bool("removeOriginal", false);

            // Original theo crystal modification settings
            originalOffset = new Vector2(
                data.Float("originalOffsetX", 0f),
                data.Float("originalOffsetY", 0f)
            );

            originalSpeed = new Vector2(
                data.Float("originalSpeedX", 0f),
                data.Float("originalSpeedY", 0f)
            );

            bounceBack = data.Bool("bounceBack", false);
            bounceBackUseDirection = data.Bool("bounceBackUseDirection", true);

            Add(new PlayerCollider(OnPlayer));

            // Initialize particles with random positions and velocities
            InitializeParticles();
        }

        public override void Update()
        {
            base.Update();

            // Update particles
            UpdateParticles();

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

            // Using both optimization tricks:
            // 1. Direct type casting in foreach
            // 2. Using CollideAll instead of manual collision checking
            foreach (DuplicatingTheoCrystal theo in this.CollideAll<DuplicatingTheoCrystal>())
            {
                // Check if this crystal can be duplicated by the zone
                if (theo.CanDuplicate())
                {
                    crystalsToDuplicate.Add(theo);
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
        }

        private void InitializeParticles()
        {
            int numParticles = (int)(Width * Height / 64f); // Adjust density as needed

            for (int i = 0; i < numParticles; i++)
            {
                Vector2 position = new Vector2(
                    Calc.Random.Range(0f, Width),
                    Calc.Random.Range(0f, Height)
                );

                Vector2 velocity = new Vector2(
                    Calc.Random.Range(10f, 30f), // Random horizontal speed (rightward)
                    Calc.Random.Range(10f, 30f)  // Random vertical speed (downward)
                );

                particles.Add(new ParticleData(position, velocity));
            }
        }

        private void UpdateParticles()
        {
            // Update existing particles
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                ParticleData particle = particles[i];
                particle.Position += particle.Velocity * Engine.DeltaTime;
                
                // Remove particles when their bottom-right corner has left the zone
                // Assuming particle sprite is about 5x5 pixels
                float particleSize = 5f;
                if (particle.Position.X + particleSize > Width || particle.Position.Y + particleSize > Height)
                {
                    particles.RemoveAt(i);
                }
                else
                {
                    particles[i] = particle;
                }
            }
            
            // Add new particles along the top and left edges to maintain density
            int targetParticleCount = (int)(Width * Height / 64f);
            int particlesToAdd = Math.Max(0, targetParticleCount - particles.Count);
            
            // Limit how many particles we try to add per frame to prevent performance issues
            int maxParticlesPerFrame = 5;
            particlesToAdd = Math.Min(particlesToAdd, maxParticlesPerFrame);
            
            for (int i = 0; i < particlesToAdd; i++)
            {
                Vector2 position;
                
                // Choose spawn edge based on relative sizes (longer edge has higher chance)
                float topEdgeWeight = Width;
                float leftEdgeWeight = Height;
                float totalWeight = topEdgeWeight + leftEdgeWeight;
                bool spawnFromTop = Calc.Random.NextFloat() < (topEdgeWeight / totalWeight);
                
                if (spawnFromTop)
                {
                    // Spawn from top edge
                    position = new Vector2(
                        Calc.Random.Range(0f, Width), // Along entire top width
                        Calc.Random.Range(0f, 0f)    // Just above the zone (closer to edge)
                    );
                }
                else
                {
                    // Spawn from left edge
                    position = new Vector2(
                        Calc.Random.Range(0f, 0f),   // Just left of the zone (closer to edge)
                        Calc.Random.Range(0f, Height) // Along entire left height
                    );
                }
                
                Vector2 velocity = new Vector2(
                    Calc.Random.Range(10f, 30f), // Random horizontal speed (rightward)
                    Calc.Random.Range(10f, 30f)  // Random vertical speed (downward)
                );
                
                particles.Add(new ParticleData(position, velocity));
            }
        }

        private void DuplicateTheoCrystal(DuplicatingTheoCrystal theo)
        {
            // Don't duplicate if original would exceed max generation limit
            if (maxGenerations != 0 && theo.CurrentGeneration >= maxGenerations)
            {
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

                if (!theo.CanClonesDuplicate)
                {
                    clone.HasDuplicated = true; // Prevent clones from duplicating
                    clone.CanDuplicateMultipleTimes = false; // Prevent clones from duplicating
                }

                // Add to scene 
                Scene.Add(clone);

                // Apply initial speed to clone if specified
                if (initialSpeed != Vector2.Zero)
                {
                    clone.Speed = initialSpeed;
                }

                // Now modify the original theo crystal
                ModifyOriginalTheoCrystal(theo);

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
                flash = 0.3f;
            }
            catch (Exception ex)
            {
                // Log the error but don't crash
                Logger.Log(LogLevel.Error, "MaxAlHelper", $"Error during duplication: {ex.Message}");
                
                // Flash to indicate failure
                flashing = true;
                flash = 0.03f;
            }
        }

        private void ModifyOriginalTheoCrystal(DuplicatingTheoCrystal theo)
        {
            // Apply position offset to original
            if (originalOffset != Vector2.Zero)
            {
                theo.Position += originalOffset;
            }

            // Apply speed to original
            if (originalSpeed != Vector2.Zero)
            {
                theo.Speed = originalSpeed;
            }
            else if (bounceBack)
            {
                // Bounce back logic
                if (bounceBackUseDirection)
                {
                    // Bounce in direction away from zone center
                    Vector2 zoneCenter = Position + new Vector2(Width * 0.5f, Height * 0.5f);
                    Vector2 direction = theo.Position - zoneCenter;
                    
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        // Use existing speed magnitude if available, otherwise use a default
                        float speedMagnitude = theo.Speed.Length();
                        if (speedMagnitude < 10f) // If speed is too low, use a minimum
                        {
                            speedMagnitude = 100f;
                        }
                        theo.Speed = direction * speedMagnitude;
                    }
                    else
                    {
                        // Fallback: bounce upward if crystal is exactly at zone center
                        theo.Speed = new Vector2(0, -100f);
                    }
                }
                else
                {
                    // Simple bounce back: reverse current velocity
                    Vector2 currentSpeed = theo.Speed;
                    if (currentSpeed != Vector2.Zero)
                    {
                        theo.Speed = -currentSpeed;
                    }
                    else
                    {
                        // If no speed, give it a default upward bounce
                        theo.Speed = new Vector2(0, -100f);
                    }
                }
            }
        }

        public override void Render()
        {
            Draw.Rect(Collider, baseColor * 0.25f);
            Draw.HollowRect(Collider, Color.LightBlue);
            foreach (ParticleData p in particles) {
                myParticle.Draw(Position + p.Position, Vector2.One * 0.3f, particleColor);
            }

            if (flashing)
            {
                Draw.Rect(Collider, Color.Lerp(Color.White, baseColor, flash) * 0.3f);
            }
        }

        private void OnPlayer(Player player)
        {
            // Visual feedback when player enters
            flashing = true;
            flash = 0.02f;
        }
    }
}