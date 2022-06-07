using DiggyDig.scripts.utils;
using Godot;

namespace DiggyDig.scripts.Tools
{
    public interface ITool
    {
        string Name { get; }
        int Cost { get; }
        AudioStreamRandomPitch AssociatedSound { get; }
        int Execute(Vector3Int hit, Vector3Int previous);
    }
}