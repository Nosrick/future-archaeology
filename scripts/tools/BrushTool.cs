using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class BrushTool : ITool
    {
        public int Cost => 5;
        public string Name => "Brush";
        public AudioStreamRandomPitch AssociatedSound { get; protected set; }

        public BrushTool()
        {
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/brush-1.wav");
            this.AssociatedSound.RandomPitch = 1.2f;
        }
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            GlobalConstants.GameManager.DiggingSpace.DamageCell(hit, 1);

            return this.Cost;
        }
    }
}