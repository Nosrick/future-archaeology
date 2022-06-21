using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace ATimeGoneBy.scripts.digging
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

        protected AudioStreamPlayer3D ToolAudioPlayer { get; set; }
        protected AudioStreamPlayer3D PickupAudioPlayer { get; set; }
        protected Timer Timer { get; set; }

        protected AudioStreamRandomPitch ItemPickUpSound { get; set; }

        public override void _Ready()
        {
            this.SetPhysicsProcess(false);
            this.Random = new Random();
            this.DiggingObjectScene = GD.Load<PackedScene>("scenes/game/DiggingObject.tscn");
            this.DigItems = new List<DigItem>();

            this.ItemPickUpSound = new AudioStreamRandomPitch();
            this.ItemPickUpSound.AudioStream = GD.Load<AudioStream>("assets/sounds/money-get.wav");
            this.ItemPickUpSound.RandomPitch = 1.1f;

            this.ToolAudioPlayer = this.GetNode<AudioStreamPlayer3D>("ToolSounds");
            this.PickupAudioPlayer = this.GetNode<AudioStreamPlayer3D>("PickupSounds");
            this.Timer = this.GetNode<Timer>("Timer");

            this.PickupAudioPlayer.Stream = this.ItemPickUpSound;

            this.ValidCells = this.MeshLibrary.GetItemList();
            this.Width = 5;
            this.Height = 5;
            this.Depth = 5;

            this.GenerateDigSite();
        }

        public void GenerateDigSite()
        {
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

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            this.CheckForUncovered();
        }

        protected async void BeginProcessing()
        {
            this.Timer.Start(1f);
            await this.ToSignal(this.Timer, "timeout");

            this.PlaceObjects();

            bool loop = true;
            while (loop)
            {
                loop = false;
                foreach (DigItem item in this.DigItems)
                {
                    if (!item.IsInsideTree())
                    {
                        loop = true;
                    }
                }

                if (loop)
                {
                    this.Timer.Start(1f);
                    await this.ToSignal(this.Timer, "timeout");
                }
            }

            this.Timer.Start(1f);
            await this.ToSignal(this.Timer, "timeout");

            this.SetPhysicsProcess(true);
        }

        protected async void PlaceObjects()
        {
            int numObjects = 5;

            for (int i = 0; i < numObjects; i++)
            {
                DigItem item = this.DiggingObjectScene.Instance<DigItem>();

                item.Translation = this.RandomPosition();
                item.RotationDegrees = this.RandomRotation();

                this.DigItems.Add(item);
                this.AddChild(item);

                var signals = item.GetSignalList();
                GD.Print(item.IsInsideTree());
                
                item.AssignObject(GlobalConstants.GameManager.Items.GetRandom(), 100);

                bool loopBreak = false;
                int tries = 0;
                while (!loopBreak && tries < 100)
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

                    tries += 1;

                    loopBreak = true;
                }
            }
        }

        public void CheckForUncovered()
        {
            foreach (DigItem item in this.DigItems)
            {
                var collidingBodies = item.GetCollidingBodies();
                if (item.Uncovered || collidingBodies.Contains(this))
                {
                    continue;
                }

                item.MarkMeUncovered();
                item.MakeMeGlow();
            }
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
                    this.DigItems.Remove(removed);
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
            this.RemoveChild(removal);
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
                var sound = GlobalConstants.GameManager.CurrentTool?.AssociatedSound;
                if (this.ToolAudioPlayer.Stream != sound)
                {
                    this.ToolAudioPlayer.Stream = sound;
                }

                this.ToolAudioPlayer.Play();
            }

            return this.IsValid(new Vector3Int(x, y, z));
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