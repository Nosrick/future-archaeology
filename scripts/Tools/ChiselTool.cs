namespace DiggyDig.scripts.Tools
{
    public class ChiselTool : ITool
    {
        public int Cost => 10;
        public string Name => "Chisel";
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            int cell = GlobalConstants.GameManager.DiggingSpace.GetCellItem(hit.x, hit.y, hit.z);
            GlobalConstants.GameManager.DiggingSpace.SetCellItem(hit.x, hit.y, hit.z, cell + 3);
            return this.Cost;
        }
    }
}