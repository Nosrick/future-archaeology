using System;
using System.Collections.Generic;
using System.Linq;
using DiggyDig.scripts.utils;
using Godot;
using Array = Godot.Collections.Array;

namespace DiggyDig.scripts.digging
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
        
        protected PackedScene DiggingObjectScene { get; set; }

        protected List<DigItem> DigItems;

        protected Random Random;
    
        public override void _Ready()
        {
            this.Random = new Random();
            this.DiggingObjectScene = GD.Load<PackedScene>("scenes/game/DiggingObject.tscn");
            this.DigItems = new List<DigItem>();
            
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
                        this.SetCellItem(x, y, z, FULL_CUBE);
                    }
                }
            }
            
            this.PlaceObjects();
        }

        protected void PlaceObjects()
        {
            int numObjects = 5;

            for (int i = 0; i < numObjects; i++)
            {
                DigItem item = this.DiggingObjectScene.Instance<DigItem>();

                item.Translation = this.RandomPosition();
                item.RotationDegrees = this.RandomRotation();

                this.DigItems.Add(item);
                this.AddChild(item);

                bool loopBreak = false;
                while (!loopBreak)
                {
                    foreach (var obj in item.GetCollidingBodies())
                    {
                        if (obj is DigItem otherItem)
                        {
                            item.Translation = this.RandomPosition();
                            item.RotationDegrees = this.RandomRotation();
                            break;
                        }
                    }

                    loopBreak = true;
                }

                //item.Connect("body_exited", this, nameof(this.RemoveObject), new Array { item });
            }
        }

        public bool RemoveObject(DigItem removed)
        {
            if (this.DigItems.Contains(removed))
            {
                Array collisionObjects = removed.GetCollidingBodies();
                if (collisionObjects.Contains(this) == false)
                {
                    GlobalConstants.GameManager.Cash += removed.CashValue;
                    this.RemoveChild(removed);
                    return true;
                }
            }

            return false;
        }

        protected Vector3 RandomPosition()
        {
            int x = this.Random.Next(-this.Width + 1, this.Width);
            int y = this.Random.Next(-this.Height + 1, this.Height);
            int z = this.Random.Next(-this.Depth + 1, this.Depth);
            return new Vector3(x, y, z);
        }

        protected Vector3 RandomRotation()
        {
            float xRot = GD.Randf() * 360;
            float yRot = GD.Randf() * 360;
            float zRot = GD.Randf() * 360;
            return new Vector3(xRot, yRot, zRot);
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
