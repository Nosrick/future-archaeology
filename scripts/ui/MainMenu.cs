using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.ui
{
    public class MainMenu : Control
    {
        protected PackedScene OptionsScreen;
        protected PackedScene GameScreen;

        public override void _Ready()
        {
            this.OptionsScreen = GD.Load<PackedScene>(GlobalConstants.OptionsMenuLocation);
            this.GameScreen = GD.Load<PackedScene>(GlobalConstants.GameSceneLocation);
        }

        public void ChangeScreen(string screen)
        {
            if (screen == "new-game")
            {
                this.GetTree().ChangeSceneTo(this.GameScreen);
            }
            else if (screen == "load-game")
            {
                File saveGame = new File();
                if (saveGame.Open("user://save.dat", File.ModeFlags.Read) != Error.Ok)
                {
                    GD.PrintErr("COULD NOT OPEN SAVE FILE");
                    return;
                }

                var obj = saveGame.GetVar();
                if (!(obj is Dictionary gameState))
                {
                    GD.PrintErr("COULD NOT PARSE SAVE TO NODE");
                    return;
                }

                GlobalConstants.AppManager.SaveState = gameState;
                
                GD.Print("Seems legit?");
                this.GetTree().ChangeScene(GlobalConstants.GameSceneLocation);
            }
            else if (screen == "options")
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