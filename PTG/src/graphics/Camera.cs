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
        private Vector3 cameraUp;

        float speed = 0.1f;
        float rotateSpeed = 0.0025f;

        private MouseState prevMouseState;

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

            prevMouseState = Mouse.GetState();
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition += cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= cameraDirection * speed;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;

            // Rotation in the world
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 * rotateSpeed) * (Mouse.GetState().X - prevMouseState.X)));

            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));

            // Reset prevMouseState
            prevMouseState = Mouse.GetState();

            CreateLookAt();
        }

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

    }
}