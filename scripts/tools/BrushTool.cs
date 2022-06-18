using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class BrushTool : ITool
    {
        public int Cost => 5;
        public string TranslationKey => "tools.brush.name";
        public AudioStreamRandomPitch AssociatedSound { get; protected set; }

        public BrushTool()
        {
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/brush-3.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            GlobalConstants.GameManager.DiggingSpace.DamageCell(hit, 1);

            return this.Cost;
        }
    }
}