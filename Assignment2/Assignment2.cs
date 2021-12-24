using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Physics;

namespace Assignment2
{
    public class Assignment2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //models, font, and cam
        SpriteFont font;
        Model sun, moon, earth, mercury, ground, guy;
        Effect sunEffect, moonEffect, earthEffect, mercuryEffect;
        Camera fixedCam;

        //need plane objects for rotation on different planes
        GameObject groundObj, guyObj, sunObj, 
                moonObj, earthObj, mercuryObj,
                earthPlane, mercuryPlane, moonPlane;

        //program variables
        bool cameraMode;
        string cameraText;
        float speed;
        Matrix proj, view;
        Dictionary<GameObject, Quaternion> originals;

        public Assignment2()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            InputManager.Initialize();
            Time.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //set reset values and speed
            originals = new Dictionary<GameObject, Quaternion>();
            speed = 1;


            //set parenting objects
            groundObj = new GameObject();
            groundObj.Transform.Position = Vector3.Down * 2 + Vector3.Right * 5;
            ground = Content.Load<Model>("Plane");

            //first person object represented as an unshaded blue sphere
            guy = Content.Load<Model>("Sphere");
            guyObj = new GameObject();
            guyObj.Add<Camera>();
            guyObj.Get<Camera>().Transform = new Transform();
            guyObj.Get<Camera>().Transform.Parent = guyObj.Transform;
            guyObj.Transform.Position = Vector3.Backward * 20;
            originals[guyObj] = guyObj.Transform.Rotation;

            sunObj = new GameObject();
            sun = Content.Load<Model>("Sol");
            sunObj.Transform.Position = Vector3.Up * 8;
            originals[sunObj] = sunObj.Transform.Rotation;

            earthPlane = new GameObject();
            mercuryPlane = new GameObject();
            //earthPlane.Transform.Parent = sunObj.Transform;
            earthPlane.Transform.Position = Vector3.Up * 8;
            //mercuryPlane.Transform.Parent = sunObj.Transform;
            mercuryPlane.Transform.Position = Vector3.Up * 8;

            originals[earthPlane] = earthPlane.Transform.Rotation;
            originals[mercuryPlane] = mercuryPlane.Transform.Rotation;

            earthObj = new GameObject();
            earth = Content.Load<Model>("Earth");
            earthObj.Transform.Parent = earthPlane.Transform;
            earthObj.Transform.LocalPosition = Vector3.Right * 15; //+ Vector3.Up * 8f;
            originals[earthObj] = earthObj.Transform.Rotation;

            moonPlane = new GameObject();
            moonPlane.Transform.Parent = earthPlane.Transform;
            moonPlane.Transform.LocalPosition = Vector3.Right * 15;
            originals[moonPlane] = moonPlane.Transform.Rotation;

            moonObj = new GameObject();
            moon = Content.Load<Model>("Luna");
            moonObj.Transform.Parent = moonPlane.Transform;
            moonObj.Transform.LocalPosition = Vector3.Left * 5f;
            originals[moonObj] = moonObj.Transform.Rotation;

            mercuryObj = new GameObject();
            mercury = Content.Load<Model>("Mercury");
            mercuryObj.Transform.Parent = mercuryPlane.Transform;
            //mercuryObj.Transform.Parent = sunObj.Transform;
            mercuryObj.Transform.LocalPosition = Vector3.Left * 10f; //+ Vector3.Up * 8f;
            originals[mercuryObj] = mercuryObj.Transform.Rotation;


            //set fixed cam
            font = Content.Load<SpriteFont>("Font");

            fixedCam = new Camera();
            fixedCam.Transform = new Transform();
            fixedCam.Transform.Position = Vector3.Up * 30 + Vector3.Backward * 10;
            //fixedCam.Transform.Rotate(Vector3.Down, 109.955f);
            fixedCam.Transform.Rotate(Vector3.Left, 45.2f);

            cameraMode = true;

            //lighting
            sunEffect = Content.Load<Effect>("SimpleShading");
            moonEffect = Content.Load<Effect>("SimpleShading");
            mercuryEffect = Content.Load<Effect>("SimpleShading");
            earthEffect = Content.Load<Effect>("SimpleShading");
            
            foreach (ModelMesh mesh in sun.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            }

            foreach (ModelMesh mesh in moon.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            }

            foreach (ModelMesh mesh in earth.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            }

