using System.Collections.Generic;
using DiggyDig.scripts.digging;
using DiggyDig.scripts.Tools;
using DiggyDig.scripts.utils;
using Godot;

namespace DiggyDig.scripts
{
    public class GameManager : Spatial
    {
        public DigMap DiggingSpace { get; protected set; }

        public IDictionary<string, ITool> ToolBox { get; protected set; }
        
        protected PackedScene OptionsMenu { get; set; }

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
            get => this.m_Cash;
            protected set
            {
                this.m_Cash = value;
                this.CashLabel.Text = CashString + this.m_Cash;
            }
        }

        protected int m_Cash;

        [Export] protected NodePath CashLabelPath;
        [Export] protected NodePath ToolLabelPath;
        [Export] protected NodePath CameraPath;
        
        protected Label CashLabel { get; set; }
        protected Label ToolLabel { get; set; }
        
        public OrbitCamera Camera { get; protected set; }

        protected const string CashString = "BiggaBux: ";
        protected const string CurrentToolString = "Current Tool: ";

        public override void _Ready()
        {
            this.OptionsMenu = GD.Load<PackedScene>("scenes/ui/Options.tscn");
            
            this.DiggingSpace = this.GetNode<DigMap>("DigMap");

            this.CashLabel = this.GetNodeOrNull<Label>(this.CashLabelPath);
            this.ToolLabel = this.GetNodeOrNull<Label>(this.ToolLabelPath);
            this.Camera = this.GetNodeOrNull<OrbitCamera>(this.CameraPath);

            if (this.CashLabel is null == false)
            {
                this.CashLabel.Text = CashString + this.Cash;
            }

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
                if (eventKey.IsActionPressed("open_pause_menu"))
                {
                    Node options = this.OptionsMenu.Instance();
                    this.AddChild(options);

                    options.Connect("tree_exiting", this, "RefreshCameraOptions");
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
            if (this.CurrentTool is null)
            {
                return;
            }

            if (this.Cash - this.CurrentTool.Cost < 0)
            {
                GD.Print("Take out a loan!");
                return;
            }

            this.Cash -= this.CurrentTool.Execute(hit, previous);
        }
    }
}