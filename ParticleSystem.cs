using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_Projeto
{
    public class ParticleSystem
    {

        GraphicsDevice graphicsDevice;
        int particleCount;
        List<Particle> particles;

        public ParticleSystem(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            
            particleCount = 20;
            particles = new List<Particle>();
            
        }

        public void CreateParticle(Vector3 position, Vector3 direction)
        {
            Vector3 plus;
            Random random = new Random();
            for (int i = 0; i < particleCount; i++)
            {
                plus = new Vector3(random.Next(1, 1), random.Next(0, 2), random.Next(0, 0));
                direction = direction *2;
                Vector3.Normalize(direction);
                particles.Add(new Particle(graphicsDevice, position, direction));
                
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var particle in particles)
            {
                particle.Update(gameTime);
            }

        }

        public void Draw(CameraManager cameraManager)
        {
            foreach (var particle in particles)
            {
                particle.Draw(graphicsDevice, cameraManager);
            }
        }
    }
}
