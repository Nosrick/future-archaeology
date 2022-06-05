using Godot;

namespace DiggyDig.scripts
{
    public class OrbitCamera : Camera
    {
        [Export] protected NodePath OrbitTargetPath;

        [Export] protected float RotationSensitivity = 0.1f;
        [Export] protected float PanSensitivity = 1f;
        [Export] protected float ZoomSensitivity = 1f;

        protected Spatial OrbitTarget;

        protected Vector2 MoveSpeed;
        protected Vector3 RotationDelta;
        protected Vector3 PanningDelta;
        protected float ZoomingDelta;

        protected const float RADIAN = Mathf.Pi / 2;

        protected bool Rotating { get; set; }
        protected bool Panning { get; set; }
        protected bool Zooming { get; set; }

        public override void _Ready()
        {
            Node target = this.GetNode(this.OrbitTargetPath);

            this.RotationDelta = this.Transform.basis.GetEuler();
            this.PanningDelta = Vector3.Zero;
        
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

            if (!this.Rotating && Input.IsMouseButtonPressed((int) ButtonList.Right))
            {
                this.Rotating = true;
            }
            else if (this.Rotating)
            {
                this.Rotating = false;
            }

            if (!this.Panning && Input.IsMouseButtonPressed((int) ButtonList.Middle))
            {
                this.Panning = true;
            }
            else if (this.Panning)
            {
                this.Panning = false;
            }

            if (this.Rotating)
            {
                this.RotationDelta.x += this.RotationSensitivity * this.MoveSpeed.y * delta;
                this.RotationDelta.y += this.RotationSensitivity * this.MoveSpeed.x * delta;
            
                /*
                if (this.RotationDelta.x < -RADIAN)
                {
                    this.RotationDelta.x = -RADIAN;
                }
                else if (this.RotationDelta.x > RADIAN)
                {
                    this.RotationDelta.x = RADIAN;
                }
                */
            
                this.MoveSpeed = Vector2.Zero;
                this.SetRotation();
            }
            else if (this.Panning)
            {
                this.PanningDelta.x += this.PanSensitivity * this.MoveSpeed.x * delta;
                this.PanningDelta.y -= this.PanSensitivity * this.MoveSpeed.y * delta;

                this.MoveSpeed = Vector2.Zero;
                this.OrbitTarget.TranslateObjectLocal(this.PanningDelta);
                
                this.LookAt(this.OrbitTarget.GlobalTransform.origin, this.GlobalTransform.basis.y);
                this.PanningDelta = Vector3.Zero;
            }
            else if (this.Zooming)
            {
                this.Translate(Vector3.Forward * this.ZoomingDelta);
            }

            if (this.Zooming)
            {
                this.Zooming = false;
                this.ZoomingDelta = 0;
            }
        }

        protected void SetRotation()
        {
            Quat t = new Quat(this.RotationDelta);
            Transform orbitTargetTransform = this.OrbitTarget.Transform;
            orbitTargetTransform.basis = new Basis(t);
            this.OrbitTarget.Transform = orbitTargetTransform;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if ((this.Rotating || this.Panning)
                && @event is InputEventMouseMotion mouseMotion)
            {
                this.MoveSpeed = mouseMotion.Relative;
            }
            else if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonIndex == (int) ButtonList.WheelUp)
                {
                    this.Zooming = true;
                    this.ZoomingDelta = ZoomSensitivity;
                }
                else if (mouseButton.ButtonIndex == (int) ButtonList.WheelDown)
                {
                    this.Zooming = true;
                    this.ZoomingDelta = -ZoomSensitivity;
                }
            }
        }
    }
}