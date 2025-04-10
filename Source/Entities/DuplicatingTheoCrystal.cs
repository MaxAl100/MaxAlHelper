using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

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
        public float DuplicationTimer { get; set; } = 5f;
        public int CurrentGeneration { get; set; } = 0;
        public string[] SpritePaths { get; set; } = {  };
        public Sprite Sprite;

        // Constructor for loading from EntityData (used by maps)
        public DuplicatingTheoCrystal(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            CanDuplicateMultipleTimes = data.Bool("canDuplicateMultipleTimes", false);
            HasDuplicated = data.Bool("hasDuplicated", false);
            CanClonesDuplicate = data.Bool("canClonesDuplicate", false);
            MaxGenerations = data.Int("maxGenerations", 1);
            TimeBetweenDuplications = data.Float("timeBetweenDuplications", 1f);
            DuplicationTimer = TimeBetweenDuplications;
            CurrentGeneration = data.Int("currentGeneration", 0);
            SpritePaths = data.Attr("spritePaths").Split(',');

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
            TimeBetweenDuplications = timeBetweenDuplications;
            DuplicationTimer = timeBetweenDuplications;
            SpritePaths = spritePaths ?? new string[] {  };
        }

        private void CreateSprite()
        {
            if (SpritePaths.Length > 0)
            {
                Sprite?.RemoveSelf();
                Sprite = GFX.SpriteBank.Create(SpritePaths[CurrentGeneration % SpritePaths.Length]);
                Add(Sprite);
            }
        }

        public override void Update()
        {
            if (DuplicationTimer > 0)
            {
                DuplicationTimer -= Engine.DeltaTime;
            }
            base.Update();
        }

        public void DuplicateTheoCrystal()
        {
            DuplicationTimer = TimeBetweenDuplications;
            DuplicatingTheoCrystal clone = new DuplicatingTheoCrystal(
                canDuplicateMultipleTimes: CanDuplicateMultipleTimes,
                hasDuplicated: false,
                canClonesDuplicate: CanClonesDuplicate,
                maxGenerations: MaxGenerations,
                timeBetweenDuplications: TimeBetweenDuplications,
                spritePaths: SpritePaths,
                position: Position 
            );
            clone.CurrentGeneration = CurrentGeneration + 1;
            CurrentGeneration += 1;

            Sprite baseSprite = clone.Get<Sprite>();
            if (baseSprite != null)
            {
                baseSprite.RemoveSelf();
            }
            clone.CreateSprite();

            if (!CanClonesDuplicate)
            {
                clone.CanDuplicateMultipleTimes = false;
                HasDuplicated = true;
            }

            Scene.Add(clone); 

            HasDuplicated = true;
        }

        public bool CanDuplicate()
        {
            if (DuplicationTimer > 0) return false;
            else if (CurrentGeneration >= MaxGenerations) return false;
            else if (HasDuplicated && !CanDuplicateMultipleTimes) return false;
            else return true;
        }
    }
}
