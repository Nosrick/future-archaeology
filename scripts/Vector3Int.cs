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

        public Vector3Int(float x, float y, float z)
        {
            this.x = Mathf.FloorToInt(x);
            this.y = Mathf.FloorToInt(y);
            this.z = Mathf.FloorToInt(z);
        }

        public Vector3Int(Vector3 vec)
        {
            this.x = Mathf.FloorToInt(vec.x);
            this.y = Mathf.FloorToInt(vec.y);
            this.z = Mathf.FloorToInt(vec.z);
        }

        public override string ToString()
        {
            return this.x + ", " + this.y + ", " + this.z;
        }
    }
}