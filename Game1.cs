using IP3D_Projeto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IP3D_Projeto
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        CameraManager _cameraManager;
        Texture2D heightMapTexture;
        Texture2D tileTexture;

        Terrain terrain;
        public Tank tank;
        public Tank tankCPU;
        Model bullet;
        Collision collision;

        KeyboardState ks;
        MouseState ms;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _cameraManager = new CameraManager(GraphicsDevice, new Vector3(40, 10, 40), terrain, tank);
            tank = new Tank(_graphics.GraphicsDevice, false);
            tankCPU = new Tank(_graphics.GraphicsDevice, true);
            collision = new Collision(tank, tankCPU);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //mapa de alturas
            heightMapTexture = Content.Load<Texture2D>("lh3d1");
            
            //textura do terreno
            tileTexture = Content.Load<Texture2D>("grass");
            //bala
            bullet = Content.Load<Model>("Bullet");

            terrain = new Terrain(GraphicsDevice, heightMapTexture, tileTexture);
            tank.LoadContent(Content);
            tankCPU.LoadContent(Content);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            ks = Keyboard.GetState();
            ms = Mouse.GetState();

            _cameraManager.Update(gameTime, tank, terrain);
            tank.Update(gameTime, GraphicsDevice, terrain, bullet , ks, ms, tankCPU );
            tankCPU.Update(gameTime, GraphicsDevice, terrain, bullet, ks, ms, tank);
            terrain.Update(gameTime);
            collision.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(new Vector3(1.1f, 0.8f, .3f)));

            terrain.Draw(GraphicsDevice, _cameraManager);
            tank.Draw(_cameraManager, terrain);
            tankCPU.Draw(_cameraManager, terrain);

            base.Draw(gameTime);
        }
    }
}