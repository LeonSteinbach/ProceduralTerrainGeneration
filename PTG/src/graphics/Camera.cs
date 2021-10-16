using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PTG.src.world
{
	public class Camera
	{
		public Matrix ViewMatrix { private set; get; }
		public Matrix ProjectionMatrix { private set; get; }
		public Matrix WorldMatrix { private set; get; }

		Vector3 position;
		Vector3 direction;
		Vector3 movement;
		Vector3 rotation;

		public Camera(Vector3 position, Vector3 direction, Vector3 movement, Vector3 landscapePosition)
		{
			this.position = position;
			this.direction = direction;
			this.movement = movement;
			rotation = movement * 0.02f;

			ViewMatrix = Matrix.CreateLookAt(position, direction, Vector3.Up);
			ProjectionMatrix = Matrix.CreatePerspective(1.2f, 0.9f, 1.0f, 1000.0f);
			WorldMatrix = Matrix.CreateTranslation(landscapePosition);
		}

		public void Update()
		{
			Vector3 tempMovement = Vector3.Zero;
			Vector3 tempRotation = Vector3.Zero;

			KeyboardState key = Keyboard.GetState();

			// Movement
			if (key.IsKeyDown(Keys.A)) tempMovement.X = +movement.X;
			if (key.IsKeyDown(Keys.D)) tempMovement.X = -movement.X;
			if (key.IsKeyDown(Keys.R)) tempMovement.Y = -movement.Y;
			if (key.IsKeyDown(Keys.F)) tempMovement.Y = +movement.Y;
			if (key.IsKeyDown(Keys.W)) tempMovement.Z = -movement.Z;
			if (key.IsKeyDown(Keys.S)) tempMovement.Z = +movement.Z;

			// Rotation
			if (key.IsKeyDown(Keys.Up))		tempRotation.Y = -rotation.Y;
			if (key.IsKeyDown(Keys.Down))	tempRotation.Y = +rotation.Y;
			if (key.IsKeyDown(Keys.Left))	tempRotation.X = -rotation.X;
			if (key.IsKeyDown(Keys.Right))	tempRotation.X = +rotation.X;

			ViewMatrix = ViewMatrix * Matrix.CreateRotationX(tempRotation.X) * Matrix.CreateRotationY(tempRotation.Y) * Matrix.CreateTranslation(tempMovement);

			position += tempMovement;
			direction += tempRotation;
		}

		public void SetEffects(BasicEffect basicEffect)
		{
			basicEffect.View = ViewMatrix;
			basicEffect.Projection = ProjectionMatrix;
			basicEffect.World = WorldMatrix;
		}

		public void Draw(Terrain terrain)
		{
			SetEffects(terrain.basicEffect);
			foreach (EffectPass pass in terrain.basicEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				terrain.Draw();
			}
		}
	}
}
