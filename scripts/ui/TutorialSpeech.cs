using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using Array = Godot.Collections.Array;

public class TutorialSpeech : Control
{
    [Export] protected NodePath SpeakerLabelPath;
    [Export] protected NodePath SpeechLabelPath;

    protected Label SpeakerLabel;
    protected RichTextLabel SpeechLabel;

    protected int Index;

    protected List<string> TutorialText;
    
    protected const string SpeakerTranslationKey = "tutorial.speaker";

    protected const int MAX = 13;
    
    public override void _Ready()
    {
        this.SpeakerLabel = this.GetNode<Label>(this.SpeakerLabelPath);
        this.SpeechLabel = this.GetNode<RichTextLabel>(this.SpeechLabelPath);

        this.TutorialText = new List<string>();
        
        for (int i = 0; i < MAX; i++)
        {
            this.TutorialText.Add("tutorial." + i);
        }

        this.SpeechLabel.Text = this.Tr(this.TutorialText[0]);
        this.SpeakerLabel.Text = this.Tr(SpeakerTranslationKey);
    }

    public void Advance()
    {
        if (this.Index < MAX - 1)
        {
            this.Index++;
            string text = this.Tr(this.TutorialText[this.Index]);
            string fetchButtonStrings = this.FetchButtonStrings(this.Index);
            if (fetchButtonStrings != string.Empty)
            {
                text = text.Replace("[%" + this.Index + "%]", "[color=yellow][" + fetchButtonStrings + "][/color]");
            }

            this.SpeechLabel.BbcodeText = text;
        }
        else
        {
            this.Hide();
        }
    }

    protected string FetchActionString(InputEvent inputEvent)
    {
        ButtonList buttonEnum;
        JoystickList padEnum;
        
        if (inputEvent is InputEventKey key)
        {
            return OS.GetScancodeString(key.Scancode);
        }
        if (inputEvent is InputEventMouseButton mouseButton
                 && Enum.TryParse(mouseButton.ButtonIndex.ToString(), out buttonEnum))
        {
            return buttonEnum.ToString();
        }
        if (inputEvent is InputEventJoypadButton joypadButton
                 && Enum.TryParse(joypadButton.ButtonIndex.ToString(), out padEnum))
        {
            return padEnum.ToString();
        }

        return string.Empty;
    }

    protected string FetchButtonStrings(int index)
    {
        string buttons = string.Empty;
        Array actionArray = new Array();
        List<string> actionsList = new List<string>();
        switch (index)
        {
            case 3:
                actionArray = InputMap.GetActionList("camera_rotate_modifier");
                break;
            
            case 4:
                actionArray = InputMap.GetActionList("camera_pan_modifier");
                break;
            
            case 5:
                actionArray = InputMap.GetActionList("camera_zoom_in");
                foreach (InputEvent action in InputMap.GetActionList("camera_zoom_out"))
                {
                    actionArray.Add(action);
                }
                break;
            
            case 6:
            case 9:
                actionArray = InputMap.GetActionList("ui_accept");
                break;
        }
                
        foreach (InputEvent inputEvent in actionArray)
        {
            actionsList.Add(this.FetchActionString(inputEvent));
        }

        buttons = string.Join(", ", actionsList);

        return buttons;
    }

    public void ClickedOn(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseButton
            && mouseButton.Pressed
            && mouseButton.ButtonIndex == (int) ButtonList.Left)
        {
            this.Advance();
        }
    }
}
