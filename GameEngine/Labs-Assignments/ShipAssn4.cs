using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;

namespace CPI311.GameEngine
{
    public class ShipAssn4 : GameObject
    {
        public bool isActive;
        public ShipAssn4(ContentManager Content, Camera camera, GraphicsDevice g, Light light) : base()
        {
            //Transform.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            //add rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            //Add renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("ship"), Transform,
                                camera, Content, g, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            //add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = true;
        }

        public override void Update()
        {
            if (!isActive) return;
            if (InputManager.IsKeyDown(Keys.W))
                Transform.LocalPosition += Transform.Forward * Time.ElapsedGameTime * GameConstants.shipSpeed;
            if (InputManager.IsKeyDown(Keys.A))
                Transform.LocalPosition += Transform.Left * Time.ElapsedGameTime * GameConstants.shipSpeed;
            if (InputManager.IsKeyDown(Keys.S))
                Transform.LocalPosition += Transform.Back * Time.ElapsedGameTime * GameConstants.shipSpeed;
            if (InputManager.IsKeyDown(Keys.D))
                Transform.LocalPosition += Transform.Right * Time.ElapsedGameTime * GameConstants.shipSpeed;

            /*
            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
               Transform.Position.X < -GameConstants.PlayfieldSizeX ||
               Transform.Position.Z > GameConstants.PlayfieldSizeY ||
               Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }*/

            base.Update();

        }
    }
}
