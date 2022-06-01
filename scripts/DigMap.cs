using Godot;
using System;

public class DigMap : GridMap
{
    protected int Width { get; set; }
    protected int Height { get; set; }
    protected int Depth { get; set; }

    protected int[] MeshList;
    protected const int FULL_CUBE = 0;
    protected const int HALF_CUBE = 1;
    protected const int BARE_CUBE = 2;
    
    public override void _Ready()
    {
        this.MeshList = this.MeshLibrary.GetItemList();
        this.Width = 5;
        this.Height = 5;
        this.Depth = 5;
        
        for (int x = -this.Width; x <= this.Width; x++)
        {
            for (int y = -this.Height; y <= this.Height; y++)
            {
                for (int z = -this.Depth; z <= this.Depth; z++)
                {
                    this.SetCellItem(x, y, z, this.MeshList[0]);
                }
            }
        }
    }
}
