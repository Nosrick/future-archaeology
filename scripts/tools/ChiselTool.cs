using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public class ChiselTool : ITool
    {
        public int Cost => 10;
        public int UsageCooldown => 0;
        public int CooldownTimer => 0;
        public bool IsUsable()
        {
            return true;
        }

        public AudioStreamRandomPitch AssociatedSound { get; protected set; }
        public string TranslationKey => "tools.chisel.name";

        public ChiselTool()
        {
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/dirt-crunch-5.wav");
            this.AssociatedSound.RandomPitch = 1.1f;
        }
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            GlobalConstants.GameManager.DiggingSpace.DamageCell(hit, 3);
            return this.Cost;
        }
    }
}