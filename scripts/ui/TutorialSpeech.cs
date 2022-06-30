using System.Collections.Generic;
using Godot;

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

        for (int i = 1; i <= MAX; i++)
        {
            this.TutorialText.Add("tutorial." + i);
        }

        this.SpeechLabel.Text = this.Tr(this.TutorialText[0]);
        this.SpeakerLabel.Text = this.Tr(SpeakerTranslationKey);
    }

    public void Advance()
    {
        if (this.Index < MAX)
        {
            this.Index++;
            this.SpeechLabel.Text = this.Tr(this.TutorialText[this.Index]);
        }
        else
        {
            this.Hide();
        }
    }

    public void ClickedOn(InputEvent inputEvent)
    {
        if (inputEvent.IsActionReleased("ui_accept"))
        {
            this.Advance();
        }
    }
}
