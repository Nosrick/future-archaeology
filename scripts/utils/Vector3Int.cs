using System;
using Godot;

namespace ATimeGoneBy.scripts.utils
{
    [Serializable]
    public struct Vector3Int
    {
        public int x, y, z;

        private static readonly Vector3Int _left = new(-1, 0, 0);
        private static readonly Vector3Int _right = new(1, 0, 0);

        private static readonly Vector3Int _up = new(0, 1, 0);
        private static readonly Vector3Int _down = new(0, -1, 0);

        private static readonly Vector3Int _forward = new(0, 0, -1);
        private static readonly Vector3Int _back = new(0, 0, 1);

        private static readonly Vector3Int _zero = new();
        private static readonly Vector3Int _unit = new(1, 1, 1);
        private static readonly Vector3Int _negUnit = new(-1, -1, -1);

        public static Vector3Int Left => _left;
        public static Vector3Int Right => _right;
        public static Vector3Int Up => _up;
        public static Vector3Int Down => _down;
        public static Vector3Int Forward => _forward;
        public static Vector3Int Back => _back;
        public static Vector3Int Zero => _zero;
        public static Vector3Int Unit => _unit;
        public static Vector3Int NegativeUnit => _negUnit; 

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

        public Vector3Int(int all)
        {
            this.x = all;
            this.y = all;
            this.z = all;
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

        public static Vector3Int operator /(Vector3Int left, int right)
        {
            return new Vector3Int(left.x / right, left.y / right, left.z / right);
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