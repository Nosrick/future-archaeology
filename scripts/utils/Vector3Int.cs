using System;
using Godot;

namespace ATimeGoneBy.scripts.utils
{
    [Serializable]
    public struct Vector3Int
    {
        public int x, y, z;

        public static Vector3Int Left => new(-1, 0, 0);
        public static Vector3Int Right => new(1, 0, 0);
        public static Vector3Int Up => new(0, 1, 0);
        public static Vector3Int Down => new(0, -1, 0);
        public static Vector3Int Forward => new(0, 0, -1);
        public static Vector3Int Back => new(0, 0, 1);
        public static Vector3Int Zero => new(0, 0, 0);
        public static Vector3Int Unit => new(1, 1, 1);
        public static Vector3Int NegativeUnit => new(-1, -1, -1); 

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

        public Vector3Int(Vector3Int copy)
        {
            this.x = copy.x;
            this.y = copy.y;
            this.z = copy.z;
        }

        public override string ToString()
        {
            return "{ " + this.x + ", " + this.y + ", " + this.z + " }";
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.x, this.y, this.z);
        }

        public static Vector3Int operator -(Vector3Int left, Vector3Int right)
        {
            return new Vector3Int(left.x - right.x, left.y - right.y, left.z - right.z);
        }

        public static Vector3Int operator +(Vector3Int left, Vector3Int right)
        {
            return new Vector3Int(left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static Vector3Int operator *(Vector3Int left, Vector3Int right)
        {
            return new Vector3Int(left.x * right.x, left.y * right.y, left.z * right.z);
        }

        public static Vector3Int operator *(Vector3Int left, int right)
        {
            return new Vector3Int(left.x * right, left.y * right, left.z * right);
        }

        public static bool operator ==(Vector3Int left, Vector3Int right)
        {
            if (left.x != right.x
                || left.y != right.y
                || left.z != right.z)
            {
                return false;
            }
            
            return true;
        }

        public static bool operator !=(Vector3Int left, Vector3Int right)
        {
            if (left.x != right.x 
                || left.y != right.y 
                || left.z != right.z)
            {
                return true;
            }
            
            return false;
        }

        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => this.x,
                    1 => this.y,
                    2 => this.z,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }
        
        public bool Equals(Vector3Int other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ z;
                return hashCode;
            }
        }

        public enum Axis
        {
            X = 0,
            Y = 1,
            Z = 2
        }
    }
}