using Godot;

public class Options : Control
{
    public override void _Ready()
    {
        
    }

    public void ReturnToMainMenu()
    {
        this.GetTree().ChangeScene("scenes/ui/MainMenu.tscn");
    }
}
