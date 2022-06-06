using System.Collections.Generic;
using Godot;

namespace DiggyDig.scripts.ui
{
    public class Options : Control
    {
        [Export] protected NodePath OptionContainerPath;

        protected Control OptionContainer;
        
        protected IDictionary<string, CheckButton> OptionButtons { get; set; }

        public override void _Ready()
        {
            if (this.OptionContainerPath is null)
            {
                GD.PrintErr("OptionContainerPath is null!");
                return;
            }
            
            this.OptionContainer = this.GetNode<Control>(this.OptionContainerPath);

            this.OptionButtons = new Dictionary<string, CheckButton>();

            foreach (Control control in this.OptionContainer.GetChildren())
            {
                if (control is CheckButton checkButton)
                {
                    this.OptionButtons.Add(checkButton.Text, checkButton);
                    bool result = GlobalConstants.AppManager.OptionHandler.GetOption(checkButton.Text);
                    checkButton.Pressed = result;
                }
            }
        }

        public void CloseMe()
        {
            GlobalConstants.AppManager.OptionHandler.SaveOptions();
            this.QueueFree();
        }

        public void SetOption(object value, string name)
        {
            if (this.OptionButtons.ContainsKey(name))
            {
                GlobalConstants.AppManager.OptionHandler.SetOption(name, value);
                this.OptionButtons[name].Pressed = (bool) value;
            }
        }
    }
}