            foreach (ModelMesh mesh in mercury.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            }
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            
            //first person view controls
            if (InputManager.IsKeyDown(Keys.W)) { guyObj.Transform.Position += guyObj.Transform.Forward * Time.ElapsedGameTime * 3; }
            if (InputManager.IsKeyDown(Keys.S)) { guyObj.Transform.Position += guyObj.Transform.Back * Time.ElapsedGameTime * 3; }
            if (InputManager.IsKeyDown(Keys.A)) { guyObj.Transform.Position += guyObj.Transform.Left * Time.ElapsedGameTime * 3; }
            if (InputManager.IsKeyDown(Keys.D)) { guyObj.Transform.Position += guyObj.Transform.Right * Time.ElapsedGameTime * 3; }
            if (InputManager.IsKeyDown(Keys.Up)) { guyObj.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.Down)) { guyObj.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.Left)) { guyObj.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * 3); }
            if (InputManager.IsKeyDown(Keys.Right)) { guyObj.Transform.Rotate(Vector3.Down, Time.ElapsedGameTime * 3); }
            
            //reset camera mode
            if (InputManager.IsKeyPressed(Keys.Tab)) { cameraMode = !cameraMode; }

            //reset specific value based on camera mode
            if (InputManager.IsKeyPressed(Keys.Space)) 
            {
                if (cameraMode)
                    guyObj.Transform.Rotation = originals[guyObj];
                else
                    fixedCam.FieldOfView = MathHelper.PiOver2;
            }

            //reset rotation axes and planes
            if (InputManager.IsKeyPressed(Keys.LeftShift))
            {
                sunObj.Transform.Rotation = originals[sunObj];
                mercuryObj.Transform.Rotation = originals[mercuryObj];
                earthObj.Transform.Rotation = originals[earthObj];
                moonObj.Transform.Rotation = originals[moonObj];
                earthPlane.Transform.Rotation = originals[earthPlane];
                mercuryPlane.Transform.Rotation = originals[mercuryPlane];
                moonPlane.Transform.Rotation = originals[moonPlane];
            }

            //rotate planes and axes
            if (InputManager.IsKeyDown(Keys.Z)) { mercuryPlane.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.X)) { earthPlane.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.C)) { moonPlane.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.V)) { sunObj.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.B)) { earthObj.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.N)) { mercuryObj.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
            if (InputManager.IsKeyDown(Keys.M)) { moonObj.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }

            //change and reset animation speed
            if (InputManager.IsKeyDown(Keys.E)) { speed += 0.01f; }
            if (InputManager.IsKeyDown(Keys.Q)) { speed -= 0.01f; }
            if (InputManager.IsKeyPressed(Keys.LeftControl)) { speed = 1; }

            if (InputManager.IsKeyDown(Keys.R)) { fixedCam.FieldOfView -= 0.01f; }
            if (InputManager.IsKeyDown(Keys.F)) { fixedCam.FieldOfView += 0.01f; }

            //the automatic rotations
            sunObj.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed);
            mercuryPlane.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed);
            earthPlane.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed);
            moonPlane.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed);
            earthObj.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed);
            moonObj.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * 2 * speed);

            //change camera view
            if (cameraMode)
            {
                cameraText = "First Person Mode";
                view = guyObj.Get<Camera>().View;
                proj = guyObj.Get<Camera>().Projection;
            }
            else
            {
                cameraText = "Third Person Mode";
                view = fixedCam.View;
                proj = fixedCam.Projection;
            }
                
            Time.Update(gameTime);
            InputManager.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //draws
            ground.Draw(groundObj.Transform.World, view, proj);
            guy.Draw(guyObj.Transform.World, view, proj);
            moon.Draw(moonObj.Transform.World, view, proj);
            earth.Draw(earthObj.Transform.World, view, proj);
            mercury.Draw(mercuryObj.Transform.World, view, proj);
            sun.Draw(sunObj.Transform.World, view, proj);

            _spriteBatch.Begin();

            //change text based on camera mode
            if(cameraMode)
            {
                _spriteBatch.DrawString(font, "Use WASD to move the guy in first person",
                           new Vector2(5, 420), Color.Black);
                _spriteBatch.DrawString(font, "Use the Arrow Keys to rotate his view",
                           new Vector2(5, 440), Color.Black);
                _spriteBatch.DrawString(font, "Use Space to reset his view",
                           new Vector2(5, 460), Color.Black);
            }
            else
            {
                _spriteBatch.DrawString(font, "Use WASD to move the guy (small blue sphere)",
                           new Vector2(5, 420), Color.Black);
                _spriteBatch.DrawString(font, "Use R to zoom in and F to zoom out",
                           new Vector2(5, 440), Color.Black);
                _spriteBatch.DrawString(font, "Use Space to reset the zoom",
                           new Vector2(5, 460), Color.Black);
            }
            _spriteBatch.DrawString(font, "Switch camera view with tab",
                       new Vector2(600, 440), Color.Black);
            _spriteBatch.DrawString(font, "Current camera view: " + cameraText,
                       new Vector2(515, 460), Color.Black);

            _spriteBatch.DrawString(font, "Use Q and E to decrease/increase animation speed",
                       new Vector2(5, 5), Color.Black);
            _spriteBatch.DrawString(font, "Use Left CTRL to reset animation speed",
                       new Vector2(5, 25), Color.Black);
            _spriteBatch.DrawString(font, "Current speed: " + speed,
                       new Vector2(5, 45), Color.Black);

            _spriteBatch.DrawString(font, "Use the Keys listed below to tilt specific features:",
                       new Vector2(460, 5), Color.Black);
            _spriteBatch.DrawString(font, "Z for Mercury Revolution Plane",
                       new Vector2(585, 25), Color.Black);
            _spriteBatch.DrawString(font, "X for Earth Revolution Plane",
                       new Vector2(600, 45), Color.Black);
            _spriteBatch.DrawString(font, "C for Luna Revolution Plane",
                       new Vector2(600, 65), Color.Black);
            _spriteBatch.DrawString(font, "V for Sol Rotation Axis",
                       new Vector2(640, 85), Color.Black);
            _spriteBatch.DrawString(font, "B for Earth Rotation Axis",
                       new Vector2(625, 105), Color.Black);
            _spriteBatch.DrawString(font, "N for Mercury Rotation Axis",
                       new Vector2(605, 125), Color.Black);
            _spriteBatch.DrawString(font, "M for Moon Rotation Axis",
                       new Vector2(620, 145), Color.Black);
            _spriteBatch.DrawString(font, "Left Shift to reset all to default values",
                       new Vector2(545, 165), Color.Black);



            _spriteBatch.End();


            
            //groundObj.Draw();

            base.Draw(gameTime);
        }
    }
}
