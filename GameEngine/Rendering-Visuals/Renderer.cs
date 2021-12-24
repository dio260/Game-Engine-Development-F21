using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine.Physics;

namespace CPI311.GameEngine.Rendering
{
    public class Renderer : Component, IRenderable
    {
        public Material Material { get; set; }
        public Model ObjectModel;
        public Transform ObjectTransform;
        public Camera Camera;
        public int CurrentTechnique;
        public GraphicsDevice g;
        public Light Light;

        public Renderer()
        {

        }
        public Renderer(Model objModel, Transform objTransform, Camera camera,
        ContentManager content, GraphicsDevice graphicsDevice, Light light, int tech,
        string filename, float shiny, Texture2D diffuseTexture)
        {
            if (filename != null)
                Material = new Material(objTransform.World, camera, content,
                                    filename, tech, shiny, diffuseTexture, light);
            else
                Material = null;
            ObjectModel = objModel;
            ObjectTransform = objTransform;
            Camera = camera;
            CurrentTechnique = tech;
            g = graphicsDevice;
            Light = light;

            (ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.White.ToVector3();
        }

        public virtual void Draw()
        {
            if (Material != null)
            {
                //Material.Camera = Camera; // Update Material's properties
                Material.World = ObjectTransform.World;
                //Material.Light = Light;
                //Material.CurrentTechnique = CurrentTechnique;
                for (int i = 0; i < Material.Passes; i++)
                {
                    Material.Apply(i); // Look at the Material's Apply method
                    foreach (ModelMesh mesh in ObjectModel.Meshes)
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            g.SetVertexBuffer(part.VertexBuffer);
                            g.Indices = part.IndexBuffer;

                            g.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, part.VertexOffset, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                            /*
                                g.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 
                                                    part.StartIndex, part.PrimitiveCount);*/
                        }
                }
            }
            else
                ObjectModel.Draw(ObjectTransform.World, Camera.View, Camera.Projection);
        }

    }
}
