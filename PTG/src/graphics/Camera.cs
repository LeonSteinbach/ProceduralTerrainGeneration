using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PTG.src.world
{
	public class Camera
	{
		public Matrix ViewMatrix { private set; get; }
		public Matrix ProjectionMatrix { private set; get; }
		public Matrix WorldMatrix { private set; get; }

		Vector3 speed;
		Vector3 rotation;

		public Camera(Vector3 position, Vector3 target, Vector3 speed, Vector3 worldPosition)
		{
			this.speed = speed;
			rotation = speed * 0.02f;

			ViewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
			ProjectionMatrix = Matrix.CreatePerspective(1f, 1f, 1.0f, 1000.0f);
			WorldMatrix = Matrix.CreateTranslation(worldPosition);
		}

		public void Update(GameTime gameTime)
		{
			Vector3 newPosition = Vector3.Zero;
			Vector3 newRotation = Vector3.Zero;

			KeyboardState key = Keyboard.GetState();

			float delta = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

			// Movement
			if (key.IsKeyDown(Keys.A)) newPosition.X = +speed.X * delta;
			if (key.IsKeyDown(Keys.D)) newPosition.X = -speed.X * delta;
			if (key.IsKeyDown(Keys.F)) newPosition.Y = +speed.Y * delta;
			if (key.IsKeyDown(Keys.R)) newPosition.Y = -speed.Y * delta;
			if (key.IsKeyDown(Keys.W)) newPosition.Z = +speed.Z * delta;
			if (key.IsKeyDown(Keys.S)) newPosition.Z = -speed.Z * delta;

			// Rotation
			if (key.IsKeyDown(Keys.Up))		newRotation.X = -rotation.X * delta;
			if (key.IsKeyDown(Keys.Down))	newRotation.X = +rotation.X * delta;
			if (key.IsKeyDown(Keys.Left))	newRotation.Y = -rotation.Y * delta;
			if (key.IsKeyDown(Keys.Right))	newRotation.Y = +rotation.Y * delta;

			ViewMatrix = 
				ViewMatrix * 
				Matrix.CreateRotationX(newRotation.X) * 
				Matrix.CreateRotationY(newRotation.Y) * 
				Matrix.CreateTranslation(newPosition);
		}
	}
}
