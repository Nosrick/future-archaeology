namespace DiggyDig.scripts
{
    public class BrushTool : ITool
    {
        public int Cost => 5;
        public string Name => "Brush";
        
        public int Execute(Vector3Int t)
        {
            int cell = GlobalConstants.GameManager.DiggingSpace.GetCellItem(t.x, t.y, t.z);
            GlobalConstants.GameManager.DiggingSpace.SetCellItem(t.x, t.y, t.z, cell + 1);

            return this.Cost;
        }
    }
}