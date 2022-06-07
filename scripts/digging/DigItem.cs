using Godot;

namespace DiggyDig.scripts.digging
{
    public class DigItem : RigidBody
    {
        public MeshInstance ObjectMesh { get; protected set; }

        public CollisionShape CollisionShape { get; protected set; }
        
        [Export] public int CashValue { get; protected set; }

        public override void _Ready()
        {
            base._Ready();

            this.ObjectMesh = this.GetNode<MeshInstance>("ObjectMesh");
            this.CollisionShape = this.GetNode<CollisionShape>("CollisionShape");

            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }

        public void AssignObject(MeshInstance meshInstance, int cashValue)
        {
            this.CashValue = cashValue;
            this.ObjectMesh = meshInstance;
            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }
    }
}