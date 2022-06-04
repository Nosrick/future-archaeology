using System.Linq;
using DiggyDig.scripts;
using Godot;

public class DigMap : GridMap
{
    protected int Width { get; set; }
    protected int Height { get; set; }
    protected int Depth { get; set; }

    public int[] ValidCells { get; protected set; }
    public const int FULL_CUBE = 0;
    public const int HALF_CUBE = 1;
    public const int BARE_CUBE = 2;
    public const int EMPTY_CELL = -1;
    
    public override void _Ready()
    {
        this.ValidCells = this.MeshLibrary.GetItemList();
        this.Width = 5;
        this.Height = 5;
        this.Depth = 5;
        
        for (int x = -this.Width; x <= this.Width; x++)
        {
            for (int y = -this.Height; y <= this.Height; y++)
            {
                for (int z = -this.Depth; z <= this.Depth; z++)
                {
                    this.SetCellItem(x, y, z, this.ValidCells[0]);
                }
            }
        }
    }

    public bool IsValid(Vector3Int pos)
    {
        return this.ValidCells.Contains(this.GetCellItem(pos.x, pos.y, pos.z));
    }

    public bool DamageCell(int x, int y, int z, int damage)
    {
        int cell = this.GetCellItem(x, y, z);
        if (this.ValidCells.Contains(cell))
        {
            this.SetCellItem(x, y, z, cell + damage);
        }

        return this.IsValid(new Vector3Int(x, y, z));
    }
}
