using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_Projeto
{
    public class Particle
    {
        //Propriedades
        GraphicsDevice graphicsDevice;
        Vector3 position;
        Vector3 direction;
        BasicEffect effect;
        float velocity, life, lifeLimit;
        public bool dead = false;
        Random random;

        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;

        //Construtor
        public Particle(GraphicsDevice graphicsDevice, Vector3 position, Vector3 direction)
        {
            this.graphicsDevice = graphicsDevice;
            this.position = position;
            this.direction = direction;
            this.effect = new BasicEffect(graphicsDevice);

            random = new Random();
            
            velocity = (float)(random.NextDouble() * (0.2 - 0.1) + 0.1);
            lifeLimit = 2;
            life = 0;

            verts = new VertexPositionColor[2];
            verts[0] = new VertexPositionColor(new Vector3(position.X + (float)0.05, position.Y, position.Z), Color.Red);
            verts[1] = new VertexPositionColor(new Vector3(position.X , position.Y , position.Z), Color.Red);

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);
        }


        public void Update(GameTime gameTime)
        {
            if (life < lifeLimit)
            {
                //atualiza a posição dos vertices
                Vector3.Normalize(direction);
                position += direction * velocity;
                life++;
                verts[0] = new VertexPositionColor(new Vector3(position.X + (float)0.05, position.Y, position.Z), Color.Red);
                verts[1] = new VertexPositionColor(new Vector3(position.X , position.Y, position.Z), Color.Red);
            }
            else
            {
                dead = true;
                
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, CameraManager cameraManager)
        {
            
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            if (!dead)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.View = cameraManager.ActiveViewMatrix;
                    effect.Projection = cameraManager.ActiveProjectionMatrix;
                    effect.CurrentTechnique.Passes[0].Apply();
                    effect.VertexColorEnabled = true;
                    pass.Apply();
                    graphicsDevice.DrawUserPrimitives<VertexPositionColor>
                    (PrimitiveType.LineStrip, verts, 0, 1);

                }
            }
        }
    }
}
