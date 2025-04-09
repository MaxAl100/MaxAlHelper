using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaxAlHelper.Entities
{
    [Tracked]
    public class DuplicatingTheoCrystalTrigger : Trigger
    {
        // Configurable attributes for the trigger
        public int DuplicatesToCreate { get; set; }
        public bool KeepOriginalStats { get; set; }
        public float DuplicationSpeed { get; set; }

        public DuplicatingTheoCrystalTrigger(Vector2 position, EntityData data) : base(position)
        {
            // Set default values or get them from Lönn
            DuplicatesToCreate = data.Int("duplicatesToCreate", 5); // How many TheoCrystals to duplicate
            KeepOriginalStats = data.Bool("keepOriginalStats", true); // Whether to keep original stats
            DuplicationSpeed = data.Float("duplicationSpeed", 1f); // Speed of duplication
        }

        public override void OnPlayer(Player player)
        {
            base.OnPlayer(player);

            // Iterate through TheoCrystals in the scene
            foreach (var theo in Scene.Tracker.GetEntities<DuplicatingTheo>())
            {
                if (theo is DuplicatingTheo duplicatingTheo && duplicatingTheo.CanDuplicateMultipleTimes)
                {
                    // Create new TheoCrystals based on the trigger's settings
                    for (int i = 0; i < DuplicatesToCreate; i++)
                    {
                        var newTheo = new DuplicatingTheo(theo.Position + new Vector2(20, 20));
                        newTheo.CanCloneDuplicate = duplicatingTheo.CanCloneDuplicate;
                        newTheo.MaxGenerations = duplicatingTheo.MaxGenerations;
                        newTheo.TimeBetweenDuplications = duplicatingTheo.TimeBetweenDuplications;
                        newTheo.Sprite = duplicatingTheo.Sprite;
                        Scene.Add(newTheo);
                    }
                }
            }
        }
    }
}
