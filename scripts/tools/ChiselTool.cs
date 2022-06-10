using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class ChiselTool : ITool
    {
        public int Cost => 10;
        public AudioStreamRandomPitch AssociatedSound { get; protected set; }
        public string Name => "Chisel";

        public ChiselTool()
        {
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/dirt-crunch-5.wav");
            this.AssociatedSound.RandomPitch = 1.2f;
        }
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            GlobalConstants.GameManager.DiggingSpace.DamageCell(hit, 3);
            return this.Cost;
        }
    }
}