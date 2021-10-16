using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PTG
{
    public class PTGGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public PTGGame()
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
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();



            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}