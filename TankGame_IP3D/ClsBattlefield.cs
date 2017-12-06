using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TankGame_IP3D
{
    class ClsBattlefield
    {
        public VertexPositionNormalTexture[] vertices;
        public VertexPositionNormalTexture v;
        int vertexCount;
        VertexBuffer vertexBuffer;

        int indexCount;
        short[] indices;
        IndexBuffer indexBuffer;

        BasicEffect effect;
        Matrix matrixTerreno = Matrix.Identity;
        public Texture2D alturas;
        Texture2D terreno;

        public Vector3[] vectorNormal;
        int vectorCount = 129540;

        Color c;
        float y;
        float escala = 0.05f;
        public float[] alturasTextura;

        TankClass tanque;

        public ClsBattlefield(GraphicsDevice device, ContentManager content)
        {
            alturas = content.Load<Texture2D>("heightmap");
            terreno = content.Load<Texture2D>("textureTerreno");

            effect = new BasicEffect(device);
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(64.0f, 12.0f, 64.0f), new Vector3(64.0f, 0.0f, 0.0f), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.01f, 10000.0f);
            effect.TextureEnabled = true;
            effect.Texture = alturas;
            effect.Texture = terreno;
            effect.VertexColorEnabled = false;

            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = Color.Brown.ToVector3();
            effect.DirectionalLight0.Direction = new Vector3(1.0f, -0.5f, 0);  

            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            CreateGeometry(device);
        }

        public void CreateGeometry(GraphicsDevice device)
        {
            Color[] texels = new Color[alturas.Height * alturas.Width];
            alturas.GetData<Color>(texels);
            alturasTextura = new float[alturas.Height * alturas.Width];
            vertexCount = alturas.Width * alturas.Height;
            vertices = new VertexPositionNormalTexture[vertexCount];

            for (int x = 0; x < alturas.Width; x++)
            {
                for (int z = 0; z < alturas.Height; z++)
                {
                    Color c = texels[z * alturas.Width + x];
                    float y = c.R * escala;
                    v = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.Up, new Vector2(x % 2, z % 2));
                    vertices[z * alturas.Width + x] = v;
                    alturasTextura[z * alturas.Width + x] = y;
                }
            }
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            //Normais
            vectorNormal = new Vector3[vectorCount];

            //Canto superior esquerdo
            for (int x = 0; x < 1; x++)
            {
                for (int z = 0; z < 1; z++)
                {
                    Vector3 v1 = vertices[(z + 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z + 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[z * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v4 = Vector3.Cross(v1, v2);
                    Vector3 v5 = Vector3.Cross(v2, v3);
                    Vector3 v6 = Vector3.Cross(v3, v1);
                    Vector3.Normalize(v4);
                    Vector3.Normalize(v5);
                    Vector3.Normalize(v6);
                    vectorNormal[z * alturas.Width + x] = (v4 + v5 + v6) / 3;
                }
            }

            //Canto superior direito
            for (int x = alturas.Width - 1; x < alturas.Width; x++)
            {
                for (int z = 0; z < 1; z++)
                {
                    Vector3 v1 = vertices[z * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z + 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[(z + 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v4 = Vector3.Cross(v1, v2);
                    Vector3 v5 = Vector3.Cross(v2, v3);
                    Vector3 v6 = Vector3.Cross(v3, v1);
                    Vector3.Normalize(v4);
                    Vector3.Normalize(v5);
                    Vector3.Normalize(v6);
                    vectorNormal[z * alturas.Width + x] = (v4 + v5 + v6) / 3;
                }
            }
            //Canto inferior esquerdo
            for (int x = 0; x < 1; x++)
            {
                for (int z = alturas.Height - 1; z < alturas.Height; z++)
                {
                    Vector3 v1 = vertices[z * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z - 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[(z - 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v4 = Vector3.Cross(v1, v2);
                    Vector3 v5 = Vector3.Cross(v2, v3);
                    Vector3 v6 = Vector3.Cross(v3, v1);
                    Vector3.Normalize(v4);
                    Vector3.Normalize(v5);
                    Vector3.Normalize(v6);
                    vectorNormal[z * alturas.Width + x] = (v4 + v5 + v6) / 3;
                }
            }

            //Canto inferior direito
            for (int x = alturas.Width - 1; x < alturas.Width; x++)
            {
                for (int z = alturas.Height - 1; z < alturas.Height; z++)
                {
                    Vector3 v1 = vertices[(z - 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z - 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[z * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v4 = Vector3.Cross(v1, v2);
                    Vector3 v5 = Vector3.Cross(v2, v3);
                    Vector3 v6 = Vector3.Cross(v3, v1);
                    Vector3.Normalize(v4);
                    Vector3.Normalize(v5);
                    Vector3.Normalize(v6);
                    vectorNormal[z * alturas.Width + x] = (v4 + v5 + v6) / 3;
                }
            }
            //Aresta superior
            for (int x = 1; x < alturas.Width - 1; x++)
            {
                for (int z = 0; z < 1; z++)
                {
                    Vector3 v1 = vertices[z * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z + 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[(z + 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v4 = vertices[(z + 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v5 = vertices[z * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v6 = Vector3.Cross(v1, v2);
                    Vector3 v7 = Vector3.Cross(v2, v3);
                    Vector3 v8 = Vector3.Cross(v3, v4);
                    Vector3 v9 = Vector3.Cross(v4, v5);
                    Vector3 v10 = Vector3.Cross(v5, v1);

                    Vector3.Normalize(v6);
                    Vector3.Normalize(v7);
                    Vector3.Normalize(v8);
                    Vector3.Normalize(v9);
                    vectorNormal[z * alturas.Width + x] = (v6 + v7 + v8 + v9) / 4;
                }
            }
            //Aresta esquerda
            for (int x = 0; x < 1; x++)
            {
                for (int z = 1; z < alturas.Height - 1; z++)
                {
                    Vector3 v1 = vertices[(z + 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z + 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[z * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v4 = vertices[(z - 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v5 = vertices[(z - 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v6 = Vector3.Cross(v1, v2);
                    Vector3 v7 = Vector3.Cross(v2, v3);
                    Vector3 v8 = Vector3.Cross(v3, v4);
                    Vector3 v9 = Vector3.Cross(v4, v5);
                    Vector3 v10 = Vector3.Cross(v5, v1);

                    Vector3.Normalize(v6);
                    Vector3.Normalize(v7);
                    Vector3.Normalize(v8);
                    Vector3.Normalize(v9);
                    vectorNormal[z * alturas.Width + x] = (v6 + v7 + v8 + v9) / 4;
                }
            }
            //Aresta inferior
            for (int x = 1; x < alturas.Width - 1; x++)
            {
                for (int z = alturas.Height - 1; z < alturas.Height; z++)
                {
                    Vector3 v1 = vertices[z * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z - 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[(z - 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v4 = vertices[(z - 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v5 = vertices[z * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v6 = Vector3.Cross(v1, v2);
                    Vector3 v7 = Vector3.Cross(v2, v3);
                    Vector3 v8 = Vector3.Cross(v3, v4);
                    Vector3 v9 = Vector3.Cross(v4, v5);
                    Vector3 v10 = Vector3.Cross(v5, v1);

                    Vector3.Normalize(v6);
                    Vector3.Normalize(v7);
                    Vector3.Normalize(v8);
                    Vector3.Normalize(v9);
                    vectorNormal[z * alturas.Width + x] = (v6 + v7 + v8 + v9) / 4;
                }
            }
            //Aresta direita
            for (int x = alturas.Width - 1; x < alturas.Width; x++)
            {
                for (int z = 1; z < alturas.Height - 1; z++)
                {
                    Vector3 v1 = vertices[(z - 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z - 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[z * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v4 = vertices[(z + 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v5 = vertices[(z + 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v6 = Vector3.Cross(v1, v2);
                    Vector3 v7 = Vector3.Cross(v2, v3);
                    Vector3 v8 = Vector3.Cross(v3, v4);
                    Vector3 v9 = Vector3.Cross(v4, v5);
                    Vector3 v10 = Vector3.Cross(v5, v1);

                    Vector3.Normalize(v6);
                    Vector3.Normalize(v7);
                    Vector3.Normalize(v8);
                    Vector3.Normalize(v9);
                    vectorNormal[z * alturas.Width + x] = (v6 + v7 + v8 + v9) / 4;
                }
            }
            //Miolo
            for (int x = 1; x < alturas.Width - 1; x++)
            {
                for (int z = 1; z < alturas.Height - 1; z++)
                {
                    Vector3 v1 = vertices[(z + 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v2 = vertices[(z + 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v3 = vertices[z * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v4 = vertices[(z - 1) * alturas.Width + (x + 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v5 = vertices[(z - 1) * alturas.Width + x].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v6 = vertices[(z - 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v7 = vertices[z * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;
                    Vector3 v8 = vertices[(z + 1) * alturas.Width + (x - 1)].Position - vertices[(z * alturas.Width + x)].Position;

                    Vector3 v10 = Vector3.Cross(v1, v2);
                    Vector3 v11 = Vector3.Cross(v2, v3);
                    Vector3 v12 = Vector3.Cross(v3, v4);
                    Vector3 v13 = Vector3.Cross(v4, v5);
                    Vector3 v14 = Vector3.Cross(v5, v6);
                    Vector3 v15 = Vector3.Cross(v6, v7);
                    Vector3 v16 = Vector3.Cross(v7, v8);
                    Vector3 v17 = Vector3.Cross(v8, v1);

                    Vector3.Normalize(v10);
                    Vector3.Normalize(v11);
                    Vector3.Normalize(v12);
                    Vector3.Normalize(v13);
                    Vector3.Normalize(v14);
                    Vector3.Normalize(v15);
                    Vector3.Normalize(v16);
                    Vector3.Normalize(v17);
                    vectorNormal[z * alturas.Width + x] = (v10 + v11 + v12 + v13 + v14 + v15 + v16 + v17) / 8;
                }
            }
            

            for (int x = 0; x < alturas.Width; x++)
            {
                for (int z = 0; z < alturas.Height; z++)
                {
                    Color c = texels[z * alturas.Width + x];
                    float y = c.R * escala;
                    v = new VertexPositionNormalTexture(new Vector3(x, y, z), vectorNormal[z*alturas.Width+x], new Vector2(x % 2, z % 2));
                    vertices[z * alturas.Width + x] = v;
                    alturasTextura[z * alturas.Width + x] = y;
                }
            }
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);


            //Usaremos strips verticais
            indexCount = alturas.Height * 2 * (alturas.Width - 1);
            indices = new short[indexCount];
            for (int ix = 0; ix < alturas.Width - 1; ix++)
            {
                for (int iz = 0; iz < alturas.Height; iz++)
                {
                    indices[2 * iz + 0 + ix * 2 * alturas.Height] = (short)(iz * alturas.Width + ix);
                    indices[2 * iz + 1 + ix * 2 * alturas.Height] = (short)(iz * alturas.Width + 1 + ix);
                }
            }

            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.None);
            indexBuffer.SetData<short>(indices);
        }

        public float Interpolacao(float X, float Z)
        {
            if (X < 0 || X > alturas.Width || Z < 0 || Z > alturas.Height)
            {
                return y;
            }
            else
            {
                VertexPositionNormalTexture v1, v2, v3, v4;

                v1 = vertices[(int)(Z + 0) * alturas.Width + (int)(X + 0)];
                v2 = vertices[(int)(Z + 0) * alturas.Width + (int)(X + 1)];
                v3 = vertices[(int)(Z + 1) * alturas.Width + (int)(X + 0)];
                v4 = vertices[(int)(Z + 1) * alturas.Width + (int)(X + 1)];

                float peso1, peso2;
                float y1, y2, Y;

                peso1 = X - v1.Position.X;
                peso2 = 1 - peso1;

                y1 = alturasTextura[(int)v1.Position.Z * alturas.Width + (int)v1.Position.X] * peso2 + alturasTextura[(int)v2.Position.Z * alturas.Width + (int)(v2.Position.X)] * peso1;

                peso1 = X - v3.Position.X;
                peso2 = 1 - peso1;

                y2 = alturasTextura[(int)v3.Position.Z * alturas.Width + (int)v3.Position.X] * peso2 + alturasTextura[(int)v4.Position.Z * alturas.Width + (int)(v4.Position.X)] * peso1;

                peso1 = Z - v1.Position.Z;
                peso2 = 1 - peso1;

                Y = y1 * peso2 + y2 * peso1;

                return Y;
            }
        }

        public void Draw(GraphicsDevice device, Matrix camView)
        {
            effect.World = matrixTerreno;
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            effect.View = camView;

            for (int ix = 0; ix < (alturas.Width - 1); ix++)
            {
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, ix * alturas.Height * 2, 2 * alturas.Height - 2); //tenta ver este ciclo
            }
        }
    }
}