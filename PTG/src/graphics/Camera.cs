using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace PTG.graphics
{
	public class Camera
	{
		public Matrix View { get; private set; }
		public Matrix Projection { get; }

		private Vector3 cameraPosition;
		private Vector3 cameraDirection;
		private Vector3 cameraRight;
		private Vector3 cameraUp;

		private readonly float speed = 0.2f;
		private readonly float rotateSpeed = 0.001f;

		private float pitch;
		private float yaw;

		public Camera(Vector3 pos, Vector3 target, Vector3 up)
		{
			cameraPosition = pos;
			cameraDirection = target - pos;
			cameraDirection.Normalize();
			cameraRight = Vector3.Cross(cameraDirection, up);
			cameraRight.Normalize();
			cameraUp = Vector3.Cross(cameraRight, cameraDirection);
			CreateLookAt();

			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1200f / 800f, 1, 10000);

			pitch = (float) Math.Asin(cameraDirection.Y);
			yaw = (float) Math.Atan2(cameraDirection.X, cameraDirection.Z);
		}

		public void Initialize()
		{
			Mouse.SetPosition(600, 400);
		}

		public void Update(GameTime gameTime)
		{
			float dt = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

			cameraDirection.Normalize();
			cameraRight.Normalize();

			Vector2 movementDirection = new Vector2(cameraDirection.X, cameraDirection.Z);
			movementDirection.Normalize();

			Vector2 movementRight = new Vector2(cameraRight.X, cameraRight.Z);
			movementRight.Normalize();

			// Movement
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				cameraPosition.X += movementDirection.X * speed * dt;
				cameraPosition.Z += movementDirection.Y * speed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				cameraPosition.X -= movementDirection.X * speed * dt;
				cameraPosition.Z -= movementDirection.Y * speed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				cameraPosition.X -= movementRight.X * speed * dt;
				cameraPosition.Z -= movementRight.Y * speed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				cameraPosition.X += movementRight.X * speed * dt;
				cameraPosition.Z += movementRight.Y * speed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.R))
			{
				cameraPosition.Y += speed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.F))
			{
				cameraPosition.Y -= speed * dt;
			}

			// Rotation
			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				yaw -= rotateSpeed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				yaw += rotateSpeed * dt;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				pitch += rotateSpeed * dt;
				pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Down))
			{
				pitch -= rotateSpeed * dt;
				pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
			}

			cameraDirection.X = (float) (Math.Cos(pitch) * Math.Cos(yaw));
			cameraDirection.Y = (float) Math.Sin(pitch);
			cameraDirection.Z = (float) (Math.Cos(pitch) * Math.Sin(yaw));

			cameraRight = Vector3.Cross(cameraDirection, Vector3.Up);
			cameraUp = Vector3.Cross(cameraRight, cameraDirection);

			cameraDirection.Normalize();
			cameraRight.Normalize();

			CreateLookAt();
		}

		private void CreateLookAt()
		{
			View = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
		}
	}
}
