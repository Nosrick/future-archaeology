using System.Linq;
using Godot;

namespace DiggyDig.scripts
{
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

        public bool IsOuterCell(Vector3Int pos)
        {
            for (int x = pos.x - 1; x <= pos.x + 1; x++)
            {
                for (int y = pos.y - 1; y <= pos.y + 1; y++)
                {
                    for (int z = pos.z - 1; z <= pos.z + 1; z++)
                    {
                        if (!this.IsValid(new Vector3Int(x, y, z)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsOuterCell(int x, int y, int z)
        {
            return this.IsOuterCell(new Vector3Int(x, y, z));
        }

        public bool DamageCell(Vector3Int pos, int damage)
        {
            return this.DamageCell(pos.x, pos.y, pos.z, damage);
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
}
