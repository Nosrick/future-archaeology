using Godot;

namespace DiggyDig.scripts.options
{
    public class Option<T> : Resource, IObjectOption
    {
        public string Name { get; protected set; }
        public object ObjectValue { get; set; }

        T Value
        {
            get => (T) this.ObjectValue;
            set => this.ObjectValue = value;
        }

        public Option(string name, T value = default)
        {
            this.Name = name;
            this.ObjectValue = value;
        }
    }
}