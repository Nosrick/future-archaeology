using System.Collections.Generic;
using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.tools;
using ATimeGoneBy.scripts.ui;
using ATimeGoneBy.scripts.utils;
using Godot;

namespace ATimeGoneBy.scripts
{
    public class GameManager : Spatial
    {
        public DigMap DiggingSpace { get; protected set; }

        public IDictionary<string, ITool> ToolBox { get; protected set; }

        protected PackedScene OptionsPackedScene { get; set; }
        protected Options OptionsScreen { get; set; }

        public ITool CurrentTool
        {
            get => this.m_CurrentTool;
            protected set
            {
                this.m_CurrentTool = value;
                this.ToolLabel.Text = CurrentToolString + this.m_CurrentTool.Name;
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

        protected const string CashString = "BiggaBux: ";
        protected const string CurrentToolString = "Current Tool: ";

        public override void _Ready()
        {
            this.OptionsPackedScene = GD.Load<PackedScene>("scenes/ui/Options.tscn");

            this.DiggingSpace = this.GetNode<DigMap>("DigMap");

            this.ToolLabel = this.GetNodeOrNull<Label>(this.ToolLabelPath);
            this.Camera = this.GetNodeOrNull<OrbitCamera>(this.CameraPath);

            if (this.ToolLabel is null == false)
            {
                this.ToolLabel.Text = CurrentToolString + "None";
            }

            this.ToolBox = new Dictionary<string, ITool>
            {
                {"brush", new BrushTool()},
                {"chisel", new ChiselTool()},
                {"hammer", new HammerTool()},
                {"bomb", new BombTool()}
            };

            this.Cash = 500;

            GlobalConstants.GameManager = this;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (@event is InputEventKey eventKey)
            {
                if (eventKey.IsActionReleased("open_pause_menu"))
                {
                    if (this.OptionsScreen is null || IsInstanceValid(this.OptionsScreen) == false)
                    {
                        this.OptionsScreen = this.OptionsPackedScene.Instance<Options>();
                        this.AddChild(this.OptionsScreen);

                        this.OptionsScreen.Connect("tree_exiting", this, "RefreshCameraOptions");
                    }
                    else if (IsInstanceValid(this.OptionsScreen))
                    {
                        this.OptionsScreen?.CloseMe();
                        this.Camera.RefreshOptions();
                    }
                }
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
    }
}