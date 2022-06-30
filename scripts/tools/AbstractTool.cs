using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.tools
{
    public abstract class AbstractTool : ITool
    {
        public const string NAME_KEY = "name-key";
        public const string COST_KEY = "cost";
        public const string USAGE_KEY = "usage-cooldown";
        public const string TIMER_KEY = "cooldown-timer";
        public const string DAMAGE_KEY = "damage";
        public const string RANGE_KEY = "range";

        public virtual string TranslationKey { get; protected set; }

        public int Cost { get; protected set; }
        public int UsageCooldown { get; protected set; }
        public int CooldownTimer { get; protected set; }

        public AudioStreamRandomPitch AssociatedSound { get; protected set; }

        public virtual bool IsUsable()
        {
            return this.CooldownTimer == 0;
        }

        public virtual Dictionary Save()
        {
            Dictionary saveDict = new Dictionary
            {
                {NAME_KEY, this.TranslationKey},
                {COST_KEY, this.Cost},
                {USAGE_KEY, this.UsageCooldown},
                {TIMER_KEY, this.CooldownTimer}
            };

            return saveDict;
        }

        public virtual void Load(Dictionary data)
        {
            this.TranslationKey = (string) data[NAME_KEY];
            this.Cost = (int) data[COST_KEY];
            this.UsageCooldown = (int) data[USAGE_KEY];
            this.CooldownTimer = (int) data[TIMER_KEY];
        }

        public virtual void TickCooldown(int value = 1)
        {
            this.CooldownTimer = Mathf.Max(0, this.CooldownTimer - value);
        }

        public abstract AABB Execute(Vector3Int hit, Vector3Int previous);
    }
}