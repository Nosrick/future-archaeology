using System;
using ATimeGoneBy.scripts.utils;
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
        
        protected int SurfaceCount { get; set; }
        
        public bool Uncovered { get; protected set; }
        
        public bool Flashing { get; protected set; }
        public bool Glowing { get; protected set; }

        public const string PICKUP_ANIM = "PickupBounce";

        [Export] public int CashValue { get; protected set; }

        public override void _Ready()
        {
            base._Ready();

            this.GetStuff();
        }

        protected void GetStuff()
        {
            this.ObjectMesh = this.GetNode<MeshInstance>("ObjectMesh");
            this.CollisionShape = this.GetNode<CollisionShape>("CollisionShape");
            this.MyAnimationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");

            this.OutlineMaterial = GD.Load<ShaderMaterial>("assets/shaders/outline-material.tres");
            this.FlashMaterial = GD.Load<ShaderMaterial>("assets/shaders/flash-item-material.tres");
        }

        protected void GetDuplicates()
        {
            this.ObjectMesh.Mesh = this.ObjectMesh.Mesh.Duplicate() as Mesh;
            this.MyMaterial = this.MyMaterial.Duplicate() as Material;
            for (int i = 0; i < this.SurfaceCount; i++)
            {
                this.ObjectMesh.SetSurfaceMaterial(i, this.MyMaterial);
            }
        }

        public void MarkMeCovered()
        {
            this.Uncovered = false;

            if (this.Glowing && this.MyMaterial is null == false)
            {
                this.Glowing = false;
                this.MyMaterial.NextPass = null;
                for (int i = 0; i < this.SurfaceCount; i++)
                {
                    this.ObjectMesh.SetSurfaceMaterial(i, this.MyMaterial);
                }
            }
        }

        public void MarkMeUncovered()
        {
            this.Uncovered = true;
            
            if (!this.Glowing && this.MyMaterial is null == false)
            {
                this.Glowing = true;
                this.MyMaterial.NextPass = this.OutlineMaterial;
                for (int i = 0; i < this.SurfaceCount; i++)
                {
                    this.ObjectMesh.SetSurfaceMaterial(i, this.MyMaterial);
                }
            }
        }

        public void MakeMeFlash()
        {
            if (!this.Flashing && this.MyMaterial is null == false)
            {
                this.Flashing = true;
                
                this.FlashMaterial.SetShaderParam("axisIndex", GD.Randi() % 3);
                this.FlashMaterial.SetShaderParam("axisDir", RandomUtil.PosNegCoinFlip());
                this.MyMaterial.NextPass = this.FlashMaterial;
                for (int i = 0; i < this.SurfaceCount; i++)
                {
                    this.ObjectMesh.SetSurfaceMaterial(i, this.MyMaterial);
                }
            }
        }

        public void EndMyFlash()
        {
            if (this.Flashing && this.MyMaterial is null == false)
            {
                this.Flashing = false;
                
                this.FlashMaterial.SetShaderParam("axisIndex", 0);
                this.FlashMaterial.SetShaderParam("axisDir", 0);
                this.MyMaterial.NextPass = null;
                for (int i = 0; i < this.SurfaceCount; i++)
                {
                    this.ObjectMesh.SetSurfaceMaterial(i, this.MyMaterial);
                }
            }
        }

        public void PlayPickupAnimation()
        {
            this.MyAnimationPlayer.Play(PICKUP_ANIM);
        }

        public void AssignObject(MeshInstance mesh, Material material, int cashValue, bool deferred = false)
        {
            this.CashValue = cashValue;

            if (this.IsInsideTree())
            {
                this.Name = mesh.Name;
                
                this.MeshPath = mesh.Filename;
                this.MaterialPath = material.ResourcePath;
                
                this.ObjectMesh.Mesh = (Mesh) mesh.Mesh.Duplicate();
                this.SurfaceCount = this.ObjectMesh.GetSurfaceMaterialCount();
                this.MyMaterial = (Material) material.Duplicate();
                this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
                
                this.GetDuplicates();
                return;
            }

            if (!deferred)
            {
                this.CallDeferred(nameof(this.AssignObject), mesh, material, cashValue, true);
            }
        }

        protected void AssignObject(
            MeshInstance mesh,
            Material material,
            string meshPath,
            string materialPath,
            int cashValue,
            bool deferred = false)
        {
            this.CashValue = cashValue;

            if (this.IsInsideTree())
            {
                this.Name = mesh.Name;
                
                this.MeshPath = mesh.Filename;
                this.MaterialPath = material.ResourcePath;
                
                this.ObjectMesh.Mesh = (Mesh) mesh.Mesh.Duplicate();
                this.SurfaceCount = this.ObjectMesh.GetSurfaceMaterialCount();
                this.MyMaterial = (Material) material.Duplicate();
                this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
                
                this.MeshPath = meshPath;
                this.MaterialPath = materialPath;
                
                this.GetDuplicates();
                return;
            }

            if (!deferred)
            {
                this.CallDeferred(nameof(this.AssignObject), mesh, material, cashValue, true);
            }
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
            string meshPath = data["mesh"] as string;
            MeshInstance myMesh = GD.Load<PackedScene>(meshPath).Instance<MeshInstance>().Duplicate() as MeshInstance;

            if (!data.Contains("material"))
            {
                return false;
            }

            string materialPath = data["material"] as string;
            Material material = GD.Load<Material>(materialPath).Duplicate() as Material;

            if (!data.Contains("value"))
            {
                return false;
            }
            int cashValue = (int) data["value"];
            
            this.AssignObject(myMesh, material, meshPath, materialPath, cashValue);

            this.Scale = (Vector3) data["scale"];
            this.Rotation = (Vector3) data["rotation"];
            this.Translation = (Vector3) data["translation"];
            this.PathsRetrieved = true;

            return true;
        }
    }
}