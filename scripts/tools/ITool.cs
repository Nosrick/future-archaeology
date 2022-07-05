using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.tools
{
    public interface ITool
    {
        string TranslationKey { get; }
        int Cost { get; }
        int UsageCooldown { get; }
        int CooldownTimer { get; }
        
        int TimesUsed { get; }

        bool IsUsable();

        void TickCooldown(int value = 1);
        
        AudioStreamRandomPitch AssociatedSound { get; }
        AABB Execute(Vector3Int hit, Vector3Int previous);
        Dictionary Save();
        void Load(Dictionary data);
    }
}