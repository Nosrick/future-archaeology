namespace DiggyDig.scripts
{
    public interface ITool
    {
        string Name { get; }
        int Cost { get; }
        int Execute(Vector3Int t);
    }
}