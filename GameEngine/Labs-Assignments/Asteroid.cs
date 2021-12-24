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
    public class Asteroid : GameObject
    {
        public bool isActive;
        public Asteroid(ContentManager Content, Camera camera, GraphicsDevice g, Light light) : base()
        {
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("asteroid4"), Transform,
                                camera, Content, g, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = false;
        }
        public override void Update()
        {
            if (!isActive) return;

            if (Transform.Position.X > GameConstants.PlayfieldSizeX)
                Transform.Position -= Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            if (Transform.Position.X < -GameConstants.PlayfieldSizeX)
                Transform.Position += Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            if (Transform.Position.Z > GameConstants.PlayfieldSizeY)
                Transform.Position -= Vector3.UnitZ * 2 * GameConstants.PlayfieldSizeY;
            if (Transform.Position.Z < -GameConstants.PlayfieldSizeY)
                Transform.Position += Vector3.UnitZ * 2 * GameConstants.PlayfieldSizeY;

            base.Update();
        }
        public override void Draw()
        {
            //if (isActive) 
                base.Draw();
        }

    }
}
