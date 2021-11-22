using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PTG.graphics;
using PTG.utility;
using PTG.world;

namespace PTG.core
{
    public class PtgGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Camera camera;
        private Terrain terrain;

        public PtgGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "res";
        }

        protected override void Initialize()
        {
            Point monitorSize = new Point(
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            Point windowSize = new Point(1200, 800);

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            if (monitorSize == windowSize)
                graphics.IsFullScreen = true;

            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            Window.IsBorderless = false;
            Window.Title = "Procedural Terrain Generation v1.0.0";
            Window.Position = new Point(
                (int)(monitorSize.X / 2f - windowSize.X / 2f),
                (int)(monitorSize.Y / 2f - windowSize.Y / 2f));

            graphics.PreferredBackBufferWidth = windowSize.X;
            graphics.PreferredBackBufferHeight = windowSize.Y;
            graphics.ApplyChanges();

            GraphicsDevice.DepthStencilState = new DepthStencilState {DepthBufferEnable = true};

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            int mapSize = 512;

            camera = new Camera(new Vector3(-mapSize / 2f, mapSize / 2f, -mapSize / 2f), new Vector3(mapSize / 2f, 0, -mapSize / 2f), Vector3.Up);
            camera.Initialize();

            terrain = new Terrain(
	            mapSize, mapSize * 2, 
	            Content.Load<Effect>("shaders/effects"), 
	            Content.Load<Texture2D>("images/dirt"),
	            Content.Load<Texture2D>("images/rock"),
	            Content.Load<Texture2D>("images/snow"),
	            Content.Load<Texture2D>("images/rock"),
                GraphicsDevice);
            terrain.Generate();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            camera.Update(gameTime);

            if (Input.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }

            if (Input.IsKeyPressed(Keys.N))
            {
                terrain.Generate();
            }

            if (Input.IsKeyHold(Keys.E))
            {
	            terrain.Erode(0);

                terrain.SetVertices();
                terrain.CalculateNormals();
                terrain.CalculateTangentsAndBinormals();

                terrain.CopyToBuffers();
            }

            if (Input.IsKeyHold(Keys.T))
            {
	            terrain.Erode(1);

	            terrain.SetVertices();
	            terrain.CalculateNormals();
	            terrain.CalculateTangentsAndBinormals();

                terrain.CopyToBuffers();
            }

            if (Input.IsKeyReleased(Keys.E))
            {
	            terrain.SetVertices();
	            terrain.CalculateNormals();
	            terrain.CalculateTangentsAndBinormals();

                terrain.CopyToBuffers();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.End();

            terrain.Render(camera);

            base.Draw(gameTime);
        }
    }
}