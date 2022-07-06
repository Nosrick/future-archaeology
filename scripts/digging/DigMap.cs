using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Timer = Godot.Timer;

namespace ATimeGoneBy.scripts.digging
{
    public class DigMap : GridMap
    {
        protected int Width { get; set; }
        protected int Height { get; set; }
        protected int Depth { get; set; }

        protected int ObjectsToGenerate { get; set; }

        public AABB Area { get; protected set; }

        public bool LevelTouched => this.DigItems.Count != this.ObjectsToGenerate;

        public int[] ValidCells { get; protected set; }
        public const int BARE_CUBE = 0;
        public const int HALF_CUBE = 1;
        public const int FULL_CUBE = 2;

        public const int FLASH_MODIFIER = 3;
        public const int EMPTY_CELL = -1;

        protected PackedScene DiggingObjectScene { get; set; }

        protected List<DigItem> DigItems;

        protected Random Random;
        protected AudioStreamPlayer3D PickupAudioPlayer { get; set; }
        protected Timer Timer { get; set; }

        protected AudioStreamRandomPitch ItemPickUpSound { get; set; }

        protected ShaderMaterial FlashMaterial;

        protected Queue<Vector3Int> AreaQueue;
        protected bool Flashing;

        [Export] protected int PointsPerFrame = 500;

        public override void _Ready()
        {
            this.SetProcess(false);
            this.SetPhysicsProcess(false);
            this.Random = new Random();
            this.AreaQueue = new Queue<Vector3Int>();
            this.DiggingObjectScene = GD.Load<PackedScene>("scenes/game/DiggingObject.tscn");
            this.DigItems = new List<DigItem>();

            this.ItemPickUpSound = new AudioStreamRandomPitch();
            this.ItemPickUpSound.AudioStream = GD.Load<AudioStream>("assets/sounds/money-get.wav");
            this.ItemPickUpSound.RandomPitch = 1.1f;

            this.PickupAudioPlayer = this.GetNode<AudioStreamPlayer3D>("PickupSounds");
            this.Timer = this.GetNode<Timer>("Timer");

            this.FlashMaterial = GD.Load<ShaderMaterial>("assets/shaders/flash-material.tres");

            this.PickupAudioPlayer.Stream = this.ItemPickUpSound;

            this.ValidCells = this.MeshLibrary.GetItemList();
            this.Width = 5;
            this.Height = 5;
            this.Depth = 5;
        }

        public void GenerateDigSite(Vector3Int dimensions, int numObjects = 5)
        {
            this.Width = dimensions.x;
            this.Height = dimensions.y;
            this.Depth = dimensions.z;

            this.ObjectsToGenerate = numObjects;

            this.Area = new AABB
            {
                Position = new Vector3(-this.Width, -this.Height, -this.Depth),
                End = new Vector3(this.Width, this.Height, this.Depth)
            };

            this.SetProcess(false);
            this.SetPhysicsProcess(false);
            this.Clear();

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

            this.BeginProcessing();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (this.AreaQueue.Any())
            {
                for (int i = 0; i < this.PointsPerFrame; i++)
                {
                    if (this.AreaQueue.Any() == false)
                    {
                        break;
                    }

                    Vector3Int point = this.AreaQueue.Dequeue();
                    if (this.Flashing)
                    {
                        this.MakeCellFlash(point.x, point.y, point.z);
                    }
                    else
                    {
                        this.EndCellFlash(point.x, point.y, point.z);
                    }
                }
            }
        }

        protected async void BeginProcessing()
        {
            await this.ToSignal(this.GetTree(), "idle_frame");

            this.CreateObjects();

            foreach (DigItem item in this.DigItems)
            {
                if (!item.IsInsideTree())
                {
                    await this.ToSignal(item, "ready");
                }
            }

            this.PlaceObjects();

            await this.ToSignal(this.GetTree(), "idle_frame");

            this.RemoveOccludedTiles();

            this.SetProcess(true);
            this.SetPhysicsProcess(true);
        }

