using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PTG.graphics;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(new Vector3(-100f, 100f, 0f), new Vector3(0, 0, 0), Vector3.Up);
            camera.Initialize();

            terrain = new Terrain(256, 256, Content.Load<Effect>("effects"), GraphicsDevice);
            terrain.Generate();
        }

        protected override void Update(GameTime gameTime)
        {
            camera.Update(gameTime);

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