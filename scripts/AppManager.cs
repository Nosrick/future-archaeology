using ATimeGoneBy.scripts.options;
using Godot;

namespace ATimeGoneBy.scripts
{
    public class AppManager : Spatial
    {
        protected PackedScene MainMenu;
        protected PackedScene GameScene;

        public OptionHandler OptionHandler { get; protected set; }

        public override void _Ready()
        {
            this.OptionHandler = new OptionHandler();
            this.MainMenu = GD.Load<PackedScene>("scenes/ui/MainMenu.tscn");
        
            this.AddChild(this.MainMenu.Instance());

            GlobalConstants.AppManager = this;
        }
    }
}