        protected void CreateObjects()
        {
            for (int i = 0; i < this.ObjectsToGenerate; i++)
            {
                DigItem item = this.DiggingObjectScene.Instance<DigItem>();

                item.Translation = this.RandomPosition();
                item.RotationDegrees = this.RandomRotation();

                this.DigItems.Add(item);
                this.AddChild(item);

                item.AssignObject(GlobalConstants.GameManager.Items.GetRandom(), 100);
            }
        }

        protected void PlaceObjects()
        {
            foreach (DigItem item in this.DigItems)
            {
                bool loopBreak = false;
                int tries = 0;
                while (!loopBreak && tries < 20)
                {
                    foreach (var obj in item.GetCollidingBodies())
                    {
                        if (obj is DigItem)
                        {
                            item.Translation = this.RandomPosition();
                            item.RotationDegrees = this.RandomRotation();
                            break;
                        }
                    }

                    tries += 1;

                    loopBreak = true;
                }
            }
        }

        protected void RemoveOccludedTiles()
        {
            foreach (DigItem item in this.DigItems)
            {
                AABB aabb = item.ObjectMesh.GetTransformedAabb();
                Vector3Int begin = new Vector3Int(aabb.Position);
                Vector3Int end = new Vector3Int(aabb.End);

                for (int x = begin.x + 1; x < end.x - 1; x++)
                {
                    for (int y = begin.y + 1; y < end.y - 1; y++)
                    {
                        for (int z = begin.z + 1; z < end.z - 1; z++)
                        {
                            this.SetCellItem(x, y, z, EMPTY_CELL);
                        }
                    }
                }
            }
        }

        public void CheckForUncovered(AABB area)
        {
            foreach (DigItem item in this.DigItems)
            {
                if (area.Intersects(item.ObjectMesh.GetTransformedAabb()))
                {
                    this.CheckObject(item);
                }
            }
        }

