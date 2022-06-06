using Godot;

namespace DiggyDig.scripts.ui
{
    public class MainMenu : Control
    {
        protected PackedScene OptionsScreen;
        protected PackedScene GameScreen;

        public override void _Ready()
        {
            this.OptionsScreen = GD.Load<PackedScene>("scenes/ui/Options.tscn");
            this.GameScreen = GD.Load<PackedScene>("scenes/GameScene.tscn");
        }

        public void ChangeScreen(string screen)
        {
            if (screen == "new-game")
            {
                this.GetTree().ChangeSceneTo(this.GameScreen);
            }

            if (screen == "options")
            {
                this.AddChild(this.OptionsScreen.Instance());
            }
            else if (screen == "exit")
            {
                this.GetTree().Quit(0);
            }
        }
    }
}