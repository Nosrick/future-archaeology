using Godot;
using System;

public class ToolCooldownButton : Button
{
    protected TextureProgress ChildTextureProgress;
    
    public override void _Ready()
    {
        this.ChildTextureProgress = this.GetNodeOrNull<TextureProgress>("CooldownOverlay");
    }

    public void SetValue(double value)
    {
        this.ChildTextureProgress.Value = value;
    }

    public void SetMin(double value)
    {
        this.ChildTextureProgress.MinValue = value;
    }

    public void SetMax(double value)
    {
        this.ChildTextureProgress.MaxValue = value;
        if (value == 0 
            && this.ChildTextureProgress.MinValue == 0
            && this.ChildTextureProgress.Visible)
        {
            this.ChildTextureProgress.Hide();
        }
        else if (value != 0
                 && this.ChildTextureProgress.MinValue != value
                 && !this.ChildTextureProgress.Visible)
        {
            this.ChildTextureProgress.Show();
        }
    }
}