        public bool IsAnyUncovered()
        {
            foreach (DigItem item in this.DigItems)
            {
                if (item.Uncovered)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckObject(DigItem item)
        {
            if (item.GetCollidingBodies().Contains(this))
            {
                if (item.Uncovered)
                {
                    item.MarkMeCovered();
                }

                return false;
            }

            item.MarkMeUncovered();
            return true;
        }

        public bool RemoveObject(DigItem removed)
        {
            if (this.DigItems.Contains(removed))
            {
                Array collisionObjects = removed.GetCollidingBodies();
                if (collisionObjects.Contains(this) == false)
                {
                    removed.PlayPickupAnimation();
                    GlobalConstants.GameManager.Cash += removed.CashValue;
                    this.PickupAudioPlayer.Play();
                    SceneTreeTimer timer = this.GetTree().CreateTimer(0.25f);
                    timer.Connect("timeout", this, nameof(this.DelayedRemoval), new Array {removed});
                    return true;
                }
            }

            return false;
        }

        public bool LevelComplete()
        {
            if (this.IsPhysicsProcessing() == false)
            {
                return false;
            }

            return !this.DigItems.Any();
        }

        protected void DelayedRemoval(Node removal)
        {
            this.DigItems.Remove(removal as DigItem);
            removal.QueueFree();
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

        public bool IsObjectAt(Vector3Int pos)
        {
            Vector3 point = pos.ToVector3();
            foreach (DigItem item in this.DigItems)
            {
                AABB aabb = item.ObjectMesh.GetTransformedAabb();
                if (aabb.HasPoint(point))
                {
                    return true;
                }
            }

            return false;
        }

        public DigItem GetObjectAt(Vector3Int pos)
        {
            Vector3 point = pos.ToVector3();
            foreach (DigItem item in this.DigItems)
            {
                AABB aabb = item.ObjectMesh.GetTransformedAabb();
                if (aabb.HasPoint(point))
                {
                    return item;
                }
            }

            return null;
        }

        public bool IsValid(Vector3Int pos)
        {
            return this.IsValid(pos.x, pos.y, pos.z);
        }

        public bool IsValid(int x, int y, int z)
        {
            return this.ValidCells.Contains(this.GetCellItem(x, y, z));
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
                if (cell >= FLASH_MODIFIER
                    && cell - damage < FLASH_MODIFIER)
                {
                    cell = EMPTY_CELL;
                }
                else
                {
                    cell = Mathf.Max(cell - damage, EMPTY_CELL);
                }

                this.SetCellItem(x, y, z, cell);
            }

            bool valid = this.IsValid(new Vector3Int(x, y, z));
            if (!valid)
            {
                GlobalConstants.GameManager.TickCooldowns();
            }

            return valid;
        }

        public void MakeCellFlash(Vector3Int pos, Vector3Int.Axis axis, int axisDir)
        {
            this.MakeCellFlash(pos.x, pos.y, pos.z, axis, axisDir);
        }

        public async void MakeAreaFlash(
            Vector3Int start,
            Vector3Int end,
            Vector3Int.Axis axis,
            int axisDir,
            int lengthInUnits,
            float duration,
            bool autoEnd = true,
            bool includeItems = false,
            bool stopItemFlashing = false)
        {
            this.SetShaderParams(axis, axisDir, lengthInUnits, duration);

            this.Flashing = true;

            Vector3Int layerEnd = new Vector3Int();

            switch (axis)
            {
                case Vector3Int.Axis.X:
                    layerEnd = new Vector3Int(start.x, end.y, end.z);
                    break;

                case Vector3Int.Axis.Y:
                    layerEnd = new Vector3Int(end.x, start.y, end.z);
                    break;

                case Vector3Int.Axis.Z:
                    layerEnd = new Vector3Int(end.x, end.y, start.z);
                    break;
            }

            this.QueueAreaForProcessing(start, layerEnd, axis, axisDir, lengthInUnits, includeItems);

            if (includeItems)
            {
                AABB box = new AABB
                {
                    Position = start.ToVector3(),
                    End = end.ToVector3()
                };

                var items = this.DigItems.Where(item => box.Intersects(item.ObjectMesh.GetTransformedAabb(), true));
                foreach (DigItem item in items)
                {
                    item.MakeMeFlash();
                }
            }

            if (autoEnd)
            {
                this.Timer.Start(duration);
                await this.ToSignal(this.Timer, "timeout");

                this.Flashing = false;
                this.QueueAreaForProcessing(start, end, axis, axisDir, lengthInUnits, stopItemFlashing);
            }
        }

        protected void QueueAreaForProcessing(
            Vector3Int start,
            Vector3Int end,
            Vector3Int.Axis axis,
            int stepDir,
            int remaining,
            bool includeItems = false)
        {
            while (true)
            {
                for (int x = start.x; x <= end.x; x++)
                {
                    for (int y = start.y; y <= end.y; y++)
                    {
                        for (int z = start.z; z <= end.z; z++)
                        {
                            this.AreaQueue.Enqueue(new Vector3Int(x, y, z));
                        }
                    }
                }

                if (remaining == 0)
                {
                    return;
                }

                Vector3Int nextStart = start;
                Vector3Int nextEnd = end;

                switch (axis)
                {
                    case Vector3Int.Axis.X:
                        nextStart += (Vector3Int.Right);
                        nextEnd += (Vector3Int.Right);
                        break;

                    case Vector3Int.Axis.Y:
                        nextStart += (Vector3Int.Up);
                        nextEnd += (Vector3Int.Up);
                        break;

                    case Vector3Int.Axis.Z:
                        nextStart += (Vector3Int.Back);
                        nextEnd += (Vector3Int.Back);
                        break;
                    default:
                        return;
                }

                start = nextStart;
                end = nextEnd;
                remaining -= 1;
            }
        }

        public void EndAreaFlash(
            Vector3Int begin,
            Vector3Int end,
            bool includeItems = false)
        {
            for (int x = begin.x; x <= end.x; x++)
            {
                for (int y = begin.y; y <= end.y; y++)
                {
                    for (int z = begin.z; z <= end.z; z++)
                    {
                        this.EndCellFlash(x, y, z);
                    }
                }
            }

            if (includeItems)
            {
                AABB box = new AABB();
                box.Position = begin.ToVector3();
                box.End = end.ToVector3();

                var items = this.DigItems.Where(
                    item => item.Flashing
                            && box.Intersects(item.ObjectMesh.GetTransformedAabb(), true));

                foreach (DigItem item in items)
                {
                    item.EndMyFlash();
                }
            }
        }

        protected void SetShaderParams(
            Vector3Int.Axis axisIndex,
            int axisDir,
            int lengthInUnits = 1,
            float duration = 0.5f)
        {
            this.FlashMaterial.SetShaderParam("shineLengthInUnits", lengthInUnits);
            this.FlashMaterial.SetShaderParam("durationInSeconds", duration);
            this.FlashMaterial.SetShaderParam("axisIndex", (int) axisIndex);
            this.FlashMaterial.SetShaderParam("axisDir", axisDir);
            this.FlashMaterial.SetShaderParam("startTime", OS.GetTicksMsec());
        }

        protected void MakeCellFlash(int x, int y, int z)
        {
            int cell = this.GetCellItem(x, y, z);
            if (cell >= FLASH_MODIFIER || !this.IsValid(x, y, z))
            {
                return;
            }

            this.SetCellItem(x, y, z, cell + FLASH_MODIFIER);
        }

        public void MakeCellFlash(int x, int y, int z, Vector3Int.Axis axisIndex, int axisDir)
        {
            this.SetShaderParams(axisIndex, axisDir);
            this.MakeCellFlash(x, y, z);
        }

        public void EndCellFlash(Vector3Int pos)
        {
            this.EndCellFlash(pos.x, pos.y, pos.z);
        }

        public void EndCellFlash(int x, int y, int z)
        {
            int cell = this.GetCellItem(x, y, z);
            if (cell < FLASH_MODIFIER || !this.IsValid(x, y, z))
            {
                return;
            }

            this.SetCellItem(x, y, z, cell - FLASH_MODIFIER);
        }

        public Dictionary Save()
        {
            Dictionary saveDict = new Dictionary();

            Dictionary tiles = new Dictionary();
            foreach (Vector3 t in this.GetUsedCells())
            {
                Vector3Int tile = new Vector3Int(t);
                int result = this.GetCellItem(tile.x, tile.y, tile.z);

                tiles.Add(t, result);
            }

            saveDict.Add("tiles", tiles);

            Array objects = new Array();
            foreach (DigItem item in this.DigItems)
            {
                objects.Add(item.Save());
            }

            saveDict.Add("objects", objects);

            return saveDict;
        }

        public bool Load(Dictionary data)
        {
            if (!data.Contains("tiles"))
            {
                return false;
            }

            this.Clear();

            Dictionary tiles = data["tiles"] as Dictionary;
            foreach (DictionaryEntry tile in tiles)
            {
                Vector3Int pos = new Vector3Int((Vector3) tile.Key);
                int cell = (int) tile.Value;

                this.SetCellItem(pos.x, pos.y, pos.z, cell);
            }

            if (!data.Contains("objects"))
            {
                return false;
            }

            foreach (Node node in this.GetChildren())
            {
                if (node is DigItem digItem)
                {
                    this.DigItems.Remove(digItem);
                    digItem.QueueFree();
                }
            }

            Array objects = (Array) data["objects"];
            PackedScene digItemPackedScene = GD.Load<PackedScene>(GlobalConstants.DigItemLocation);
            foreach (Dictionary itemDict in objects)
            {
                DigItem digItem = digItemPackedScene.Instance<DigItem>();
                digItem.Load(itemDict);
                this.DigItems.Add(digItem);
                this.AddChild(digItem);
            }

            return true;
        }
    }
}