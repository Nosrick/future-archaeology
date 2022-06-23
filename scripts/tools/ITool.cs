﻿using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts.tools
{
    public interface ITool
    {
        string TranslationKey { get; }
        int Cost { get; }
        int UsageCooldown { get; }
        int CooldownTimer { get; }

        bool IsUsable();
        
        AudioStreamRandomPitch AssociatedSound { get; }
        int Execute(Vector3Int hit, Vector3Int previous);
    }
}