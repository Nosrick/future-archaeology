using Godot;

namespace DiggyDig.scripts.Tools
{
    public class HammerTool : ITool
    {
        public string Name => "Hammer";
        public int Cost => 40;

        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            DigMap digSite = GlobalConstants.GameManager.DiggingSpace;

            Vector3Int dir = hit - previous;

            Vector3.Axis hitAxis = Vector3.Axis.X;

            if (dir.y != 0)
            {
                hitAxis = Vector3.Axis.Y;
            }
            else if (dir.z != 0)
            {
                hitAxis = Vector3.Axis.Z;
            }

            int xStep = 0;
            int yStep = 0;
            int zStep = 0;

            if (hitAxis == Vector3.Axis.X)
            {
                yStep = 1;
                zStep = 1;

                int x = hit.x;

                for (int y = hit.y - yStep; y <= hit.y + yStep; y += yStep)
                {
                    for (int z = hit.z - zStep; z <= hit.z + zStep; z += zStep)
                    {
                        digSite.DamageCell(x, y, z, 1);
                    }
                }
            }
            else if (hitAxis == Vector3.Axis.Y)
            {
                xStep = 1;
                zStep = 1;

                int y = hit.y;

                for (int x = hit.x - xStep; x <= hit.x + xStep; x += xStep)
                {
                    for (int z = hit.z - zStep; z <= hit.z + zStep; z += zStep)
                    {
                        digSite.DamageCell(x, y, z, 1);
                    }
                }
            }
            else
            {
                xStep = 1;
                yStep = 1;

                int z = hit.z;

                for (int x = hit.x - xStep; x <= hit.x + xStep; x += xStep)
                {
                    for (int y = hit.y - yStep; y <= hit.y + yStep; y += yStep)
                    {
                        digSite.DamageCell(x, y, z, 1);
                    }
                }
            }

            return this.Cost;
        }
    }
}