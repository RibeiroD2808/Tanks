using IP3D_Projeto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace IP3D_Projeto
{
    class LiveCamera
    {
        //Propriedades
        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        float yaw, pitch;
        Vector3 pos;
        int viewportWidth, viewportHeight;
        float velocity;

        //Construtor
        public LiveCamera(GraphicsDevice device, Vector3 position)
        {
            this.pos = position;
            velocity = 100f;

            this.viewportWidth = device.Viewport.Width;
            this.viewportHeight = device.Viewport.Height;

            Matrix rot = Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f);
            Vector3 direcao = Vector3.Transform(Vector3.UnitX, rot);
            Vector3 target = pos + direcao;

            float aspectRatio = (float)device.Viewport.Width /
                                       device.Viewport.Height;

            viewMatrix = Matrix.CreateLookAt(
                               pos,
                               target,
                               Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45.0f),
                               aspectRatio, 0.1f, 1000.0f);
        }

        public void Update(GameTime gameTime, Terrain terreno)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            float grausPorPixelX = 0.2f; //sensibilidade horizontal
            float grausPorPixelY = 0.1f; //sensibilidade vertical
            int deltaX = ms.Position.X - viewportWidth / 2;
            int deltaY = ms.Position.Y - viewportHeight / 2;

            yaw -= deltaX * MathHelper.ToRadians(grausPorPixelX);
            pitch += deltaY * MathHelper.ToRadians(grausPorPixelY);

            //Vector base aponta para z negativo
            Vector3 vectorBase = -Vector3.UnitZ;

            //direção horizontal
            Vector3 direcaoH = Vector3.Transform(vectorBase, Matrix.CreateRotationY(yaw));

            Vector3 direita = Vector3.Cross(direcaoH, Vector3.UnitY);
            Matrix rotPitch = Matrix.CreateFromAxisAngle(direita, pitch);
            Vector3 direcao = Vector3.Transform(direcaoH, rotPitch);

            //Descobre a tecla premida e movimenta a camera
            if (kb.IsKeyDown(Keys.NumPad8))
            {
                pos += direcao * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.NumPad5))
            {
                pos -= direcao * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.NumPad6))
            {
                pos += direita * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.NumPad4))
            {
                pos += -direita * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (kb.IsKeyDown(Keys.NumPad7))
            {
                pos += Vector3.Up * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (kb.IsKeyDown(Keys.NumPad1))
            {
                pos -= Vector3.Up * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }



            Vector3 target = pos + direcao;
            viewMatrix = Matrix.CreateLookAt(
                               pos,
                               target,
                               Vector3.Up);

            Mouse.SetPosition(viewportWidth / 2, viewportHeight / 2);
        }
    }
}
