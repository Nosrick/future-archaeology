namespace ATimeGoneBy.scripts
{
    public class GlobalConstants
    {
        public static GameManager GameManager { get; set; }
        
        public static AppManager AppManager { get; set; }

        public const string OptionsMenuLocation = "scenes/ui/Options.tscn";
        public const string GameSceneLocation = "scenes/game/GameScene.tscn";
        public const string PauseMenuLocation = "scenes/ui/PauseMenu.tscn";
        public const string MainMenuLocation = "scenes/ui/MainMenu.tscn";

        public const string DigItemLocation = "scenes/game/DiggingObject.tscn";
    }
}