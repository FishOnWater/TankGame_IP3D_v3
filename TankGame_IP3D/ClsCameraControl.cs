using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TankGame_IP3D
{
    class ClsCameraControl
    {
        Matrix cameraMatrix;
        BasicEffect effect;
        Vector3 vectorCamera = new Vector3(2.0f, 2.0f, 2.0f);
        Vector3 posicao;
        MouseState rato;
        MouseState previousState;
        Vector2 centro;
        float yaw;
        float pitch;
        ContentManager content;

        public ClsCameraControl(GraphicsDevice device)
        {
            effect = new BasicEffect(device);
            cameraMatrix = Matrix.Identity;
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(2.0f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10.0f);

            UpdateCameraPosition(device);
        }

        public void UpdateCameraPosition(GraphicsDevice device)
        {
            MouseState rato = Mouse.GetState();
            Vector2 posRato, diferenca = new Vector2(0.0f, 0.0f);
            Vector2 centro = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);

            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            Vector3 directionBase = Vector3.UnitX;
            float yaw = MathHelper.ToRadians(1.0f);
            Vector3 speed = new Vector3(0.1f, 0f, 0f);

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
                speed = Vector3.Transform(speed, Matrix.CreateRotationY(yaw));
            if (keyboardState.IsKeyDown(Keys.Right))
                speed = Vector3.Transform(speed, -Matrix.CreateRotationY(yaw));

            Matrix rotation = Matrix.CreateRotationY(yaw);
            Vector3 dir = speed;
            dir.Normalize();
            rotation.Forward = dir;
            rotation.Up = Vector3.UnitY;
            rotation.Right = Vector3.Cross(dir, Vector3.UnitY);
            cameraMatrix = rotation * Matrix.CreateTranslation(vectorCamera);
            Vector3 direction = Vector3.Transform(directionBase, rotation);

            if (keyboardState.IsKeyDown(Keys.Up))
                vectorCamera = vectorCamera + speed;
            if (keyboardState.IsKeyDown(Keys.Down))
                vectorCamera = -vectorCamera + speed;

            effect.View = Matrix.CreateLookAt(vectorCamera, Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10.0f);

            //return vectorCamera;
        }
    }
}
