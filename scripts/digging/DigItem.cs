using Godot;

namespace ATimeGoneBy.scripts.digging
{
    public class DigItem : RigidBody
    {
        public MeshInstance ObjectMesh { get; protected set; }

        public CollisionShape CollisionShape { get; protected set; }
        
        protected ShaderMaterial OutlineMaterial { get; set; }
        protected Material MyMaterial { get; set; }
        
        [Export] public int CashValue { get; protected set; }

        public override void _Ready()
        {
            base._Ready();

            this.ObjectMesh = this.GetNode<MeshInstance>("ObjectMesh");
            this.CollisionShape = this.GetNode<CollisionShape>("CollisionShape");

            this.ObjectMesh = (MeshInstance) this.ObjectMesh.Duplicate();
            
            this.OutlineMaterial = GD.Load<ShaderMaterial>("assets/shaders/outline-material.tres");
            
            this.MyMaterial = (Material) this.ObjectMesh.Mesh.SurfaceGetMaterial(0).Duplicate(true);
            this.MyMaterial.NextPass = this.OutlineMaterial;
            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }

        public void MakeMeGlow()
        {
            this.ObjectMesh.Mesh.SurfaceSetMaterial(0, this.MyMaterial);
        }

        public void AssignObject(MeshInstance meshInstance, int cashValue)
        {
            this.CashValue = cashValue;
            this.ObjectMesh = meshInstance;
            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }
    }
}