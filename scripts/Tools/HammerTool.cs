using Godot;

namespace DiggyDig.scripts
{
    public class HammerTool : ITool
    {
        public string Name => "Hammer";
        public int Cost => 40;

        public int Execute(Vector3Int t)
        {
            int xFlip = 1;
            int yFlip = 1;
            int zFlip = 1;

            if (t.x < 0)
            {
                xFlip = -1;
            }

            if (t.y < 0)
            {
                yFlip = -1;
            }

            if (t.z < 0)
            {
                zFlip = -1;
            }

            GridMap digSite = GlobalConstants.GameManager.DiggingSpace;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        int cell = digSite.GetCellItem(t.x + x, t.y + y, t.z + z);
                        if (cell >= 0)
                        {
                            digSite.SetCellItem(t.x + x, t.y + y, t.z + z, cell + 1);
                        }
                    }
                }
            }

            return this.Cost;
        }
    }
}