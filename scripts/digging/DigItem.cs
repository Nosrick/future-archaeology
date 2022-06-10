using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.digging
{
    public class DigItem : RigidBody
    {
        public MeshInstance ObjectMesh { get; protected set; }

        public CollisionShape CollisionShape { get; protected set; }

        protected ShaderMaterial OutlineMaterial { get; set; }
        protected Material MyMaterial { get; set; }
        protected AnimationPlayer MyAnimationPlayer { get; set; }

        public const string PICKUP_ANIM = "PickupBounce";

        [Export] public int CashValue { get; protected set; }

        public override void _Ready()
        {
            base._Ready();

            this.GetStuff();

            this.OutlineMaterial = GD.Load<ShaderMaterial>("assets/shaders/outline-material.tres");

            this.MyMaterial = this.ObjectMesh.Mesh.SurfaceGetMaterial(0);

            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }

        protected void GetStuff()
        {
            this.ObjectMesh = this.GetNode<MeshInstance>("ObjectMesh");
            this.CollisionShape = this.GetNode<CollisionShape>("CollisionShape");
            this.MyAnimationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public void MakeMeGlow()
        {
            if (this.MyMaterial is null == false)
            {
                this.MyMaterial.NextPass = this.OutlineMaterial;
            }
        }

        public void PlayPickupAnimation()
        {
            this.MyAnimationPlayer.Play(PICKUP_ANIM);
        }

        public void AssignObject(MeshInstance meshInstance, int cashValue)
        {
            this.CashValue = cashValue;
            this.ObjectMesh = meshInstance;
            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }

        public Dictionary Save()
        {
            Dictionary saveDict = new Dictionary();

            saveDict.Add("mesh", this.ObjectMesh.Mesh.ResourcePath);
            saveDict.Add("material", this.ObjectMesh.Mesh.SurfaceGetMaterial(0).ResourcePath);
            saveDict.Add("value", this.CashValue);
            saveDict.Add("translation", this.Translation);
            saveDict.Add("rotation", this.Rotation);
            saveDict.Add("scale", this.Scale);

            return saveDict;
        }

        public bool Load(Dictionary data)
        {
            if (!data.Contains("mesh"))
            {
                return false;
            }

            this.GetStuff();
            this.ObjectMesh.Mesh = GD.Load<Mesh>(data["mesh"] as string);

            if (!data.Contains("material"))
            {
                return false;
            }

            this.ObjectMesh.Mesh.SurfaceSetMaterial(0, GD.Load<Material>(data["material"] as string));

            this.Scale = (Vector3) data["scale"];
            this.Rotation = (Vector3) data["rotation"];
            this.Translation = (Vector3) data["translation"];

            if (!data.Contains("value"))
            {
                return false;
            }

            this.CashValue = (int) data["value"];

            return true;
        }
    }
}