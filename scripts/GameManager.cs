using System.Collections.Generic;
using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.tools;
using ATimeGoneBy.scripts.ui;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts
{
    public class GameManager : Spatial
    {
        public DigMap DiggingSpace { get; protected set; }

        public IDictionary<string, ITool> ToolBox { get; protected set; }

        protected PackedScene PauseMenuPackedScene { get; set; }
        protected PauseMenu PauseMenuScreen { get; set; }

        public ITool CurrentTool
        {
            get => this.m_CurrentTool;
            protected set
            {
                this.m_CurrentTool = value;
                this.ToolLabel.Text = this.Tr(CurrentToolString) + this.Tr(this.m_CurrentTool.TranslationKey);
            }
        }

        protected ITool m_CurrentTool;

        public int Cash
        {
            get;
            set;
        }

        [Export] protected NodePath ToolLabelPath;
        [Export] protected NodePath CameraPath;

        protected Label ToolLabel { get; set; }

        public OrbitCamera Camera { get; protected set; }
        
        public bool ProcessClicks { get; set; }

        protected const string CurrentToolString = "tools.current.label";

        public override void _Ready()
        {
            this.PauseMenuPackedScene = GD.Load<PackedScene>(GlobalConstants.PauseMenuLocation);

            this.DiggingSpace = this.GetNode<DigMap>("DigMap");

            this.ToolLabel = this.GetNodeOrNull<Label>(this.ToolLabelPath);
            this.Camera = this.GetNodeOrNull<OrbitCamera>(this.CameraPath);

            if (this.ToolLabel is null == false)
            {
                this.ToolLabel.Text = this.Tr(CurrentToolString) + this.Tr("tools.none.name");
            }

            this.ToolBox = new System.Collections.Generic.Dictionary<string, ITool>
            {
                {"brush", new BrushTool()},
                {"chisel", new ChiselTool()},
                {"hammer", new HammerTool()},
                {"bomb", new BombTool()}
            };

            this.Cash = 500;

            this.ProcessClicks = true;

            if (GlobalConstants.AppManager.SaveState is null == false)
            {
                this.Load(GlobalConstants.AppManager.SaveState);
            }

            GlobalConstants.GameManager = this;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (@event is InputEventKey eventKey)
            {
                if (eventKey.IsActionReleased("open_pause_menu"))
                {
                    if (this.PauseMenuScreen is null || IsInstanceValid(this.PauseMenuScreen) == false)
                    {
                        this.PauseMenuScreen = this.PauseMenuPackedScene.Instance<PauseMenu>();
                        this.AddChild(this.PauseMenuScreen);

                        this.ProcessClicks = false;
                    }
                    else if (IsInstanceValid(this.PauseMenuScreen))
                    {
                        if (this.PauseMenuScreen.Visible == false)
                        {
                            this.PauseMenuScreen.Show();
                            this.ProcessClicks = false;
                        }
                        else
                        {
                            this.PauseMenuScreen.Hide();
                            this.ProcessClicks = true;
                        }
                    }
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (this.DiggingSpace.LevelComplete())
            {
                this.DiggingSpace.SetPhysicsProcess(false);
                GD.Print("LEVEL COMPLETE!");
                this.DiggingSpace.GenerateDigSite();
            }
        }

        public void RefreshCameraOptions()
        {
            this.Camera.RefreshOptions();
        }

        public void SetTool(string tool)
        {
            if (this.ToolBox.ContainsKey(tool))
            {
                this.CurrentTool = this.ToolBox[tool];
            }
        }

        public void ExecuteTool(Vector3Int hit, Vector3Int previous)
        {
            this.CurrentTool?.Execute(hit, previous);
        }

        public void GenerateLevel()
        {
            this.DiggingSpace.GenerateDigSite();
        }

        public Dictionary Save()
        {
            Dictionary saveDict = new Dictionary();
            
            saveDict.Add("cash", this.Cash);
            saveDict.Add("dig-site", this.DiggingSpace.Save());

            return saveDict;
        }

        public bool Load(Dictionary data)
        {
            this.Cash = (int) data["cash"];
            this.DiggingSpace.Load(data["dig-site"] as Dictionary);

            return true;
        }
    }
}