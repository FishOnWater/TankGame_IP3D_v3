using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankGame_IP3D
{
    class TankClass
    {
        Model modelTank;

        Matrix world;
        Matrix view;
        Matrix projection;
        BasicEffect effect;

        ModelBone turretBone;
        ModelBone canonBone;
        ModelBone rEngineBone;
        ModelBone rBackWheelBone;
        ModelBone rSteerBone;
        ModelBone rFrontWheelBone;
        ModelBone lEngineBone;
        ModelBone lBackWheelBone;
        ModelBone lFrontWheelBone;
        ModelBone lSteerBone;
        ModelBone hatchBone;

        Matrix turretTransform;
        Matrix canonTransform;
        Matrix rEngineTransform;
        Matrix rBackWheelTranform;
        Matrix rSteerTransform;
        Matrix rFrontWheelTransform;
        Matrix lEngineTransform;
        Matrix lBackWheelTransform;
        Matrix lFrontWheelTransform;
        Matrix lSteerTransform;
        Matrix hatchTransform;

        float turretAngle = 0.0f;
        float canonAngle = 0.01f;
        float steerAngle = 0.01f;

        //Yaw Pitch Roll  and other parameters
        float yaw = 0.01f;
        float pitch = 1.0f;
        float roll = 1.0f;
        Vector3 directionBase = Vector3.UnitX;

        Matrix[] boneTransforms;

        //Componentes do tanque
        Vector3 positionTank;
        float speedTank;
        Vector3 directionTank;
        Vector3 normalTank;

        public Vector3 PositionTank
        {
            get { return positionTank; }
            set { positionTank = value; }
        }

        public float SpeedTank
        {
            get { return speedTank; }
            set { speedTank = value; }
        }

        public Vector3 DirectionTank
        {
            get { return directionTank; }
            set { directionTank = value; }
        }

        public Vector3 NormalTank
        {
            get { return normalTank; }
            set { normalTank = value; }
        }

        public TankClass(GraphicsDevice device, ContentManager content, ClsBattlefield terreno)
        {
            modelTank = content.Load<Model>("tank");

            world = terreno.matrixTerreno;

            effect = new BasicEffect(device);
            view = Matrix.CreateLookAt(new Vector3(1.0f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 2.0f, 1000.0f);

            turretBone = modelTank.Bones["turret_geo"];
            canonBone = modelTank.Bones["canon_geo"];
            rEngineBone = modelTank.Bones["r_engine_geo"];
            lEngineBone = modelTank.Bones["l_engine_geo"];
            rBackWheelBone = modelTank.Bones["r_back_wheel_geo"];
            lBackWheelBone = modelTank.Bones["l_back_wheel_geo"];
            rSteerBone = modelTank.Bones["r_steer_geo"];
            lSteerBone = modelTank.Bones["l_steer_geo"];
            rFrontWheelBone = modelTank.Bones["r_front_wheel_geo"];
            lFrontWheelBone = modelTank.Bones["l_front_wheel_geo"];
            hatchBone = modelTank.Bones["hatch_geo"];

            turretTransform = turretBone.Transform;
            canonTransform = canonBone.Transform;
            rEngineTransform = rEngineBone.Transform;
            lEngineTransform = lEngineBone.Transform;
            rBackWheelTranform = rBackWheelBone.Transform;
            lBackWheelTransform = rBackWheelBone.Transform;
            rSteerTransform = rSteerBone.Transform;
            lSteerTransform = lSteerBone.Transform;
            rFrontWheelTransform = rFrontWheelBone.Transform;
            lFrontWheelTransform = lFrontWheelBone.Transform;
            hatchTransform = hatchBone.Transform;

            boneTransforms = new Matrix[modelTank.Bones.Count];

            directionTank = Vector3.UnitX;

            //Inicialização do tanque
            PositionTank = new Vector3(64.0f, terreno.Interpolacao(PositionTank.X, PositionTank.Z), 64.0f);
            NormalTank = terreno.GetNormals((int)PositionTank.X, (int)PositionTank.Z);
            DirectionTank = Vector3.UnitX;
            SpeedTank = 0.5f;
        }

        public void UpdateTankStuff(KeyboardState keyboard, ClsBattlefield terreno)
        {
            //Controlo da torre
            if (keyboard.IsKeyDown(Keys.Left))
                turretAngle += MathHelper.ToRadians(yaw);
            if (keyboard.IsKeyDown(Keys.Right))
                turretAngle -= MathHelper.ToRadians(yaw);
            if (keyboard.IsKeyDown(Keys.Up))
                canonAngle -= MathHelper.ToRadians(yaw);
            if (keyboard.IsKeyDown(Keys.Down))
                canonAngle += MathHelper.ToRadians(yaw);

            //Controlo do movimento
            if (keyboard.IsKeyDown(Keys.A))
            {
                directionTank = Vector3.Transform(directionTank, Matrix.CreateRotationY(yaw));
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                directionTank = Vector3.Transform(directionTank, Matrix.CreateRotationY(-yaw));
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                positionTank = positionTank - directionTank * speedTank;
                positionTank.Y = terreno.Interpolacao(positionTank.X, positionTank.Z);
                //normalTank = terreno.GetNormals((int)positionTank.X, (int)positionTank.Z);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                positionTank = positionTank + directionTank * speedTank;
                positionTank.Y = terreno.Interpolacao(positionTank.X, positionTank.Z);
                //normalTank = terreno.GetNormals((int)positionTank.X, (int)positionTank.Z);
            }
        }

        public void Draw(Matrix camview, ClsBattlefield terreno)
        {
            Matrix translation = Matrix.CreateTranslation(positionTank.X, terreno.Interpolacao(positionTank.X, positionTank.Z), positionTank.Z);
            Matrix rotation = Matrix.CreateRotationY(yaw);

            normalTank = terreno.GetNormals((int)positionTank.X, (int)positionTank.Z);
            //Matrix rotation = Matrix.Identity;
            rotation.Up = normalTank;
            rotation.Forward = directionTank;
            rotation.Right = Vector3.Cross(normalTank, directionTank);
            world = rotation * translation;

            modelTank.Root.Transform = Matrix.CreateScale(0.005f) * world;

            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            canonBone.Transform = Matrix.CreateRotationX(canonAngle) * canonTransform;

            //lEngineBone.Transform = Matrix.CreateTranslation(positionTank) * lEngineTransform;
            //rEngineBone.Transform = Matrix.CreateTranslation(positionTank) * rEngineTransform;
            //hatchBone.Transform = Matrix.CreateTranslation() * hatchTransform;

            modelTank.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in modelTank.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camview;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model 
                mesh.Draw();
            }
        }
    }
}
