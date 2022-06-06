using DiggyDig.scripts;
using Godot;

public class AppManager : Spatial
{
    protected PackedScene MainMenu;
    protected PackedScene GameScene;

    public override void _Ready()
    {
        this.MainMenu = GD.Load<PackedScene>("scenes/ui/MainMenu.tscn");
        
        this.AddChild(this.MainMenu.Instance());

        GlobalConstants.AppManager = this;
    }
}
