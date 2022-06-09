using ATimeGoneBy.scripts;
using Godot;

public class LoanModal : Control
{
    protected static int LoanAmount = 1000;
    
    public override void _Ready()
    {
    }

    public void CloseMe()
    {
        this.QueueFree();
    }

    public void AcceptLoan()
    {
        GlobalConstants.GameManager.Cash += LoanAmount;
        GlobalConstants.GameManager.AccumulatedLoan += LoanAmount;

        LoanAmount *= 2;
        
        this.QueueFree();
    }
}
