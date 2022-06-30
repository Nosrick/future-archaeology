using System;
using System.Collections.Generic;
using System.Linq;
using ATimeGoneBy.scripts.digging;
using ATimeGoneBy.scripts.utils;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace ATimeGoneBy.scripts.tools
{
    public class BombTool : AbstractTool
    {
        public const string TILES_KEY = "tiles";
        
        public override string TranslationKey => "tools.bomb.name";

        protected int Tiles = 20;

        protected int Damage = 3;

        protected static readonly Random Random = new Random();

        public const int DEFAULT_COST = 75;
        public const int DEFAULT_COOLDOWN = 20;

        public BombTool()
        {
            this.Cost = DEFAULT_COST;
            this.UsageCooldown = DEFAULT_COOLDOWN;
            
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/bomb-1.wav");
            this.AssociatedSound.RandomPitch = 1.2f;
        }
        
        public override AABB Execute(Vector3Int hit, Vector3Int previous)
        {
            DigMap digSite = GlobalConstants.GameManager.DiggingSpace;

            Array usedCells = digSite.GetUsedCells();
            List<Vector3Int> outerTiles = new List<Vector3Int>();
            foreach (Vector3 cell in usedCells)
            {
                outerTiles.Add(new Vector3Int(cell));
            }

            outerTiles = outerTiles.Where(tile => digSite.IsValid(tile) && digSite.IsOuterCell(tile)).ToList();

            if (outerTiles.Count <= Tiles)
            {
                foreach (Vector3Int pos in outerTiles)
                {
                    digSite.DamageCell(pos.x, pos.y, pos.z, Damage);
                }

                return digSite.Area;
            }

            HashSet<int> tilesToHit = new HashSet<int>();

            while(tilesToHit.Count < Tiles)
            {
                int roll = Random.Next(outerTiles.Count);
                if (tilesToHit.Contains(roll))
                {
                    continue;
                }

                tilesToHit.Add(roll);
            }

            foreach (int index in tilesToHit)
            {
                Vector3Int tile = outerTiles[index];
                digSite.DamageCell(tile, Damage);
            }

            this.CooldownTimer = this.UsageCooldown;
            return digSite.Area;
        }

        public override Dictionary Save()
        {
            Dictionary saveDict = base.Save();
            
            saveDict.Add(TILES_KEY, this.Tiles);
            saveDict.Add(DAMAGE_KEY, this.Damage);

            return saveDict;
        }

        public override void Load(Dictionary data)
        {
            base.Load(data);

            this.Tiles = (int) data[TILES_KEY];
            this.Damage = (int) data[DAMAGE_KEY];
        }
    }
}