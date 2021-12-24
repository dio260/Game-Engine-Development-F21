using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class BulletFinalProj : GameObject
    {
        float speed = 30f;
        public float alivetime;

        public BulletFinalProj(ContentManager Content, Camera camera, GraphicsDevice g, Light light, FirstPersonPlayer player) : base()
        {
            alivetime = 0;
            Transform.Scale = Vector3.One * 0.05f;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("sphere"), Transform,
                                camera, Content, g, light, 1, null, 20f, texture);
            foreach (ModelMesh mesh in renderer.ObjectModel.Meshes)
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DiffuseColor = Color.Black.ToVector3();
                    }
                       
            Add<Renderer>(renderer);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * 0.05f;
            //sphereCollider.Radius = 0.05f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            Transform.LocalPosition = player.Transform.LocalPosition + player.Transform.Forward;
            Rigidbody.Velocity = player.Transform.Forward * speed;
        }

        public override void Update()
        {
            alivetime += 0.1f;
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
            //base.Draw();
        }
    }
}
