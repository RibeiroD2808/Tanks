using IP3D_Projeto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D_Projeto
{
    public class CameraManager
    {
        //Propriedades
        SurfaceCamera surfaceCamera;
        LiveCamera liveCamera;
        FollowCamera followCamera;
        Terrain terreno;

        public Viewport Viewport { get; set; }
        public int ActiveCamera { get; set; }

        public Matrix ActiveViewMatrix { get; set; }
        public Matrix ActiveProjectionMatrix { get; set; }

        //construtor
        public CameraManager(GraphicsDevice device, Vector3 position, Terrain terreno, Tank tank)
        {
            this.terreno = terreno;
            liveCamera = new LiveCamera(device, position);
            surfaceCamera = new SurfaceCamera(device, position);
            followCamera = new FollowCamera(device);

            ActiveCamera = 0;
            ActiveViewMatrix = surfaceCamera.viewMatrix;
            ActiveProjectionMatrix = surfaceCamera.projectionMatrix;
        }

        // muda a camera
        public void Update(GameTime gametime, Tank tank, Terrain terreno)
        {
            NCamera();
            
            switch (ActiveCamera)
            {
                case 0:
                    surfaceCamera.Update(gametime, terreno);
                    ActiveViewMatrix = surfaceCamera.viewMatrix;
                    ActiveProjectionMatrix = surfaceCamera.projectionMatrix;
                    break;

                case 1:
                    liveCamera.Update(gametime, terreno);
                    ActiveViewMatrix = liveCamera.viewMatrix;
                    ActiveProjectionMatrix = liveCamera.projectionMatrix;
                    break;

                case 2:
                    followCamera.Update(gametime, tank);
                    ActiveViewMatrix = followCamera.viewMatrix;
                    ActiveProjectionMatrix = followCamera.projectionMatrix;

                    break;
            }
        }
        //recebe o numero da camera
        private void NCamera()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.F1))
            {
                ActiveCamera = 0;
            }
            else if (kb.IsKeyDown(Keys.F2))
            {
                ActiveCamera = 1;
            }
            else if (kb.IsKeyDown(Keys.F3))
            {
                ActiveCamera = 2;
            }
        }
    }
}

