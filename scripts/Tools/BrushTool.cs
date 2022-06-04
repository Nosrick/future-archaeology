using Godot;

namespace DiggyDig.scripts
{
    public class BrushTool : ITool
    {
        public int Cost => 5;
        public string Name => "Brush";
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            int cell = GlobalConstants.GameManager.DiggingSpace.GetCellItem(hit.x, hit.y, hit.z);
            GlobalConstants.GameManager.DiggingSpace.SetCellItem(hit.x, hit.y, hit.z, cell + 1);

            return this.Cost;
        }
    }
}