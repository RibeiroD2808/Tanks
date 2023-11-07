using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace IP3D_Projeto
{
    public class Terrain
    {
        //vertex buffer e indice
        VertexBuffer vBuffer;
        IndexBuffer iBuffer;
        int vertexCount;
        int indexCount;
        //efeito
        BasicEffect effect;
        //guarda as altura do mapa 
        public float[] heightsMap;
        VertexPositionNormalTexture[] vertices;
        // terreno largura e comprimento
        public int terrainWidth, terrainHeight;
        //guarda as normais
        //List<Vector3> normals;

        /// <summary>
        /// Construtor
        /// </summary>
        public Terrain(GraphicsDevice device, Texture2D heightMapTexture, Texture2D tileTexture)
        {
            //  efeito basico
            effect = new BasicEffect(device);

            float aspectRatio = (float)device.Viewport.AspectRatio;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 2000.0f);
            effect.World = Matrix.Identity;

            effect.LightingEnabled = true;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = tileTexture;



            //  Cria os eixos 3D
            CreateGeometry(device, heightMapTexture);
        }


        /// <summary>
        /// // Usa o tom de cor dos pixeis da textura para guardar as alturas do terreno
        /// </summary>
        private void CreateGeometry(GraphicsDevice device, Texture2D heightMapTexture)
        {

            Color[] heightColors;
            heightColors = new Color[heightMapTexture.Width * heightMapTexture.Height];

            //guarda as cores da textura no array heightColors
            heightMapTexture.GetData<Color>(heightColors);

            heightsMap = new float[heightMapTexture.Width * heightMapTexture.Height];

            // Escala do terreno
            float verticalScale = 0.03f;

            //Transformar heightColors em heightsMap (calcular as alturas)
            for (int i = 0; i < heightMapTexture.Width * heightMapTexture.Height; i++)
            {
                heightsMap[i] = heightColors[i].R * verticalScale;
            }

            terrainWidth = heightMapTexture.Width;
            terrainHeight = heightMapTexture.Height;

            vertexCount = terrainWidth * terrainHeight;
            vertices = new VertexPositionNormalTexture[vertexCount];

            //Cria todos os vectores para construção do terreno
            for (int z = 0; z < terrainHeight; z++)
            {
                for (int x = 0; x < terrainWidth; x++)
                {
                    int i;
                    i = z * terrainWidth + x;

                    float h, u, v;
                    h = heightsMap[i];
                    u = x % 2;
                    v = z % 2;

                    vertices[i] = new VertexPositionNormalTexture(
                        new Vector3(x, h, z),
                        Vector3.Up,
                        new Vector2(u, v));
                }
            }

            short[] indices;
            indexCount = (terrainWidth - 1) * 2 * terrainHeight;
            indices = new short[indexCount];

            // guarda os vertices de forma a poderem ser desenhados em Triangle Strip
            for (int strip = 0; strip < terrainWidth - 1; strip++)
            {
                for (int linha = 0; linha < terrainHeight; linha++)
                {
                    indices[(strip * 2) * (terrainHeight) + 2 * linha + 0] = (short)(strip + (linha * terrainWidth) + 0);
                    indices[(strip * 2) * (terrainHeight) + 2 * linha + 1] = (short)(strip + (linha * terrainWidth) + 1);
                }
            }

            //calcula as normais de cada vertice
            for (int i = 1; i < indices.Length - 2; i++)
            {

                Vector3 v0 = vertices[indices[i]].Position, v1 = vertices[indices[i + 1]].Position, v2 = vertices[indices[i + 2]].Position;

                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);

                // muda a direçao do vetor
                normal *= -1;

                if (i % 2 != 0)
                    normal *= -1;

                normal.Normalize();
                vertices[indices[i]].Normal += normal;
                vertices[indices[i + 1]].Normal += normal;
                vertices[indices[i + 2]].Normal += normal;

            }

            vBuffer = new VertexBuffer(
            device,
            typeof(VertexPositionNormalTexture),
            vertexCount,
            BufferUsage.None);
            vBuffer.SetData<VertexPositionNormalTexture>(vertices);

            iBuffer = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);
            iBuffer.SetData<short>(indices);



        }

        // calcula a altura do terreno na posiçao do tanque
        public float GetHeigh(Vector3 vector)
        {
            float distX1, distX2;
            float distZ1, distZ2;
            float yA, yB, yC, yD;
            float y1, y2;

            distX1 = vector.X - (float)Math.Floor(vector.X);
            distX2 = (float)Math.Ceiling(vector.X) - vector.X;
            distZ1 = vector.Z - (float)Math.Floor(vector.Z);
            distZ2 = (float)Math.Ceiling(vector.Z) - vector.Z;

            yA = GetVector((float)Math.Floor(vector.X), (float)Math.Floor(vector.Z)).Position.Y;
            yB = GetVector((float)Math.Ceiling(vector.X), (float)Math.Floor(vector.Z)).Position.Y;
            yC = GetVector((float)Math.Floor(vector.X), (float)Math.Ceiling(vector.Z)).Position.Y;
            yD = GetVector((float)Math.Ceiling(vector.X), (float)Math.Ceiling(vector.Z)).Position.Y;

            y1 = (yA * distX2) + (yB * distX1);
            y2 = (yC * distX2) + (yD * distX1);

            vector.Y = ((y1 * distZ2) + (y2 * distZ1));

            return vector.Y;
        }


       //calcula a normal inclinaçao do tanque
        public Vector3 GetNormal(float x, float z)
        {
            float xLeft = (float)Math.Floor(x);
            float xRight = xLeft + 1;
            float zUp = (float)Math.Floor(z);
            float zDown = zUp + 1;

            float distX1 = x - xLeft;
            float distX2 = xRight - x;                                                                   
            float distZ1 = z - zUp;
            float distZ2 = zDown - z;

            Vector3 normalA = GetVector(xLeft, zUp).Normal;
            Vector3 normalB = GetVector(xRight, zUp).Normal;
            Vector3 normalC = GetVector(xRight, zDown).Normal;
            Vector3 normalD = GetVector(xLeft, zDown).Normal;

            Vector3 Top = normalA * distX2 + normalB * distX1;
            Vector3 Bot = normalD * distX2 + normalC * distX1;

            return Vector3.Normalize(Top * distZ2 + Bot * distZ1);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GraphicsDevice device, CameraManager cameraManager)
        {
            effect.World = Matrix.Identity;
            effect.Projection = cameraManager.ActiveProjectionMatrix;
            effect.View = cameraManager.ActiveViewMatrix;

            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vBuffer);
            device.Indices = iBuffer;

            for (int strip = 0; strip < terrainWidth - 1; strip++)
            {
                // Luzes
                SetLighting();

                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,
                0, //onde se começa
                (strip) * (2 * terrainHeight), 
                terrainHeight * 2 - 2); 
            }

        }

        //Devolve a altura do terreno(y) com as variaveis x e z
        public VertexPositionNormalTexture GetVector(float x, float z)
        {
            float y;

            y = z * terrainWidth + x;

            return vertices[(int)y];
        }

        //Luzes
        private void SetLighting()
        {
            effect.LightingEnabled = true;
            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
            effect.DirectionalLight0.Enabled = true;

            effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);

            effect.DiffuseColor = new Vector3(2.5f, 1.9f, 1.2f);
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.4f, 0.2f, 0.2f);

            effect.SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            effect.DirectionalLight0.SpecularColor = new Vector3(0.4f, 0.2f, 0.2f);

            Vector3 lightDirection = new Vector3(.5f, -0.5f, 1.0f);
            lightDirection.Normalize();
            effect.DirectionalLight0.Direction = lightDirection;

            effect.SpecularPower = 40;
            effect.Alpha = 0.8f;
        }
    }
}