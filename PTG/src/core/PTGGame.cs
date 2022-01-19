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
        //private SpriteBatch spriteBatch;

        private Camera camera;
        private Terrain terrain;

        private Model tree;

        private int mapSize;

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
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            tree = Content.Load<Model>("models/tree");

            mapSize = 512;

            camera = new Camera(new Vector3(mapSize * 1.5f, mapSize, -mapSize / 2f), new Vector3(mapSize / 2f, 0, -mapSize / 2f), Vector3.Up);
            camera.Initialize();

            terrain = new Terrain(
	            mapSize, mapSize, 
	            Content.Load<Effect>("shaders/effects"), 
	            Content.Load<Texture2D>("images/ice"),
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

            if (Input.IsKeyPressed(Keys.O))
            {
	            terrain.GenerateObjects();
            }

            if (Input.IsKeyHold(Keys.E))
            {
	            terrain.Erode();

                terrain.SetVertices();
                terrain.CalculateNormals();
                terrain.CalculateTangentsAndBinormals();

                terrain.CopyToBuffers();
            }

            if (Input.IsKeyPressed(Keys.P))
            {
                Noise.SaveArrayToPng("gen/map.png", GraphicsDevice, terrain.HeightMap, terrain.Width, terrain.Height, terrain.MaxHeight);
            }

            if (Input.IsKeyPressed(Keys.L))
            {
                terrain.HeightMap = Noise.LoadArrayFromPng("gen/map.png", GraphicsDevice, terrain.MaxHeight);
                //Noise.Amplify(terrain.HeightMap, terrain.Width, terrain.Height, 0, 1, 1);

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

            terrain.Render(camera, tree);

            base.Draw(gameTime);
        }
    }
}