using IP3D_Projeto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace IP3D_Projeto
{
    class FollowCamera
    {
        //Propriedades
        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        Vector3 pos;
        int viewportWidth, viewportHeight;

        //Construtor
        public FollowCamera(GraphicsDevice device)
        {

            this.viewportWidth = device.Viewport.Width;
            this.viewportHeight = device.Viewport.Height;

            float aspectRatio = (float)device.Viewport.Width /
                                       device.Viewport.Height;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45.0f),
                               aspectRatio, 0.1f, 1000.0f);
        }

        public void Update(GameTime gameTime,Tank tank)
        {
            pos = tank.position + tank.rotation.Up * 5 + tank.rotation.Forward * 10;
            viewMatrix = Matrix.CreateLookAt(pos, tank.position + tank.rotation.Backward * 5, tank.rotation.Up);

            Mouse.SetPosition(viewportWidth / 2, viewportHeight / 2);
        }
    }
}