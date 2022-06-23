using System;
using Godot;
using Godot.Collections;

namespace ATimeGoneBy.scripts.digging
{
    public class DigItem : RigidBody
    {
        public MeshInstance ObjectMesh { get; protected set; }

        public CollisionShape CollisionShape { get; protected set; }

        protected ShaderMaterial OutlineMaterial { get; set; }
        protected ShaderMaterial FlashMaterial { get; set; }
        
        protected string MeshPath { get; set; }
        protected string MaterialPath { get; set; }
        protected Material MyMaterial { get; set; }
        protected AnimationPlayer MyAnimationPlayer { get; set; }
        
        protected bool PathsRetrieved { get; set; }
        
        public bool Uncovered { get; protected set; }
        
        public bool Flashing { get; protected set; }
        public bool Glowing { get; protected set; }

        public const string PICKUP_ANIM = "PickupBounce";

        [Export] public int CashValue { get; protected set; }

        public override void _Ready()
        {
            base._Ready();

            this.GetStuff();
            this.GetPaths();
            this.GetDuplicates();
        }

        protected void GetStuff()
        {
            this.ObjectMesh = this.GetNode<MeshInstance>("ObjectMesh");
            this.CollisionShape = this.GetNode<CollisionShape>("CollisionShape");
            this.MyAnimationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");

            this.OutlineMaterial = GD.Load<ShaderMaterial>("assets/shaders/outline-material.tres");
            this.FlashMaterial = GD.Load<ShaderMaterial>("assets/shaders/flash-material.tres");

            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
            this.MyMaterial = this.ObjectMesh.Mesh.SurfaceGetMaterial(0);
        }

        protected void GetPaths()
        {
            if (this.PathsRetrieved == false)
            {
                this.MeshPath = this.ObjectMesh.Mesh.ResourcePath;
                this.MaterialPath = this.MyMaterial.ResourcePath;
            }
        }

        protected void GetDuplicates()
        {
            this.ObjectMesh.Mesh = this.ObjectMesh.Mesh.Duplicate() as Mesh;
            this.MyMaterial = this.MyMaterial.Duplicate() as Material;
            this.ObjectMesh.Mesh?.SurfaceSetMaterial(0, this.MyMaterial);
        }

        public void MarkMeUncovered()
        {
            this.Uncovered = true;
        }

        public void MakeMeGlow()
        {
            if (!this.Glowing && this.MyMaterial is null == false)
            {
                this.Glowing = true;
                this.MyMaterial.NextPass = this.OutlineMaterial;
                this.ObjectMesh.Mesh.SurfaceSetMaterial(0, this.MyMaterial);
            }
        }

        public void MakeMeFlash()
        {
            if (!this.Flashing && this.MyMaterial is null == false)
            {
                this.Flashing = true;
                this.MyMaterial.NextPass = this.FlashMaterial;
                this.ObjectMesh.Mesh.SurfaceSetMaterial(0, this.MyMaterial);
            }
        }

        public void PlayPickupAnimation()
        {
            this.MyAnimationPlayer.Play(PICKUP_ANIM);
        }

        public void AssignObject(ArrayMesh mesh, Material material, int cashValue, bool deferred = false)
        {
            this.CashValue = cashValue;

            if (this.IsInsideTree())
            {
                this.ObjectMesh.Mesh = (ArrayMesh) mesh.Duplicate();
                this.MyMaterial = (Material) material.Duplicate();
                this.ObjectMesh.SetSurfaceMaterial(0, this.MyMaterial);
                this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
                return;
            }

            if (!deferred)
            {
                this.CallDeferred(nameof(this.AssignObject), mesh, cashValue, true);
            }
        }

        public void AssignObject(Tuple<ArrayMesh, Material> itemTuple, int cashValue, bool deferred = false)
        {
            this.AssignObject(itemTuple.Item1, itemTuple.Item2, cashValue, deferred);
        }

        public Dictionary Save()
        {
            Dictionary saveDict = new Dictionary();

            saveDict.Add("mesh", this.MeshPath);
            saveDict.Add("material", this.MaterialPath);
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
            this.MeshPath = data["mesh"] as string;
            this.ObjectMesh.Mesh = GD.Load<Mesh>(this.MeshPath).Duplicate() as Mesh;

            if (!data.Contains("material"))
            {
                return false;
            }

            this.MaterialPath = data["material"] as string;
            this.MyMaterial = GD.Load<Material>(this.MaterialPath).Duplicate() as Material;
            this.ObjectMesh.Mesh.SurfaceSetMaterial(0, this.MyMaterial);

            this.Scale = (Vector3) data["scale"];
            this.Rotation = (Vector3) data["rotation"];
            this.Translation = (Vector3) data["translation"];

            if (!data.Contains("value"))
            {
                return false;
            }

            this.CashValue = (int) data["value"];
            this.PathsRetrieved = true;

            return true;
        }
    }
}