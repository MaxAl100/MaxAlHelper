using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    [Tracked]
    [CustomEntity("MaxAlHelper/DuplicatingTheoCrystal")]
    public class DuplicatingTheoCrystal : TheoCrystal
    {
        public bool CanDuplicateMultipleTimes { get; set; }
        public bool HasDuplicated { get; set; }
        public bool CanClonesDuplicate { get; set; }
        public int MaxGenerations { get; set; } = 1;
        public float TimeBetweenDuplications { get; set; } = 1f;
        public float DuplicationTimer { get; set; }
        public int CurrentGeneration { get; set; }
        public string[] SpritePaths { get; set; }
        public Sprite CustomSprite;

        private float lastDuplicationTime = 0f;
        private const float MinTimeBetweenDuplications = 0.5f;
        private Vector2 _initialSpeed = Vector2.Zero;
        private bool _hasAppliedInitialSpeed = false;

        // Modified Speed property that won't override normal behavior
        public Vector2 InitialSpeed
        {
            get => _initialSpeed;
            set => _initialSpeed = value;
        }

        public DuplicatingTheoCrystal(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Initialize(
                data.Bool("canDuplicateMultipleTimes", false),
                data.Bool("hasDuplicated", false),
                data.Bool("canClonesDuplicate", false),
                data.Int("maxGenerations", 1),
                Math.Max(MinTimeBetweenDuplications, data.Float("timeBetweenDuplications", 1f)),
                data.Int("currentGeneration", 0),
                data.Attr("spritePaths", "")
            );
        }

        public DuplicatingTheoCrystal(
            Vector2 position,
            bool canDuplicateMultipleTimes = false,
            bool hasDuplicated = false,
            bool canClonesDuplicate = false,
            int maxGenerations = 1,
            float timeBetweenDuplications = 1f,
            string[] spritePaths = null,
            int currentGeneration = 0
        ) : base(position)
        {
            Initialize(
                canDuplicateMultipleTimes,
                hasDuplicated,
                canClonesDuplicate,
                maxGenerations,
                Math.Max(MinTimeBetweenDuplications, timeBetweenDuplications),
                currentGeneration,
                null,
                spritePaths
            );
        }

        private void Initialize(
            bool canDuplicateMultipleTimes,
            bool hasDuplicated,
            bool canClonesDuplicate,
            int maxGenerations,
            float timeBetweenDuplications,
            int currentGeneration,
            string spritesString = null,
            string[] spritePaths = null)
        {
            CanDuplicateMultipleTimes = canDuplicateMultipleTimes;
            HasDuplicated = hasDuplicated;
            CanClonesDuplicate = canClonesDuplicate;
            MaxGenerations = maxGenerations;
            TimeBetweenDuplications = timeBetweenDuplications;
            DuplicationTimer = TimeBetweenDuplications; // Start with timer at full to prevent instant duplication
            CurrentGeneration = currentGeneration;

            // Handle sprite paths
            if (spritePaths != null)
            {
                SpritePaths = spritePaths;
            }
            else if (!string.IsNullOrEmpty(spritesString))
            {
                SpritePaths = spritesString.Split(',');
            }
            else
            {
                SpritePaths = new string[] { };
            }

            // Don't remove the original sprite immediately, wait until we create our custom one
            CreateSprite();
        }

        public void CreateSprite()
        {
            // Safely remove existing custom sprite if it exists
            if (CustomSprite != null)
            {
                Remove(CustomSprite);
                CustomSprite = null;
            }

            // Use default sprite if no custom sprites specified
            if (SpritePaths == null || SpritePaths.Length == 0 || string.IsNullOrEmpty(SpritePaths[0]))
            {
                // Keep the original sprite if we don't have a custom one
                return;
            }

            // Choose sprite based on index (limited by array bounds)
            int spriteIndex = 0;
            if (SpritePaths.Length > 1)
            {
                // Use generation to select sprite if multiple are available
                spriteIndex = Math.Min(CurrentGeneration, SpritePaths.Length - 1);
            }

            string spritePath = SpritePaths[spriteIndex].Trim();
            if (string.IsNullOrEmpty(spritePath))
            {
                return; // Keep original sprite
            }

            try
            {
                // Remove the original sprite
                Sprite baseSprite = Get<Sprite>();
                if (baseSprite != null)
                {
                    Remove(baseSprite);
                }

                // Create new sprite with specified path
                CustomSprite = GFX.SpriteBank.Create(spritePath);
                if (CustomSprite != null)
                {
                    Add(CustomSprite);
                }
                else
                {
                    // If sprite creation failed, log and revert to default
                    Logger.Log(LogLevel.Warn, "MaxAlHelper", $"Failed to create sprite: {spritePath}");
                    CustomSprite = GFX.SpriteBank.Create("theo_crystal");
                    Add(CustomSprite);
                }
            }
            catch (Exception e)
            {
                // Handle any exceptions during sprite creation
                Logger.Log(LogLevel.Error, "MaxAlHelper", $"Error creating sprite: {e.Message}");
                
                // Fall back to default sprite
                CustomSprite = GFX.SpriteBank.Create("theo_crystal");
                Add(CustomSprite);
            }
        }

        public override void Update()
        {
            // Update duplication timer
            if (DuplicationTimer > 0)
            {
                DuplicationTimer -= Engine.DeltaTime;
            }

            // Apply initial speed only once if set
            if (!_hasAppliedInitialSpeed && _initialSpeed != Vector2.Zero)
            {
                // Apply initial velocity by directly setting the base class's Speed fields
                // This will integrate with the normal Theo Crystal physics
                Speed = _initialSpeed;
                _hasAppliedInitialSpeed = true;
            }

            // Call base update to handle normal TheocrystalZone physics
            base.Update();
        }

        public bool CanDuplicate()
        {
            // Clones can't duplicate if CanClonesDuplicate is false
            if (!CanClonesDuplicate && CurrentGeneration > 0)
                return false;

            // Can't duplicate if timer is running
            if (DuplicationTimer > 0)
                return false;

            // Can't duplicate if we're at max generations
            if (CurrentGeneration >= MaxGenerations)
                return false;

            // Can't duplicate if we've already duplicated and multi-duplication is off
            if (HasDuplicated && !CanDuplicateMultipleTimes)
                return false;

            // Make sure we're not duplicating too quickly
            float currentTime = (float)Scene.TimeActive;
            if (currentTime - lastDuplicationTime < MinTimeBetweenDuplications)
                return false;

            return true;
        }

        public void RecordDuplication()
        {
            // Reset duplication timer
            DuplicationTimer = TimeBetweenDuplications;
            lastDuplicationTime = (float)Scene.TimeActive;
            HasDuplicated = true;

            // Increment generation of the duplicator
            CurrentGeneration++;

            // Update sprite for new generation
            CreateSprite();
        }
    }
}