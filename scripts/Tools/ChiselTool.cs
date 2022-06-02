namespace DiggyDig.scripts
{
    public class ChiselTool : ITool
    {
        public int Cost => 10;
        public string Name => "Chisel";
        
        public int Execute(Vector3Int t)
        {
            int cell = GlobalConstants.GameManager.DiggingSpace.GetCellItem(t.x, t.y, t.z);
            GlobalConstants.GameManager.DiggingSpace.SetCellItem(t.x, t.y, t.z, cell + 3);
            return this.Cost;
        }
    }
}