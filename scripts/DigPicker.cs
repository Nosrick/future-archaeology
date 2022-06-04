using Godot;
using System;
using DiggyDig.scripts;

public class DigPicker : RayCast
{
    protected const float RayLength = 60f;

    [Export] protected NodePath CameraPath;
    [Export] protected NodePath GridPath;

    protected Camera MyCamera { get; set; }
    protected DigMap DigMap { get; set; }

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
        this.DigMap = this.GetNodeOrNull<DigMap>(this.GridPath);
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
            Vector3 origin = this.MyCamera.ProjectRayOrigin(this.MousePosition);
            Vector3 direction = this.MyCamera.ProjectRayNormal(this.MousePosition).Normalized();

            Vector3Int current = new Vector3Int(origin);
            Vector3Int previous = new Vector3Int(origin);

            int xiStep = Mathf.Sign(direction.x);
            int yiStep = Mathf.Sign(direction.y);
            int ziStep = Mathf.Sign(direction.z);

            float deltaX = float.MaxValue;
            if (xiStep != 0)
            {
                deltaX = 1.0f / Mathf.Abs(direction.x);
            }

            float deltaY = float.MaxValue;
            if (yiStep != 0)
            {
                deltaY = 1.0f / Mathf.Abs(direction.y);
            }

            float deltaZ = float.MaxValue;
            if (ziStep != 0)
            {
                deltaZ = 1.0f / Mathf.Abs(direction.z);
            }

            float crossX = float.MaxValue;
            float crossY = float.MaxValue;
            float crossZ = float.MaxValue;

            if (xiStep == 1)
            {
                crossX = (Mathf.Ceil(origin.x) - origin.x) * deltaX;
            }
            else if (xiStep != 0)
            {
                crossX = (origin.x - Mathf.Floor(origin.x)) * deltaX;
            }

            if (yiStep == 1)
            {
                crossY = (Mathf.Ceil(origin.y) - origin.y) * deltaY;
            }
            else if (yiStep != 0)
            {
                crossY = (origin.y - Mathf.Floor(origin.y)) * deltaY;
            }

            if (ziStep == 1)
            {
                crossZ = (Mathf.Ceil(origin.z) - origin.z) * deltaZ;
            }
            else if (ziStep != 0)
            {
                crossZ = (origin.z - Mathf.Floor(origin.z)) * deltaZ;
            }

            if (crossX == 0.0f)
            {
                crossX += deltaX;
                if (xiStep == -1)
                {
                    current.x -= 1;
                }
            }

            if (crossY == 0.0f)
            {
                crossY += deltaY;
                if (yiStep == -1)
                {
                    current.y -= 1;
                }
            }

            if (crossZ == 0.0f)
            {
                crossZ += deltaZ;
                if (ziStep == -1)
                {
                    current.z -= 1;
                }
            }

            while (true)
            {
                previous = new Vector3Int(current);

                if (crossX < crossY)
                {
                    if (crossX < crossZ)
                    {
                        current.x += xiStep;
                        if (crossX > RayLength)
                        {
                            this.CastRay = false;
                            return;
                        }

                        crossX += deltaX;
                    }
                    else
                    {
                        current.z += ziStep;
                        if (crossZ > RayLength)
                        {
                            this.CastRay = false;
                            return;
                        }

                        crossZ += deltaZ;
                    }
                }
                else
                {
                    if (crossY < crossZ)
                    {
                        current.y += yiStep;
                        if (crossY > RayLength)
                        {
                            this.CastRay = false;
                            return;
                        }

                        crossY += deltaY;
                    }
                    else
                    {
                        current.z += ziStep;
                        if (crossZ > RayLength)
                        {
                            this.CastRay = false;
                            return;
                        }

                        crossZ += deltaZ;
                    }
                }

                if (this.DigMap.IsValid(current))
                {
                    break;
                }
            }
            
            GD.Print(current);
            GD.Print(current - previous);

            GlobalConstants.GameManager.ExecuteTool(current, previous);

            this.CastRay = false;
        }
    }
}