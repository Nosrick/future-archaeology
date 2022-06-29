using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class BrushTool : AbstractTool
    {
        public override string TranslationKey => "tools.brush.name";

        public const int DEFAULT_COST = 5;
        public const int DEFAULT_COOLDOWN = 0;

        public BrushTool()
        {
            this.Cost = DEFAULT_COST;
            this.UsageCooldown = DEFAULT_COOLDOWN;
            
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/brush-3.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }
        
        public override AABB Execute(Vector3Int hit, Vector3Int previous)
        {
            GlobalConstants.GameManager.DiggingSpace.DamageCell(hit, 1);

            return new AABB
            {
                Position = hit.ToVector3(),
                Size = Vector3.One
            };
        }
    }
}