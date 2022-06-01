using Godot;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public class OrbitCamera : Camera
{
    [Export] protected NodePath OrbitTargetPath;

    [Export(PropertyHint.Range)] protected float MoveSensitivity = 0.1f;

    protected Spatial OrbitTarget;

    protected Vector2 MoveSpeed;
    protected float Distance = 20f;
    protected Vector3 Rotation;

    protected const float RADIAN = Mathf.Pi / 2;

    protected bool Moving { get; set; }

    public override void _Ready()
    {
        Node target = this.GetNode(this.OrbitTargetPath);

        this.Rotation = this.Transform.basis.GetEuler();
        
        if (target is Spatial spatial)
        {
            this.OrbitTarget = spatial;
            this.SetRotation();
        }
        else
        {
            GD.PrintErr("OrbitTargetPath is not of type Spatial!");
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!this.Moving && Input.IsMouseButtonPressed((int) ButtonList.Right))
        {
            this.Moving = true;
        }
        else if (this.Moving)
        {
            this.Moving = false;
        }

        if (this.Moving)
        {
            this.Rotation.x -= this.MoveSensitivity * this.MoveSpeed.y * delta;
            this.Rotation.y -= this.MoveSensitivity * this.MoveSpeed.x * delta;
            
            if (this.Rotation.x < -RADIAN)
            {
                this.Rotation.x = -RADIAN;
            }
            else if (this.Rotation.x > RADIAN)
            {
                this.Rotation.x = RADIAN;
            }
            
            this.MoveSpeed = Vector2.Zero;

            /*
            this.Distance += this.MoveSensitivity * delta;

            if (this.Distance < 0)
            {
                this.Distance = 0;
            }

            this.TranslateObjectLocal(new Vector3(0, 0, this.Distance));
            */
            this.SetRotation();
        }
    }

    protected void SetRotation()
    {
        Quat t = new Quat(this.Rotation);
        Transform orbitTargetTransform = this.OrbitTarget.Transform;
        orbitTargetTransform.basis = new Basis(t);
        this.OrbitTarget.Transform = orbitTargetTransform;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (this.Moving && @event is InputEventMouseMotion mouseMotion)
        {
            this.MoveSpeed = mouseMotion.Relative;
        }
    }
}