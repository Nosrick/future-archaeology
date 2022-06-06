using System.Collections;
using System.Collections.Generic;
using Godot;

namespace DiggyDig.scripts.ui
{
    public class Options : Control
    {
        [Export] protected NodePath OptionContainerPath;

        protected Control OptionContainer;
        
        protected IDictionary<string, Control> OptionButtons { get; set; }

        public override void _Ready()
        {
            if (this.OptionContainerPath is null)
            {
                GD.PrintErr("OptionContainerPath is null!");
                return;
            }
            
            this.OptionContainer = this.GetNode<Control>(this.OptionContainerPath);

            this.OptionButtons = new Dictionary<string, Control>();

            foreach (Control control in this.OptionContainer.GetChildren())
            {
                if (control is CheckButton checkButton)
                {
                    this.OptionButtons.Add(checkButton.Text, checkButton);
                    bool result = GlobalConstants.AppManager.OptionHandler.GetOption<bool>(checkButton.Text);
                    checkButton.Pressed = result;
                }
                else if (control is LabelledHSlider hSlider)
                {
                    this.OptionButtons.Add(hSlider.Text, hSlider);
                    float result = GlobalConstants.AppManager.OptionHandler.GetOption<float>(hSlider.Text);
                    hSlider.Value = result;
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
                if (this.OptionButtons[name] is BaseButton button)
                {
                    button.Pressed = (bool) value;
                }
                else if (this.OptionButtons[name] is LabelledHSlider slider)
                {
                    slider.Value = (float) value;
                }
            }
        }

        public void ResetOptions()
        {
            GlobalConstants.AppManager.OptionHandler.ResetToDefaults();
            foreach (DictionaryEntry option in GlobalConstants.AppManager.OptionHandler.Options)
            {
                this.SetOption(option.Value, option.Key.ToString());
            }
        }
    }
}