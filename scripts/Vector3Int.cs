using Godot;

namespace DiggyDig.scripts
{
    public struct Vector3Int
    {
        public int x, y, z;

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Int(Vector3 vec)
        {
            this.x = (int) vec.x;
            this.y = (int) vec.y;
            this.z = (int) vec.z;
        }

        public override string ToString()
        {
            return this.x + ", " + this.y + ", " + this.z;
        }
    }
}