using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Demo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix worldHC;
        Matrix worldH;
        Matrix worldL;
        Matrix worldObj;
        Matrix view;
        Matrix projection;

        Model hovercraft;
        Model ground;
        Model lantern;
        Model staticObj;
        
        Effect specular;
        Effect specularG;

        bool eblinn = false;
        bool gouraud = false;

        Texture2D textureFC;
        Texture2D textureGRO;
        Texture2D textureObj;
        
        Vector3 position = new Vector3(2, 0, 0);
        float angle = (float)Math.PI;
        float height = -0.5f;

        Vector3 cameraPosition = new Vector3(2, 4.5f, 0);
        Vector3 lanternCamera = new Vector3(-2, 1.5f, 5);
        Vector3 mainCamera = new Vector3(2, 4, 0);
        int cp = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            worldHC = Matrix.Identity * Matrix.CreateRotationY((float)Math.PI/2)*Matrix.CreateTranslation(position);
            worldH = Matrix.Identity;
            worldObj = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-3, 0, 0));
            worldL = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-2, 0, 6)); 
            view = createLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(120), 800f/600f, 0.1f, 100f);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            specularG = Content.Load<Effect>("Effects/SpecularG");
            specular = Content.Load<Effect>("Effects/Specular");

            textureObj = Content.Load<Texture2D>("Textures/circus");
            textureGRO = Content.Load<Texture2D>("Textures/ground");
            textureFC = Content.Load<Texture2D>("Textures/hovercraft2");

            hovercraft = Content.Load<Model>("Models/hovercraft2");
            lantern = Content.Load<Model>("Models/lantern");
            ground = Content.Load<Model>("Models/ground");
            staticObj = Content.Load<Model>("Models/circusbig");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private Matrix createLookAt(Vector3 position, Vector3 target, Vector3 upVector)
        {
            upVector.Normalize();
            Vector3 zAxis = position - target;
            zAxis.Normalize();
            Vector3 xAxis = Vector3.Cross(upVector, zAxis);
            xAxis.Normalize();
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

            Matrix m = new Matrix(xAxis.X, xAxis.Y, xAxis.Z, 0.0f,
                yAxis.X, yAxis.Y, yAxis.Z, 0.0f,
                zAxis.X, zAxis.Y, zAxis.Z, 0.0f,
               position.X, position.Y, position.Z, 1);
            return Matrix.Invert(m);
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Vector3 newPosition = position;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                if (height < 1)
                    height += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                height -= 0.01f;
            //blinn
            if (Keyboard.GetState().IsKeyDown(Keys.B))
                eblinn = true;
            //phong
            if (Keyboard.GetState().IsKeyDown(Keys.N))
                eblinn = false;
            //gouraud
            if (Keyboard.GetState().IsKeyDown(Keys.G))
                gouraud = true;
            //phong
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                gouraud = false;
            //self view
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cp = 0;
            //main view
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                cp = 1;
            //lantern view
            if (Keyboard.GetState().IsKeyDown(Keys.L))
                cp = -1;
            if (cp==0)
            {
                cameraPosition = position;
                view = createLookAt(position + new Vector3((float)Math.Cos(-angle), 1, (float)Math.Sin(-angle)), cameraPosition +new Vector3(2*(float)Math.Cos(-angle),1,2*(float)Math.Sin(-angle)), Vector3.UnitY);
            }
            else if(cp==1)
            {
                cameraPosition = mainCamera;
                view = createLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);
            }
            else
            {
                cameraPosition = lanternCamera;
                view = createLookAt(cameraPosition, position, Vector3.UnitY);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                newPosition.X += (float)Math.Cos(angle) / 100;
                newPosition.Z -= (float)Math.Sin(angle) / 100;
           } 
            if(Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                newPosition.X -= (float)Math.Cos(angle) / 100;
                newPosition.Z += (float)Math.Sin(angle) / 100;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.01f;
            }
            if (canMove(newPosition))
                position = newPosition;
            worldHC = Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(position);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private bool canMove(Vector3 newPosition)
        {
            return Math.Abs(newPosition.X)<9 && Math.Abs(newPosition.Z)<9
                && Vector2.Distance(new Vector2(newPosition.X, newPosition.Z), new Vector2(-3, 0)) > 3.8f 
                && Vector2.Distance(new Vector2(newPosition.X, newPosition.Z), new Vector2(-2, 6)) > 1.6f;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);

            // TODO: Add your drawing code here
            if (gouraud)
            {
                DrawModelSpecularG(staticObj, worldObj, view, projection, position + new Vector3(0, 1.5f, 0), textureObj,2);
                DrawModelSpecularG(hovercraft, worldHC, view, projection, new Vector3(-2, 2, 5) - position, textureFC,1);
            }
            else
            {
                DrawModelSpecular(staticObj, worldObj, view, projection, position + new Vector3(0, 1.5f, 0), textureObj,2);
                DrawModelSpecular(hovercraft, worldHC, view, projection, new Vector3(-2, 2, 5) - position, textureFC,1);
            }
            lantern.Draw(worldL, view, projection);
            ground.Draw(Matrix.Identity, view, projection);
            base.Draw(gameTime);
        }
        private void DrawModelSpecular(Model model, Matrix world, Matrix view, Matrix projection, Vector3 position, Texture2D text, int amount)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = specular;
                    specular.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    specular.Parameters["View"].SetValue(view);
                    specular.Parameters["Projection"].SetValue(projection);

                    specular.Parameters["LightPosition"].SetValue(new Vector3[] { new Vector3(-2, 3, 5), position + new Vector3(2 * (float)Math.Cos(-angle), 0.2f, 2 * (float)Math.Sin(-angle))});
                    specular.Parameters["LightDirection"].SetValue(new Vector3[] { new Vector3(0, -1, -1), new Vector3((float)Math.Cos(-angle), height, (float)Math.Sin(-angle))});
                    specular.Parameters["LightColor"].SetValue(new Vector3[] { new Vector3(1, 1, 1), new Vector3(1, 1, 1) });
                    specular.Parameters["LightFalloff"].SetValue(new float[] { 200, 20 });
                    specular.Parameters["ConeAngle"].SetValue(new float[] { 45, 45 });
                    specular.Parameters["ModelTexture"].SetValue(text);
                    specular.Parameters["CameraPosition"].SetValue(cameraPosition);
                    specular.Parameters["isBlinn"].SetValue(eblinn);
                    specular.Parameters["amount"].SetValue(amount);

                }
                mesh.Draw();
            }
        }
        private void DrawModelSpecularG(Model model, Matrix world, Matrix view, Matrix projection, Vector3 position, Texture2D text, int amount)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = specularG;
                    specularG.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    specularG.Parameters["View"].SetValue(view);
                    specularG.Parameters["Projection"].SetValue(projection);

                    specularG.Parameters["LightPosition"].SetValue(new Vector3[] { new Vector3(-2, 3, 5), position + new Vector3(2 * (float)Math.Cos(-angle), 0.2f, 2 * (float)Math.Sin(-angle))});
                    specularG.Parameters["LightDirection"].SetValue(new Vector3[] { new Vector3(0, -1, -1), new Vector3((float)Math.Cos(-angle), height, (float)Math.Sin(-angle))});
                    specularG.Parameters["LightColor"].SetValue(new Vector3[] { new Vector3(1, 1, 1), new Vector3(1, 1, 1) });
                    specularG.Parameters["LightFalloff"].SetValue(new float[] { 200, 20 });
                    specularG.Parameters["ConeAngle"].SetValue(new float[] { 45, 45 });
                    specularG.Parameters["ModelTexture"].SetValue(text);
                    specularG.Parameters["CameraPosition"].SetValue(cameraPosition);
                    specularG.Parameters["isBlinn"].SetValue(eblinn);
                    specular.Parameters["amount"].SetValue(amount);
                }
                mesh.Draw();
            }
        }
        
    }
}
