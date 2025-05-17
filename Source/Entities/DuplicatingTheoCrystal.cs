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
        public bool CanDuplicateMultipleTimes { get; set; } = false;
        public bool HasDuplicated { get; set; } = false;
        public bool CanClonesDuplicate { get; set; } = false;
        public int MaxGenerations { get; set; } = 1;
        public float TimeBetweenDuplications { get; set; } = 1f;
        public float DuplicationTimer { get; set; } = 0f; // Start at 0 so first duplication can happen immediately
        public int CurrentGeneration { get; set; } = 0;
        public string[] SpritePaths { get; set; } = { };
        public Sprite Sprite;

        // Track the last time this crystal was duplicated to prevent rapid duplications
        private float lastDuplicationTime = 0f;
        private const float MinTimeBetweenDuplications = 0.5f; // Safety minimum

        // Property to access the Speed of the TheoCrystal directly
        public Vector2 Speed
        {
            get
            {
                // Try to access speed - fallback to zero if we can't
                try
                {
                    if (_isSpeedPropertyInitialized)
                    {
                        return _theoCrystalSpeed;
                    }
                }
                catch
                {
                    // Ignore failures at this point
                }
                return Vector2.Zero;
            }
            set
            {
                // Try to set the speed - this allows the zone to control crystal movement
                try
                {
                    _theoCrystalSpeed = value;
                    _isSpeedPropertyInitialized = true;
                }
                catch
                {
                    // Fall back to direct position update if needed
                    Position += value * Engine.DeltaTime;
                }
            }
        }

        // Internal tracking for speed property
        private Vector2 _theoCrystalSpeed = Vector2.Zero;
        private bool _isSpeedPropertyInitialized = false;

        // Constructor for loading from EntityData (used by maps)
        public DuplicatingTheoCrystal(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            CanDuplicateMultipleTimes = data.Bool("canDuplicateMultipleTimes", false);
            HasDuplicated = data.Bool("hasDuplicated", false);
            CanClonesDuplicate = data.Bool("canClonesDuplicate", false);
            MaxGenerations = data.Int("maxGenerations", 1);
            TimeBetweenDuplications = Math.Max(MinTimeBetweenDuplications, data.Float("timeBetweenDuplications", 1f));
            DuplicationTimer = 0f; // Start ready to duplicate
            CurrentGeneration = data.Int("currentGeneration", 0);

            string spritesString = data.Attr("spritePaths", "");
            SpritePaths = string.IsNullOrEmpty(spritesString) ? new string[] { } : spritesString.Split(',');

            Sprite baseSprite = Get<Sprite>();
            if (baseSprite != null)
            {
                baseSprite.RemoveSelf();
            }

            CreateSprite();
        }

        // Runtime duplication constructor
        public DuplicatingTheoCrystal(
            bool canDuplicateMultipleTimes = false,
            bool hasDuplicated = false,
            bool canClonesDuplicate = false,
            int maxGenerations = 1,
            float timeBetweenDuplications = 1f,
            string[] spritePaths = null,
            Vector2? position = null
        ) : base(position ?? Vector2.Zero)
        {
            CanDuplicateMultipleTimes = canDuplicateMultipleTimes;
            HasDuplicated = hasDuplicated;
            CanClonesDuplicate = canClonesDuplicate;
            MaxGenerations = maxGenerations;
            TimeBetweenDuplications = Math.Max(MinTimeBetweenDuplications, timeBetweenDuplications);
            DuplicationTimer = TimeBetweenDuplications; // Start with timer at max - need to wait before duplicating
            SpritePaths = spritePaths ?? new string[] { };

            Sprite baseSprite = Get<Sprite>();
            if (baseSprite != null)
            {
                baseSprite.RemoveSelf();
            }

            CreateSprite();
        }

        public void CreateSprite()
        {
            if (SpritePaths != null && SpritePaths.Length > 0)
            {
                Sprite?.RemoveSelf();

                // Always select a sprite based on the current generation
                int spriteIndex = Math.Min(CurrentGeneration, SpritePaths.Length - 1);
                if (spriteIndex < 0 || spriteIndex >= SpritePaths.Length)
                {
                    // Safety check to prevent out of bounds
                    spriteIndex = 0;
                }

                string spritePath = SpritePaths[spriteIndex];

                if (!string.IsNullOrEmpty(spritePath))
                {
                    Sprite = GFX.SpriteBank.Create(spritePath.Trim());
                    Add(Sprite);
                }
                else
                {
                    // Fallback to default theo sprite if the path is empty
                    Sprite = GFX.SpriteBank.Create("theo_crystal");
                    Add(Sprite);
                }
            }
            else
            {
                // Fallback to default theo sprite if no paths are provided
                Sprite = GFX.SpriteBank.Create("theo_crystal");
                Add(Sprite);
            }
        }

        public override void Update()
        {
            // Update duplication timer
            if (DuplicationTimer > 0)
            {
                DuplicationTimer -= Engine.DeltaTime;
            }

            // Handle movement using base class
            base.Update();
        }

        // This method checks if the crystal can be duplicated by a DuplicatingTheoCrystalZone
        public bool CanDuplicate()
        {
            // Check all conditions that would prevent duplication
            if (DuplicationTimer > 0)
                return false;

            // Check if we've reached the max generation
            if (CurrentGeneration >= MaxGenerations)
                return false;

            // Check if we've already duplicated and can't do it multiple times
            if (HasDuplicated && !CanDuplicateMultipleTimes)
                return false;

            // Check if we're being duplicated too rapidly
            float currentTime = (float)Scene.TimeActive;
            if (currentTime - lastDuplicationTime < MinTimeBetweenDuplications)
                return false;

            return true;
        }

        // Record that this crystal has been duplicated
        public void RecordDuplication()
        {
            // Reset timer and track last duplication time
            DuplicationTimer = TimeBetweenDuplications;
            lastDuplicationTime = (float)Scene.TimeActive;
            HasDuplicated = true;
            CurrentGeneration += 1;
            CreateSprite();
        }
    }
}