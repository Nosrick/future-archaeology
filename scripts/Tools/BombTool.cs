using System;
using System.Collections.Generic;
using System.Linq;
using DiggyDig.scripts.digging;
using DiggyDig.scripts.utils;
using Godot;
using Array = Godot.Collections.Array;

namespace DiggyDig.scripts.Tools
{
    public class BombTool : ITool
    {
        public string Name => "Bomb";
        public int Cost => 75;
        
        protected const int Tiles = 20;

        protected const int Damage = 3;
        public AudioStreamRandomPitch AssociatedSound { get; protected set; }

        protected static readonly Random Random = new Random();

        public BombTool()
        {
            this.AssociatedSound = new AudioStreamRandomPitch();
            this.AssociatedSound.AudioStream = GD.Load<AudioStream>("assets/sounds/bomb-1.wav");
            this.AssociatedSound.RandomPitch = 1.2f;
        }
        
        public int Execute(Vector3Int hit, Vector3Int previous)
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

                return this.Cost;
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

            return this.Cost;
        }
    }
}