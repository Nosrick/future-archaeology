using Godot;

namespace DiggyDig.scripts
{
    public class GameManager : Spatial
    {
        protected GridMap DiggingSpace { get; set; }

        public override void _Ready()
        {
            this.DiggingSpace = this.GetNode<GridMap>("DigMap");
        }
    }
}