namespace DiggyDig.scripts.Tools
{
    public interface ITool
    {
        string Name { get; }
        int Cost { get; }
        int Execute(Vector3Int hit, Vector3Int previous);
    }
}