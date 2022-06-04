using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace DiggyDig.scripts
{
    public class BombTool : ITool
    {
        public string Name => "Bomb";
        public int Cost => 75;
        
        protected const int Tiles = 20;
        
        public int Execute(Vector3Int hit, Vector3Int previous)
        {
            GridMap digSite = GlobalConstants.GameManager.DiggingSpace;

            Array usedCells = digSite.GetUsedCells();
            List<Vector3> outerTiles = new List<Vector3>();
            foreach (Vector3 cell in usedCells)
            {
            }
            
            GD.Print("BOOM!");

            return this.Cost;
        }
    }
}