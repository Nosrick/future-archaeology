using Godot;

namespace DiggyDig.scripts.digging
{
    public class DigItem : StaticBody
    {
        protected MeshInstance ObjectMesh;

        protected CollisionShape CollisionShape;

        public override void _Ready()
        {
            base._Ready();

            this.ObjectMesh = this.GetNode<MeshInstance>("ObjectMesh");
            this.CollisionShape = this.GetNode<CollisionShape>("CollisionShape");

            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }

        public void AssignObject(MeshInstance meshInstance)
        {
            this.ObjectMesh = meshInstance;
            this.CollisionShape.Shape = this.ObjectMesh.Mesh.CreateConvexShape();
        }
    }
}