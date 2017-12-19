using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_IP3D
{
    class Camera
    {
        Vector3 posicao;
        float alturaCam = 5.0f;
        Vector3 directionBase = Vector3.UnitX;
        public Matrix view;
        Vector3 speed = new Vector3(1.0f, 0.0f, 0.0f);
        float yaw = 0.01f;
        float pitch = 0.01f;
        Matrix Projection;
        float offSetChao = 1.80f;
        float offSetTank = 5.0f;

        public Camera(GraphicsDevice device)
        {
            posicao = new Vector3(64.0f, alturaCam, 64.0f);
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            view = Matrix.CreateLookAt(posicao, speed, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 100.0f);
        }

        public void UpdateCameraPositionSurfaceFollow(ClsBattlefield terreno, KeyboardState keyboardState)
        {
            MouseState mousestate = Mouse.GetState();

            pitch = MathHelper.ToRadians(mousestate.Y * 0.1f);
            Matrix pitchRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);

            if (keyboardState.IsKeyDown(Keys.NumPad4))
                speed = Vector3.Transform(speed, Matrix.CreateRotationY(yaw));
            if (keyboardState.IsKeyDown(Keys.NumPad6))
                speed = Vector3.Transform(speed, Matrix.CreateRotationY(-yaw));

            //Matrix yawRotation = Matrix.CreateRotationY(yaw);
            //Vector3 dir = speed;
            //dir.Normalize();
            //yawRotation.Forward = dir;
            //yawRotation.Up = Vector3.UnitY;
            //yawRotation.Right = Vector3.Cross(dir, Vector3.UnitY);
            //view = yawRotation * Matrix.CreateTranslation(posicao);
            //direcao = Vector3.Transform(directionBase, yawRotation);

            if (keyboardState.IsKeyDown(Keys.NumPad8))
            {
                posicao = posicao + speed;
                alturaCam = terreno.Interpolacao(posicao.X, posicao.Z);
                posicao.Y = alturaCam + offSetChao;
            }
            if (keyboardState.IsKeyDown(Keys.NumPad2))
            {
                posicao = posicao - speed;
                alturaCam = terreno.Interpolacao(posicao.X, posicao.Z);
                posicao.Y = alturaCam + offSetChao;
            }

            view = Matrix.CreateLookAt(posicao, posicao + speed, Vector3.Up);
        }

        public void UpdateCameraPositionTankFollow(TankClass tanque, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.W))
            {
                posicao = tanque.PositionTank;
                posicao.X = posicao.X + offSetTank;
                posicao.Y = posicao.Y + offSetChao;
                posicao.Z = posicao.Z + offSetTank;
                alturaCam = tanque.PositionTank.Y + offSetChao;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                posicao = tanque.PositionTank;
                posicao.X = posicao.X + offSetTank;
                posicao.Y = posicao.Y + offSetChao;
                posicao.Z = posicao.Z + offSetTank;
                alturaCam = tanque.PositionTank.Y + offSetChao;
            }

            if (keyboard.IsKeyDown(Keys.A))
                speed = tanque.DirectionTank;
            if (keyboard.IsKeyDown(Keys.D))
                speed = tanque.DirectionTank;

            view = Matrix.CreateLookAt(posicao, speed, Vector3.Up);
        }
    }
}
