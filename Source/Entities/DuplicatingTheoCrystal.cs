using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    [Tracked]
    public class DuplicatingTheoCrystal : TheoCrystal
    {
        // Extra attributes for duplication
        public bool CanDuplicateMultipleTimes { get; set; }
        public bool HasDuplicated { get; private set; }
        public bool CanCloneDuplicate { get; set; }
        public int MaxGenerations { get; set; }
        public int CurrentGeneration { get; private set; }
        public float TimeBetweenDuplications { get; set; }
        private float duplicationTimer;
        public Sprite[] GenerationSprites { get; set; }

        public DuplicatingTheoCrystal(Vector2 position) : base(position)
        {
            // Set defaults for the new attributes
            CanDuplicateMultipleTimes = true;
            HasDuplicated = false;
            CanCloneDuplicate = true;
            MaxGenerations = 3; // Example max generations
            TimeBetweenDuplications = 5f; // Time between duplications in seconds
            CurrentGeneration = 1;
            duplicationTimer = TimeBetweenDuplications;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            ResetDuplicationTimer();
        }

        public override void Update()
        {
            base.Update();

            // Manage duplication timer
            if (HasDuplicated || duplicationTimer <= 0)
                return;

            duplicationTimer -= Engine.DeltaTime;

            if (duplicationTimer <= 0)
            {
                TryDuplicate();
            }
        }

        private void TryDuplicate()
        {
            // Check if duplication is possible based on current generation and clone properties
            if (CanDuplicateMultipleTimes && CurrentGeneration <= MaxGenerations)
            {
                // Logic to create a duplicate TheoCrystal
                DuplicatingTheoCrystal clone = new DuplicatingTheoCrystal(Position + new Vector2(20, 20)); // Adjust position
                clone.CurrentGeneration = CurrentGeneration + 1;
                clone.Sprite = GenerationSprites[CurrentGeneration % GenerationSprites.Length];
                // Add clone to the scene and initialize stats
                Scene.Add(clone);
                HasDuplicated = true;
                duplicationTimer = TimeBetweenDuplications;
            }
        }

        private void ResetDuplicationTimer()
        {
            duplicationTimer = TimeBetweenDuplications;
        }
    }
}
