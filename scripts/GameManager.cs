using System;
using System.Collections.Generic;
using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.tools;
using ATimeGoneBy.scripts.ui;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace ATimeGoneBy.scripts
{
    public class GameManager : Spatial
    {
        public DigMap DiggingSpace { get; protected set; }

        public IDictionary<string, ITool> ToolBox { get; protected set; }

        public List<Tuple<ArrayMesh, Material>> Items { get; protected set; }

        protected PackedScene PauseMenuPackedScene { get; set; }
        protected PauseMenu PauseMenuScreen { get; set; }
        
        protected bool CompletedTutorial { get; set; }

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
        [Export] protected NodePath CameraIconContainerPath;
        [Export] protected NodePath ToolButtonContainerPath;
        [Export] protected NodePath UIRootPath;
        
        protected Dictionary ToolButtons;

        protected Label ToolLabel { get; set; }
        
        protected TutorialSpeech TutorialSpeech { get; set; }
        
        protected HBoxContainer CameraIconContainer { get; set; }
        protected List<Control> CameraIcons { get; set; }

        protected AudioStreamPlayer3D ToolAudioPlayer { get; set; }

        public OrbitCamera Camera { get; protected set; }
        
        public bool ProcessClicks { get; set; }

        protected bool CheckForUncovered;
        protected int CheckDelay;
        protected AABB UncoverArea;

        protected const string CurrentToolString = "tools.current.label";

        protected const string ItemMeshPaths = "res://scenes/game/items";
        protected const string ItemMaterialPaths = "res://assets/materials/items";

        public override void _Ready()
        {
            this.Items = new List<Tuple<ArrayMesh, Material>>();

            List<string> files = new List<string>();
            Directory itemDir = new Directory();
            if (itemDir.Open(ItemMeshPaths) == Error.Ok)
            {
                itemDir.ListDirBegin(true, true);
                string file = itemDir.GetNext();
                while (file != string.Empty)
                {
                    if (file.EndsWith(".tres"))
                    {
                        files.Add(file);
                    }

                    file = itemDir.GetNext();
                }

                foreach (string f in files)
                {
                    ArrayMesh mesh = GD.Load(ItemMeshPaths + "/" + f) as ArrayMesh;
                    Material material = GD.Load(ItemMaterialPaths + "/" + f) as Material;
                    if (mesh is null == false
                        && material is null == false)
                    {
                        this.Items.Add(new Tuple<ArrayMesh, Material>(mesh, material));
                    }
                }
            }

            this.PauseMenuPackedScene = GD.Load<PackedScene>(GlobalConstants.PauseMenuLocation);

            this.DiggingSpace = this.GetNode<DigMap>("DigMap");
            this.ToolAudioPlayer = this.DiggingSpace.GetNode<AudioStreamPlayer3D>("ToolSounds");

            this.ToolLabel = this.GetNodeOrNull<Label>(this.ToolLabelPath);
            this.Camera = this.GetNodeOrNull<OrbitCamera>(this.CameraPath);
            this.CameraIconContainer = this.GetNodeOrNull<HBoxContainer>(this.CameraIconContainerPath);

            if (this.ToolLabel is null == false)
            {
                this.ToolLabel.Text = this.Tr(CurrentToolString) + this.Tr("tools.none.name");
            }

            this.ToolBox = new System.Collections.Generic.Dictionary<string, ITool>
            {
                {"brush", new BrushTool()},
                {"chisel", new ChiselTool()},
                {"hammer", new HammerTool()},
                {"laser", new LaserTool()},
                {"bomb", new BombTool()},
                {"survey", new SurveyTool()}
            };

            this.ToolButtons = new Dictionary();
            foreach (ToolCooldownButton toolButton in this.GetNode(this.ToolButtonContainerPath).GetChildren())
            {
                if (toolButton.Visible == false)
                {
                    continue;
                }
                
                string toolName = this.Tr(toolButton.Text.ToLower());
                this.ToolButtons.Add(toolName, toolButton);
                if (!this.ToolBox.ContainsKey(toolName))
                {
                    continue;
                }
                
                ITool tool = this.ToolBox[toolName];
                toolButton.SetMax(tool.UsageCooldown);
                toolButton.SetValue(0);
            }

            this.CameraIcons = new List<Control>();
            foreach (Control child in this.CameraIconContainer.GetChildren())
            {
                child.Hide();
                this.CameraIcons.Add(child);
            }

            this.TutorialSpeech = this.GetNode<TutorialSpeech>(this.UIRootPath + "/TutorialSpeech/SpeechContainer");

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

            if (this.Camera.Rotating && !this.CameraIcons[0].Visible)
            {
                this.CameraIcons[0].Show();
            }
            else if(!this.Camera.Rotating && this.CameraIcons[0].Visible)
            {
                this.CameraIcons[0].Hide();
            }

            if (this.Camera.Panning && !this.CameraIcons[1].Visible)
            {
                this.CameraIcons[1].Show();
            }
            else if(!this.Camera.Panning && this.CameraIcons[1].Visible)
            {
                this.CameraIcons[1].Hide();
            }

            if (this.Camera.Zooming && !this.CameraIcons[2].Visible)
            {
                this.CameraIcons[2].Show();
            }
            else if(!this.Camera.Zooming && this.CameraIcons[2].Visible)
            {
                this.CameraIcons[2].Hide();
            }
            
            if (@event is InputEventKey eventKey)
            {
                if (eventKey.IsActionReleased("open_pause_menu"))
                {
                    this.CompletedTutorial = true;
                    this.TutorialSpeech.Hide();
                    
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

            if (this.CheckForUncovered)
            {
                if (this.CheckDelay > 0)
                {
                    this.CheckDelay--;
                }

                if (this.CheckDelay == 0)
                {
                    this.DiggingSpace.CheckForUncovered(this.UncoverArea);
                    this.CheckForUncovered = false;
                }
            }

            if (this.DiggingSpace.LevelComplete())
            {
                this.DiggingSpace.SetPhysicsProcess(false);
                GD.Print("LEVEL COMPLETE!");
                this.DiggingSpace.GenerateDigSite(new Vector3Int(3, 3, 3));
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
            
            this.UncoverArea = this.CurrentTool.Execute(hit, previous);
            
            var sound = GlobalConstants.GameManager.CurrentTool.AssociatedSound;
            if (this.ToolAudioPlayer.Stream != sound)
            {
                this.ToolAudioPlayer.Stream = sound;
            }
            this.ToolAudioPlayer.Play();

            string name = this.Tr(GlobalConstants.GameManager.CurrentTool.TranslationKey);
            ToolCooldownButton toolCooldownButton = this.ToolButtons[name] as ToolCooldownButton;
            toolCooldownButton?.SetValue(GlobalConstants.GameManager.CurrentTool.UsageCooldown);

            this.CheckDelay = 2;
            this.CheckForUncovered = true;
        }

        public void TickCooldowns()
        {
            foreach (ITool tool in this.ToolBox.Values)
            {
                if (tool.IsUsable())
                {
                    continue;
                }

                string toolName = this.Tr(tool.TranslationKey);
                ToolCooldownButton button = this.ToolButtons[toolName] as ToolCooldownButton;
                if (button is null)
                {
                    continue;
                }
                
                tool.TickCooldown();
                button.SetValue(tool.CooldownTimer);
            }
        }

        protected void RefreshCooldownProgressBars()
        {
            foreach (ITool tool in this.ToolBox.Values)
            {
                if (tool.IsUsable())
                {
                    continue;
                }

                string toolName = this.Tr(tool.TranslationKey);
                ToolCooldownButton button = this.ToolButtons[toolName] as ToolCooldownButton;
                if (button is null)
                {
                    continue;
                }
                
                button.SetMax(tool.UsageCooldown);
                button.SetValue(tool.CooldownTimer);
            }
        }

        public void GenerateLevel()
        {
            if (!this.CompletedTutorial)
            {
                this.RunTutorial();
            }
            else
            {
                this.TutorialSpeech.Hide();
            }
            if (GlobalConstants.AppManager.SaveState is null == false)
            {
                return;
            }
            
            this.DiggingSpace.GenerateDigSite(new Vector3Int(3, 3, 3), 4);
        }

        public void RunTutorial()
        {
            
        }

        public Dictionary Save()
        {
            Dictionary saveDict = new Dictionary();
            
            saveDict.Add("cash", this.Cash);
            saveDict.Add("dig-site", this.DiggingSpace.Save());
            saveDict.Add("tutorial-done", this.CompletedTutorial);

            Array tools = new Array();
            
            foreach(ITool tool in this.ToolBox.Values)
            {
                tools.Add(tool.Save());
            }
            
            saveDict.Add("tools", tools);

            return saveDict;
        }

        public bool Load(Dictionary data)
        {
            this.Cash = (int) data["cash"];
            this.DiggingSpace.Load(data["dig-site"] as Dictionary);
            this.CompletedTutorial = (bool) data["tutorial-done"];

            Array tools = (Array) data["tools"];

            foreach (Dictionary tool in tools)
            {
                string name = this.Tr((string) tool[AbstractTool.NAME_KEY]);
                if (this.ToolBox.ContainsKey(name))
                {
                    this.ToolBox[name].Load(tool);
                }
            }
            
            this.RefreshCooldownProgressBars();
            
            this.DiggingSpace.SetProcess(true);
            this.DiggingSpace.SetPhysicsProcess(true);

            return true;
        }
    }
}