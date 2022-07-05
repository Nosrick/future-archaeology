using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class ChiselTool : AbstractTool
    {
        public const int DEFAULT_COST = 10;
        public const int DEFAULT_COOLDOWN = 0;
        public override string TranslationKey => "tools.chisel.name";

        public ChiselTool()
        {
            this.Cost = DEFAULT_COST;
            this.UsageCooldown = DEFAULT_COOLDOWN;
            
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/dirt-crunch-5.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }
        
        public override AABB Execute(Vector3Int hit, Vector3Int previous)
        {
            GlobalConstants.GameManager.DiggingSpace.DamageCell(hit, 3);

            this.TimesUsed++;
            return new AABB
            {
                Position = hit.ToVector3(),
                Size = Vector3.One
            };
        }
    }
}