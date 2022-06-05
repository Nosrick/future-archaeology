using System.Collections.Generic;
using DiggyDig.scripts.Tools;
using Godot;

namespace DiggyDig.scripts
{
    public class GameManager : Spatial
    {
        public DigMap DiggingSpace { get; protected set; }

        public IDictionary<string, ITool> ToolBox { get; protected set; }

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
        
        public Camera Camera { get; protected set; }

        protected const string CashString = "BiggaBux: ";
        protected const string CurrentToolString = "Current Tool: ";

        public override void _Ready()
        {
            this.DiggingSpace = this.GetNode<DigMap>("DigMap");

            this.CashLabel = this.GetNodeOrNull<Label>(this.CashLabelPath);
            this.ToolLabel = this.GetNodeOrNull<Label>(this.ToolLabelPath);
            this.Camera = this.GetNodeOrNull<Camera>(this.CameraPath);

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

            this.Cash -= this.CurrentTool.Execute(hit, previous);
        }
    }
}