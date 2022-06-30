using ATimeGoneBy.scripts.options;
using Godot;

namespace ATimeGoneBy.scripts.utils
{
    public class OrbitCamera : Camera
    {
        [Export] protected NodePath OrbitTargetPath;

        protected float RotationSensitivity = 0.1f;
        protected float PanSensitivity = 1f;
        protected float ZoomSensitivity = 1f;

        protected Spatial OrbitTarget;

        protected Transform OriginalTarget;
        protected Transform OriginalTransform;

        protected Vector2 MoveSpeed;
        protected Vector3 RotationDelta;
        protected Vector3 PanningDelta;
        protected float ZoomingDelta;

        protected const float RADIAN = Mathf.Pi / 2;

        protected const float MIN_DISTANCE = 2f;

        public bool Rotating { get; protected set; }
        public bool Panning { get; protected set; }
        public bool Zooming { get; protected set; }

        protected int XRotationDirection = 1;
        protected int YRotationDirection = 1;
        protected int XPanningDirection = 1;
        protected int YPanningDirection = 1;
        protected int ZoomingDirection = 1;

        public override void _Ready()
        {
            Node target = this.GetNode(this.OrbitTargetPath);

            this.RotationDelta = this.Transform.basis.GetEuler();
            this.PanningDelta = Vector3.Zero;

            if (target is Spatial spatial)
            {
                this.OrbitTarget = spatial;
                this.OriginalTarget = new Transform(this.OrbitTarget.Transform.basis, this.OrbitTarget.Transform.origin);
                this.OriginalTransform = new Transform(this.Transform.basis, this.Transform.origin);
                this.SetRotation();
            }
            else
            {
                GD.PrintErr("OrbitTargetPath is not of type Spatial!");
            }

            this.RefreshOptions();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (this.Rotating)
            {
                this.RotationDelta.x += this.RotationSensitivity * this.MoveSpeed.y * delta * this.XRotationDirection;
                this.RotationDelta.y += this.RotationSensitivity * this.MoveSpeed.x * delta * this.YRotationDirection;

                this.MoveSpeed = Vector2.Zero;
                this.SetRotation();
            }
            else if (this.Panning)
            {
                this.PanningDelta.x += this.PanSensitivity * this.MoveSpeed.x * delta * this.XPanningDirection;
                this.PanningDelta.y -= this.PanSensitivity * this.MoveSpeed.y * delta * this.YPanningDirection;

                this.MoveSpeed = Vector2.Zero;
                this.OrbitTarget.TranslateObjectLocal(this.PanningDelta);

                this.LookAt(this.OrbitTarget.GlobalTransform.origin, this.GlobalTransform.basis.y);
                this.PanningDelta = Vector3.Zero;
            }
            else if (this.Zooming)
            {
                if (this.Translation.DistanceTo(this.OrbitTarget.Translation) > MIN_DISTANCE || this.ZoomingDelta < 0)
                {
                    this.Translate(Vector3.Forward * this.ZoomingDelta * this.ZoomingDirection);
                }
            }

            if (this.Zooming)
            {
                this.Zooming = false;
                this.ZoomingDelta = 0;
            }
        }

        public void RefreshOptions()
        {
            OptionHandler optionHandler = GlobalConstants.AppManager.OptionHandler;

            this.XRotationDirection = optionHandler.GetOption<bool>(optionHandler.InvertXRotation) ? -1 : 1;
            this.YRotationDirection = optionHandler.GetOption<bool>(optionHandler.InvertYRotation) ? -1 : 1;

            this.XPanningDirection = optionHandler.GetOption<bool>(optionHandler.InvertXPanning) ? -1 : 1;
            this.YPanningDirection = optionHandler.GetOption<bool>(optionHandler.InvertYPanning) ? -1 : 1;

            this.ZoomingDirection = optionHandler.GetOption<bool>(optionHandler.InvertZooming) ? -1 : 1;

            this.RotationSensitivity = optionHandler.GetOption<float>(optionHandler.RotationSensitivity);
            this.PanSensitivity = optionHandler.GetOption<float>(optionHandler.PanningSensitivity);
            this.ZoomSensitivity = optionHandler.GetOption<float>(optionHandler.ZoomingSensitivity);
        }

        protected void SetRotation()
        {
            Quat t = new Quat(this.RotationDelta);
            Transform orbitTargetTransform = this.OrbitTarget.Transform;
            orbitTargetTransform.basis = new Basis(t);
            this.OrbitTarget.Transform = orbitTargetTransform;
        }

        protected void ResetCamera()
        {
            this.Transform = this.OriginalTransform;
            this.OrbitTarget.Transform = this.OriginalTarget;

            this.MoveSpeed = Vector2.Zero;

            this.PanningDelta = Vector3.Zero;
            this.ZoomingDelta = 0;
            
            this.RotationDelta = Vector3.Zero;
            this.SetRotation();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._Input(@event);

            if (@event.IsActionReleased("reset_camera"))
            {
                this.ResetCamera();
            }

            if (!this.Rotating && @event.IsActionPressed("camera_rotate_modifier"))
            {
                this.Rotating = true;
            }
            else if (this.Rotating && @event.IsActionReleased("camera_rotate_modifier"))
            {
                this.Rotating = false;
            }

            if (!this.Panning && @event.IsActionPressed("camera_pan_modifier"))
            {
                this.Panning = true;
            }
            else if (this.Panning && @event.IsActionReleased("camera_pan_modifier"))
            {
                this.Panning = false;
            }

            if (@event is InputEventMouseMotion mouseMotion)
            {
                this.Zooming = false;
                this.MoveSpeed = mouseMotion.Relative;
            }
            else if (@event.IsActionPressed("camera_zoom_in"))
            {
                this.Zooming = true;
                this.ZoomingDelta = ZoomSensitivity;
            }
            else if (@event.IsActionPressed("camera_zoom_out"))
            {
                this.Zooming = true;
                this.ZoomingDelta = -ZoomSensitivity;
            }
        }
    }
}