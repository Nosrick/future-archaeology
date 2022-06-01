using Godot;
using System;
using DiggyDig.scripts;

public class DigPicker : RayCast
{
    protected const float RayLength = 1000f;

    [Export] protected NodePath CameraPath;
    [Export] protected NodePath GridPath;

    protected Camera MyCamera { get; set; }
    protected GridMap DigMap { get; set; }
    
    protected Vector2 MousePosition { get; set; }
    protected bool CastRay { get; set; }
    
    public override void _Ready()
    {
        if (this.CameraPath is null)
        {
            GD.Print("CAMERA PATH IS NULL");
            return;
        }

        if (this.GridPath is null)
        {
            GD.Print("GRID PATH IS NULL");
            return;
        }

        this.MyCamera = this.GetNodeOrNull<Camera>(this.CameraPath);
        this.DigMap = this.GetNodeOrNull<GridMap>(this.GridPath);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton mouseButton
            && mouseButton.Pressed
            && mouseButton.ButtonIndex == (int) ButtonList.Left)
        {
            this.MousePosition = mouseButton.Position;
            this.CastRay = true;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (this.CastRay)
        {
            var origin = this.MyCamera?.ProjectRayOrigin(this.MousePosition);
            var direction = origin + this.MyCamera?.ProjectRayNormal(this.MousePosition) * RayLength;

            if (!origin.HasValue || !direction.HasValue)
            {
                this.CastRay = false;
                return;
            }

            var spaceState = this.GetWorld().DirectSpaceState;
            this.ForceRaycastUpdate();
            GD.Print("CLICK");

            if (this.IsColliding())
            {
                var iRay = spaceState.IntersectRay(origin.Value, direction.Value);
                if (iRay.Contains("position"))
                {
                    Vector3 point = (Vector3) iRay["position"];
                    Vector3Int p = new Vector3Int(point.x, point.y, point.z);
                    GD.Print(point);
                    GD.Print(p);

                    var cell = this.DigMap?.GetCellItem(p.x, p.y, p.z);
                    if (cell >= 0)
                    {
                        this.DigMap?.SetCellItem(p.x, p.y, p.z, cell.Value + 1);
                    }
                }
            }

            this.CastRay = false;
        }
    }
}
