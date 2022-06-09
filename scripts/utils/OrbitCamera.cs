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

        protected Vector2 MoveSpeed;
        protected Vector3 RotationDelta;
        protected Vector3 PanningDelta;
        protected float ZoomingDelta;

        protected const float RADIAN = Mathf.Pi / 2;

        protected const float MIN_DISTANCE = 2f;

        protected bool Rotating { get; set; }
        protected bool Panning { get; set; }
        protected bool Zooming { get; set; }

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