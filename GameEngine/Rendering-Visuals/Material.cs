using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine.Rendering
{
    public class Material
    {
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }
        public Effect effect;
        public Texture2D DiffuseTexture;
        public Vector3 diffuse, ambient, specular;
        public float Shininess;
        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }
        public int CurrentTechnique { get; set; }

        public Matrix World { get; set; }
        public Camera Camera { get; set; }

        public Light Light;


        public Material(Matrix world, Camera camera, ContentManager content,
                        string filename, int tech, float shiny, Texture2D diffuseTexture, Light light)
        {
            effect = content.Load<Effect>(filename);
            World = world; Camera = camera;
            CurrentTechnique = tech; Shininess = shiny;
            DiffuseTexture = diffuseTexture;
            Ambient = Color.LightGray.ToVector3();
            Specular = Color.LightGray.ToVector3();
            Diffuse = Color.LightGray.ToVector3();
            Light = light;
        }
        public virtual void Apply(int currentPass)
        {
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.Position);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.Position);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["AmbientColor"].SetValue(Ambient);
            effect.Parameters["DiffuseColor"].SetValue(Diffuse);
            effect.Parameters["SpecularColor"].SetValue(Specular);
            effect.Parameters["DiffuseTexture"].SetValue(DiffuseTexture);

            effect.CurrentTechnique.Passes[currentPass].Apply();
        }

    }
}
