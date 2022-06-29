using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public abstract class AbstractTool : ITool
    {
        public virtual string TranslationKey => "Someone forgot to override the TranslationKey from AbstractTool!";
        public int Cost { get; protected set; }
        public int UsageCooldown { get; protected set; }
        public int CooldownTimer { get; protected set; }
        public virtual bool IsUsable()
        {
            return this.CooldownTimer == 0;
        }

        public virtual void TickCooldown(int value = 1)
        {
            this.CooldownTimer = Mathf.Max(0, this.CooldownTimer - value);
        }

        public AudioStreamRandomPitch AssociatedSound { get; protected set; }
        public abstract AABB Execute(Vector3Int hit, Vector3Int previous);
    }
}