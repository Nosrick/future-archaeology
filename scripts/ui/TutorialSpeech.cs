using System;
using System.Collections.Generic;
using System.Text;
using ATimeGoneBy.scripts;
using ATimeGoneBy.scripts.tools;
using ATimeGoneBy.scripts.utils;
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

    protected int ActionCounter = 0;

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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (this.CheckIfActionPerformed()
            && this.CheckIfActionCompleted())
        {
            this.ActionCounter = 0;
            this.Advance();
        }
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
            if (buttonEnum == ButtonList.MaskMiddle)
            {
                return "WheelUp";
            }
            return Enum.GetName(typeof(ButtonList), buttonEnum);
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

    protected bool CheckIfActionRequired()
    {
        switch (this.Index)
        {
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 9:
            case 10:
            case 11:
                return true;
        }

        return false;
    }

    protected bool CheckIfActionPerformed()
    {
        OrbitCamera camera = GlobalConstants.GameManager.Camera;

        switch (this.Index)
        {
            case 3:
                if (camera.Rotating)
                {
                    this.ActionCounter++;
                    return true;
                }

                break;

            case 4:
                if (camera.Panning)
                {
                    this.ActionCounter++;
                    return true;
                }

                break;

            case 5:
                if (camera.Zooming)
                {
                    this.ActionCounter++;
                    return true;
                }

                break;

            case 6:
                if (GlobalConstants.GameManager.CurrentTool is SurveyTool)
                {
                    this.ActionCounter++;
                    return true;
                }

                break;

            case 7:
                if (GlobalConstants.GameManager.CurrentTool is SurveyTool surveyTool
                    && surveyTool.TimesUsed > 0)
                {
                    this.ActionCounter++;
                    return true;
                }

                break;

            case 9:
                if (GlobalConstants.GameManager.CurrentTool is ChiselTool chisel)
                {
                    this.ActionCounter = chisel.TimesUsed;
                    return true;
                }

                break;

            case 10:
                if (GlobalConstants.GameManager.DiggingSpace.IsAnyUncovered())
                {
                    this.ActionCounter++;
                    return true;
                }

                break;
            
            case 11:
                if (GlobalConstants.GameManager.DiggingSpace.LevelTouched)
                {
                    this.ActionCounter++;
                    return true;
                }

                break;
        }

        return false;
    }

    protected bool CheckIfActionCompleted()
    {
        switch (this.Index)
        {
            case 3:
                return this.ActionCounter >= 10;

            case 4:
                return this.ActionCounter >= 10;

            case 5:
                return this.ActionCounter >= 4;

            case 6:
            case 7:
                return this.ActionCounter >= 1;
            case 9:
                return this.ActionCounter >= 3;
            case 10:
            case 11:
                return this.ActionCounter >= 1;
        }

        return false;
    }

    public void ClickedOn(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseButton
            && mouseButton.Pressed
            && mouseButton.ButtonIndex == (int) ButtonList.Left
            && this.CheckIfActionRequired() == false)
        {
            this.Advance();
        }
    }
}