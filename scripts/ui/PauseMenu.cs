using Godot;

namespace ATimeGoneBy.scripts.ui
{
    public class PauseMenu : Control
    {
        protected PackedScene OptionsPackedScene;

        protected Options OptionsMenu;

        public override void _Ready()
        {
            base._Ready();

            this.OptionsPackedScene = GD.Load<PackedScene>(GlobalConstants.OptionsMenuLocation);
        }

        public void Resume()
        {
            this.Hide();
        }

        public void SaveGame()
        {
            File saveFile = new File();
            if (saveFile.Open("user://save.dat", File.ModeFlags.Write) == Error.Ok)
            {
                saveFile.StoreVar(GlobalConstants.GameManager.DiggingSpace.Save());
                saveFile.Close();
                
                GD.Print("Game saved!");
            }
            else
            {
                GD.PrintErr("Could not save game.");
            }
        }

        public void DisplayOptions()
        {
            if (IsInstanceValid(this.OptionsMenu) == false)
            {
                this.OptionsMenu = this.OptionsPackedScene.Instance<Options>();
                this.AddChild(this.OptionsMenu);
            }
            
            this.OptionsMenu.Show();
        }

        public void ToMenu()
        {
            this.GetTree().ChangeScene(GlobalConstants.MainMenuLocation);
        }

        public void ExitGame()
        {
            this.GetTree().Quit(0);
        }
    }
}