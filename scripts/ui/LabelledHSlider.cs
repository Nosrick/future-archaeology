using Godot;

namespace ATimeGoneBy.scripts.ui
{
    [Tool]
    public class LabelledHSlider : Control
    {
        protected Label NameLabel;
        protected Label MinValueLabel;
        protected Label MaxValueLabel;
        protected HSlider Slider;

        [Export] public double MinValue
        {
            get
            {
                if (this.Slider is null)
                {
                    return 0;
                }
            
                return this.Slider.MinValue;
            }
            set
            {
                if (this.Slider is null || this.MinValueLabel is null)
                {
                    return;
                }
                this.Slider.MinValue = value;
                this.MinValueLabel.Text = value.ToString("0.##");
            }
        }

        [Export] public double MaxValue
        {
            get
            {
                if (this.Slider is null)
                {
                    return 0;
                }
            
                return this.Slider.MaxValue;
            }
            set
            {
                if (this.Slider is null || this.MaxValueLabel is null)
                {
                    return;
                }
                this.Slider.MaxValue = value;
                this.MaxValueLabel.Text = value.ToString("0.##");
            }
        }

        [Export] public double Value
        {
            get
            {
                if (this.Slider is null)
                {
                    return 0;
                }
            
                return this.Slider.Value;
            }
            set
            {
                if (this.Slider is null)
                {
                    return;
                }
                this.Slider.Value = value;
            }
        }

        [Export] public string Text
        {
            get => this.NameLabel is null ? string.Empty : this.NameLabel.Text;
            set
            {
                if (this.NameLabel is null)
                {
                    return;
                }
                this.NameLabel.Text = value;
            }
        }

        [Signal]
        public delegate void ValueChanged();
    
        public override void _EnterTree()
        {
            this.NameLabel = this.GetNode<Label>("Name");
            this.MinValueLabel = this.GetNode<Label>("MinValueLabel");
            this.MaxValueLabel = this.GetNode<Label>("MaxValueLabel");
            this.Slider = this.GetNode<HSlider>("Slider");

            this.Slider.Connect("value_changed", this, nameof(this.OnValueChange));
        }

        public void OnValueChange(float value)
        {
            this.EmitSignal(nameof(ValueChanged), this.Value);
        }
    }
}
