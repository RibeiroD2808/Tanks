using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_Projeto
{
    public class Collision
    {

        Tank tank;
        Tank tankCPU;

        public Collision(Tank tankCPU)
        {
            this.tankCPU = tankCPU;
        }

        public Collision(Tank tank, Tank tankCPU)
        {
            this.tank = tank;
            this.tankCPU = tankCPU;
        }

        public void Update(GameTime gameTime)
        {
            //colisão entre os dois tanques
            if (Vector3.Distance(tank.position, tankCPU.position) <= 2)
            {
                tank.outd = false;
                tankCPU.outd = false;
            }
            else
            {
                tank.outd = true;
                tankCPU.outd = true;
            }
        }
        //Colisão entre a bala e o segundo tanque
        public void BulletCollision(Bullet bullet)
        {
            if (Vector3.Distance(tankCPU.position, bullet.position) <= 2)
            {
                tankCPU.position = new Vector3(10, 10, 10);
                bullet.dead = true;
            }
        }
    }
}
