namespace DiggyDig.scripts.options
{
    public interface IObjectOption
    {
        string Name { get; }
        object ObjectValue { get; set; }
    }
}