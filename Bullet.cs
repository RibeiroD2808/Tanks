using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_Projeto
{
    public class Bullet
    {
        //Propriedades
        Collision collision;
        Tank tank;
        public Vector3 position;
        Vector3 direction;
        Model bulletModel;
        BasicEffect effect;
        Matrix world = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f));
        Matrix rotation;
        Matrix[] transforms;
        float velocity, life, lifeLimit;
        public bool dead;

        //Construtor
        public Bullet(GraphicsDevice graphicsDevice, Model bulletModel, Vector3 position, Vector3 direction, Tank tankCPU)
        {
            effect = new BasicEffect(graphicsDevice);
            this.position = position;
            this.direction = direction;
            this.bulletModel = bulletModel;
            collision = new Collision(tankCPU);
            rotation = Matrix.CreateRotationY(MathHelper.ToRadians(0));
            world = rotation * Matrix.CreateScale(.2f);

            transforms = new Matrix[bulletModel.Bones.Count];
            velocity = (float)2;
            life = 0;
            lifeLimit = 100;
            dead = false;
        }

        public void Update(GameTime gameTime)
        {
            if (life < lifeLimit)
            {
                position += direction * velocity;
                life ++;
            }
            else
            {
                dead = true;
            }
            collision.BulletCollision(this);

        }
        public void Draw(CameraManager cameraManager)
        {
            
            bulletModel.Root.Transform = world * rotation * Matrix.CreateTranslation(position);
            effect.World = Matrix.Identity;
            effect.Projection = cameraManager.ActiveProjectionMatrix;
            effect.View = cameraManager.ActiveViewMatrix;

            bulletModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in bulletModel.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = cameraManager.ActiveProjectionMatrix;
                    be.View = cameraManager.ActiveViewMatrix;
                    be.World = transforms[mesh.ParentBone.Index];
                }
                mesh.Draw();
            }
        }
    }
}
