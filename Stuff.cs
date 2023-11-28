using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace SimCars
{
    class Stuff
    {
        public static GraphicsDevice device;
        public static ContentManager Content;

        static Matrix viewMatrix;
        static Matrix projectionMatrix;

        public Vector3 position;
        public Quaternion rotation;
        
        Quaternion cameraRotation;

        Model model;       

        public Stuff()
        {
            position = new Vector3(0.0f, 0.0f, 0.0f);
            rotation = Quaternion.Identity;

            cameraRotation = Quaternion.Identity;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                device.Viewport.AspectRatio, 1.0f, 1000.0f);
        }

        public void LoadModel(string assetName)
        {
            model = Content.Load<Model>(assetName);
        }

        protected float lerpAmount = 1.0f;
         
        public void UpdateCamera()
        {
            cameraRotation = Quaternion.Lerp(cameraRotation, rotation, lerpAmount);
            Vector3 cameraPosition = new Vector3(0f , 1.7f, 4f);
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateFromQuaternion(cameraRotation));
            cameraPosition += position;

            Vector3 cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromQuaternion(cameraRotation));

            Vector3 cameraTarget = new Vector3(0f, 1.0f, 0f);
            cameraTarget = Vector3.Transform(cameraTarget, Matrix.CreateFromQuaternion(cameraRotation));
            cameraTarget += position;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUp);
        }

        public void Draw()
        {
            Matrix worldMatrix = 
                Matrix.CreateFromQuaternion(rotation) * 
                Matrix.CreateTranslation(position);

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
