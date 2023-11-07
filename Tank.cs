using IP3D_Projeto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace IP3D_Projeto
{
    
    public class Tank
    {
        #region Fields

        public bool outd;
        // Modelo do tanque
        Model tankModel;
        //  Matrizes do modelo
        public Matrix world = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f));  
        private Matrix view;
        private Matrix projection;
        private GraphicsDevice device;

        //lista de balas
        List<Bullet> Bullets;    
        
        //Referencias para os bones
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone hatchBone;
        ModelBone cannonBone;

        //matrix de transformação dos bones
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform; 
        Matrix cannonTransform;
        Matrix hatchBoneTransform;

        // Array que guarda todas as matrizes de tranformaçao dos bones
        Matrix[] boneTransforms;


        // Rotação das rodas
        float wheelRotationValue;
        float steerRotationValue;

        // sistema de particulas
        ParticleSystem particleSystem;
        #endregion

        #region Properties

        // Bool para distinguir os tanques
        // Se CPU for verdadeiro o tanque é controlado pelo jogador. Se for falso é controlado pelo CPU ou por um segundo jogador 
        private bool CPU;
        // Guarda qual o comportamento do segundo tanque. Se for 'O' o segundo tanque segue o primeiro, se for 'P' é controlado por um segundo jogador
        private char CPUMode;

        // posição do tanque
        public Vector3 position;

        //rotaçao do tanque
        public Matrix rotation;

        //velocidade do tanque
        public float velocity = (float)0.15;

        // Contador para o disparo nao ser contínuo
        private int timerToShoot = 0;

        public float WheelRotation
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }

        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }

        #endregion

        public Tank(GraphicsDevice graphicsDevice, bool CPU)
        {
            device = graphicsDevice;
            this.CPU = CPU;
            outd = true;
            //se o tanque for controlado do jogador
            if (!CPU)
            {
                position = new Vector3(10, 10, 40);
                Bullets = new List<Bullet>();
            }
            else
            //se o tanque for controlado pelo CPU
            {
                CPUMode = 'O';
                position = new Vector3(80, 10, 80);
            }
                
            particleSystem = new ParticleSystem(graphicsDevice);
        }

        // Importa o tanque
        public void LoadContent(ContentManager content)
        {
            // Importa o modelo do content
            tankModel = content.Load<Model>("tank");
            // Referencias dos bones
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // Matriz de tranformação 
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchBoneTransform = hatchBone.Transform;


            boneTransforms = new Matrix[tankModel.Bones.Count];

            // Rotaçãp do tanque
            rotation = Matrix.CreateRotationY(MathHelper.ToRadians(0));

            // Define a escala do tanque
            world = rotation * Matrix.CreateScale(.005f);

            // Define as matrizes de projeção e  de view
            Viewport viewport = device.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            view = Matrix.CreateLookAt(
                new Vector3(0, 10, 10),
                Vector3.Zero,
                Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                aspectRatio,
                1,
                100);
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, 
                            Terrain terrain, Model bulletModel, KeyboardState kb, MouseState ms, Tank target)
        {
            // Se o tanque nao for controlado pelo jogador
            if (!CPU )
            {
                if (kb.IsKeyDown(Keys.W) && outd == true)
                {
                    // Move o tanque para a frente à velocidade definida
                    position += rotation.Backward * velocity;
                    
                    // Roda as rodas 
                    wheelRotationValue += .05f;

                    //Adiociona as particulas
                    particleSystem.CreateParticle(boneTransforms[rightBackWheelBone.Index].Translation, Vector3.Normalize(boneTransforms[hatchBone.Index].Forward));
                    particleSystem.CreateParticle(boneTransforms[leftBackWheelBone.Index].Translation, Vector3.Normalize(boneTransforms[hatchBone.Index].Forward));
                }

                if (kb.IsKeyDown(Keys.S) )
                {
                    // Move o tanque para a tras à velocidade definida
                    position -= rotation.Backward * velocity;

                    // Roda as rodas 
                    wheelRotationValue -= .05f;
                }

                if (kb.IsKeyDown(Keys.A) && outd == true)
                {
                    //Roda o tanque para a sua esquerda
                    rotation *= Matrix.CreateRotationY(MathHelper.Pi / 90);
                }

                if (kb.IsKeyDown(Keys.D) && outd == true)
                {
                    //Roda o tanque para a sua direita
                    rotation *= Matrix.CreateRotationY(-MathHelper.Pi / 90);

                }
                if (kb.IsKeyDown(Keys.Space))
                {
                    //Dispara uma bala
                    shoot(graphicsDevice, bulletModel, target);
                }

                // Balas update
                foreach (var bullet in Bullets)
                {
                    bullet.Update(gameTime);
                }
                // Pariuclas update
                particleSystem.Update(gameTime);
            }
            else
            {
                // Muda o estado para o segundo tanque ser controlado pelo CPU
                if (kb.IsKeyDown(Keys.O))
                    CPUMode = 'O';
                // Muda o estado para o segundo tanque ser controlado por um segundo jogador
                if (kb.IsKeyDown(Keys.P))
                    CPUMode = 'P';

                // Usa as letras 'O' e 'P' para mudar o estado do segundo tanque
                switch (CPUMode)
                {
                    case 'O':
                        // Persegue o tanque do jogador
                        if (outd)
                        {
                            chase(target);
                        }
                        
                        break;
                    case 'P':
                        if (kb.IsKeyDown(Keys.I) && outd == true)
                        {
                            // Move o tanque para a frente à velocidade definida
                            position += rotation.Backward * velocity;
                            // Roda as rodas 
                            wheelRotationValue += .05f;}

                        if (kb.IsKeyDown(Keys.K ) && outd == true)
                        {
                            // Move o tanque para a tras à velocidade definida
                            position -= rotation.Backward * velocity;
                            // Roda as rodas 
                            wheelRotationValue -= .05f;
                        }

                        if (kb.IsKeyDown(Keys.J) && outd == true)
                        {
                            //Roda o tanque para a sua esquerda
                            rotation *= Matrix.CreateRotationY(MathHelper.Pi / 90);
                        }

                        if (kb.IsKeyDown(Keys.L) && outd == true)
                        {
                            //Roda o tanque para a sua direita
                            rotation *= Matrix.CreateRotationY(-MathHelper.Pi / 90);
                        }
                        break;
                    default:
                        break;
                }
                
                
            }

            //Restringe o tanque aos limites do terreno
            position.X = Math.Clamp(position.X, 5, terrain.terrainWidth - 5);
            position.Z = Math.Clamp(position.Z, 5, terrain.terrainHeight - 5);

            //calcula a altura do tanque
            position.Y = terrain.GetHeigh(position);
            
            //calcula a rotaçao do tanque através de interpolação bilinear
            rotation.Up = terrain.GetNormal(position.X, position.Z);
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, terrain.GetNormal(position.X, position.Z)));
            rotation.Forward = Vector3.Normalize(Vector3.Cross(terrain.GetNormal(position.X, position.Z), rotation.Right));

            timerToShoot++;


            removeBullets();
        }

        /// <summary>
        /// Remove as balas que chegaram ao fim da vida
        /// </summary>
        void removeBullets()
        {
            //caso a lista de balas seja null
            if (Bullets == null)
            {
                return;
            }
            //remove as balas que chegaram ao fim de vida da lista de balas
            foreach (var bullet in Bullets.ToList())
            {
                if (bullet != null && bullet.dead == true)
                {
                    Bullets.Remove(bullet);
                }
            }
        }

        /// <summary>
        /// Move o segundo tanque para a direçao do primeiro, calculando a direção
        /// </summary>
        void chase(Tank target)
        {
            // Calcula a direcão
            Vector3 direction = (target.position - position);
            direction.Normalize();

            //Roda o tanque para este ficar de frente para o tanque do jogador
            rotation.Forward = - direction;
            // Move o tanque mais lento que a velocidade do primeiro tanque
            position += direction * velocity / 3;
            //Roda as rodas
            wheelRotationValue += .05f;
        }

        /// <summary>
        /// Adiciona uma bala
        /// </summary>
        void shoot(GraphicsDevice graphicsDevice, Model bulletModel, Tank tankCPU)
        {
            if (timerToShoot >= 30)
            {
                
                Bullets.Add(new Bullet(graphicsDevice, bulletModel, boneTransforms[cannonBone.Index].Translation, Vector3.Normalize(boneTransforms[cannonBone.Index].Backward), tankCPU));
                timerToShoot = 0;
            }
            
        }

        /// <summary>
        /// Desenha o tanque
        /// </summary>
        public void Draw(CameraManager cameraManager, Terrain terreno)
        {
            //Utiliza a matriz mundo com a posiçao e rotaçao para calcular a trnsformação do taque
            tankModel.Root.Transform = world * rotation * Matrix.CreateTranslation(position);
            
            // Usa os valores de rotaçao das rodas para calcular as matrizes
            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);

            // calcular a trasformaçao dos bones 
            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // desenha o modelo
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // utiliza a view e projection matriz da atual camera
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = cameraManager.ActiveViewMatrix;
                    effect.Projection = cameraManager.ActiveProjectionMatrix;

                    // Luzes
                    effect.LightingEnabled = true;
                    effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.DirectionalLight0.Enabled = true;

                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

                    effect.DiffuseColor = new Vector3(2.0f, 2.0f, 2.0f);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.4f, 0.2f, 0.2f);

                    effect.SpecularColor = new Vector3(.8f, 1.0f, 1.0f);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.4f, 0.2f, 0.2f);

                    Vector3 lightDirection = new Vector3(1.0f, -1.0f, 1.0f);
                    lightDirection.Normalize();
                    effect.DirectionalLight0.Direction = lightDirection;
                    
                    
                    effect.SpecularPower = 60;
                    effect.Alpha = 1f; 
                }
                mesh.Draw();
            }
            //Desenha as balas e as particulas
            if (!CPU)
            {
                foreach (var bullet in Bullets)
                {
                    bullet.Draw(cameraManager);
                }
                particleSystem.Draw(cameraManager);
            }
        }
    }
}
