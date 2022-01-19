using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PTG.graphics
{
    public class Camera
    {
        public Matrix View { get; private set; }
        public Matrix Projection { get; }

        private Vector3 cameraPosition;
        private Vector3 cameraDirection;
        private readonly Vector3 cameraUp;

        float speed = 0.5f;
        float rotateSpeed = 0.005f;

        public Camera(Vector3 pos, Vector3 target, Vector3 up)
        {
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1200f / 800f, 1, 10000);
        }

        public void Initialize()
        {
            Mouse.SetPosition(600, 400);
        }

        public void Update(GameTime gameTime)
        {
	        float dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Movement
	        if (Keyboard.GetState().IsKeyDown(Keys.W))
	        {
		        cameraPosition.X += cameraDirection.X * speed * dt;
		        cameraPosition.Z += cameraDirection.Z * speed * dt;
            }

	        if (Keyboard.GetState().IsKeyDown(Keys.S))
	        {
		        cameraPosition.X -= cameraDirection.X * speed * dt;
		        cameraPosition.Z -= cameraDirection.Z * speed * dt;
            }

	        if (Keyboard.GetState().IsKeyDown(Keys.A))
	        {
		        cameraPosition.X += Vector3.Transform(cameraDirection, Matrix.CreateRotationY(MathHelper.PiOver2)).X * speed * dt;
                cameraPosition.Z += Vector3.Transform(cameraDirection, Matrix.CreateRotationY(MathHelper.PiOver2)).Z * speed * dt;
            }

	        if (Keyboard.GetState().IsKeyDown(Keys.D))
	        {
		        cameraPosition.X -= Vector3.Transform(cameraDirection, Matrix.CreateRotationY(MathHelper.PiOver2)).X * speed * dt;
		        cameraPosition.Z -= Vector3.Transform(cameraDirection, Matrix.CreateRotationY(MathHelper.PiOver2)).Z * speed * dt;
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
		        cameraDirection.X += Vector3.Transform(cameraDirection, Matrix.CreateRotationY(1f)).X * speed * rotateSpeed * dt;
                cameraDirection.Z += Vector3.Transform(cameraDirection, Matrix.CreateRotationY(1f)).Z * speed * rotateSpeed * dt;
                cameraDirection.Normalize();
	        }

	        if (Keyboard.GetState().IsKeyDown(Keys.Right))
	        {
		        cameraDirection.X -= Vector3.Transform(cameraDirection, Matrix.CreateRotationY(1f)).X * speed * rotateSpeed * dt;
                cameraDirection.Z -= Vector3.Transform(cameraDirection, Matrix.CreateRotationY(1f)).Z * speed * rotateSpeed * dt;
                cameraDirection.Normalize();
            }

	        if (Keyboard.GetState().IsKeyDown(Keys.Up))
	        {
		        cameraDirection.Y += Vector3.Transform(cameraDirection, -Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), 1f)).Y * speed * rotateSpeed * dt;
            }

	        if (Keyboard.GetState().IsKeyDown(Keys.Down))
	        {
		        cameraDirection.Y -= Vector3.Transform(cameraDirection, -Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), 1f)).Y * speed * rotateSpeed * dt;
            }

            CreateLookAt();
        }

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

    }
}