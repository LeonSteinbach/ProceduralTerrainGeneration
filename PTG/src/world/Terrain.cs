using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PTG.src.utility;

namespace PTG.src.world
{
	public class Terrain
	{
		private int width, height;

		private VertexPositionColorNormal[] vertices;
		private ushort[] indices;

		private float[,] heights;

		VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;

		GraphicsDevice device;

		Effect effect;

		public Terrain(int width, int height, Effect effect, GraphicsDevice device)
		{
			this.width = width;
			this.height = height;
			this.effect = effect;
			this.device = device;
		}

		public void Generate()
		{
			SetHeights();

			SetVertices();
			SetIndices();
			CalculateNormals();

			CopyToBuffers();
		}

		public void SetHeights()
		{
			heights = Noise.PerlinNoise(width, height, 6, maximum: 40f);
		}

		public void SetIndices()
		{
			indices = new ushort[6 * (width - 1) * (height - 1)];

			int i = 0;
			for (int y = 0; y < height - 1; y++)
			{
				for (int x = 0; x < width - 1; x++)
				{
					// Create triangles
					indices[i] = (ushort)(x + (y + 1) * width);           // up left
					indices[i + 1] = (ushort)(x + y * width + 1);         // down right
					indices[i + 2] = (ushort)(x + y * width);             // down left
					indices[i + 3] = (ushort)(x + (y + 1) * width);       // up left
					indices[i + 4] = (ushort)(x + (y + 1) * width + 1);   // up right
					indices[i + 5] = (ushort)(x + y * width + 1);         // down right
					i += 6;
				}
			}
		}

		public void SetVertices()
		{
			vertices = new VertexPositionColorNormal[width * height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					vertices[x + y * width].Position = new Vector3(x, heights[x, y], -y);
					vertices[x + y * width].Color = Color.White;
				}
			}
		}

		private void CalculateNormals()
		{
			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Normal = Vector3.Zero;

			for (int i = 0; i < indices.Length / 3; i++)
			{
				int index1 = indices[i * 3];
				int index2 = indices[i * 3 + 1];
				int index3 = indices[i * 3 + 2];

				Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
				Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
				Vector3 normal = Vector3.Cross(side1, side2);

				vertices[index1].Normal += normal;
				vertices[index2].Normal += normal;
				vertices[index3].Normal += normal;
			}

			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Normal.Normalize();
		}

		private void CopyToBuffers()
		{
			vertexBuffer = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vertices);

			indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
			indexBuffer.SetData(indices);
		}

		public void Render(Camera camera, GraphicsDevice graphicsDevice)
		{
			effect.CurrentTechnique = effect.Techniques["Colored"];

			// Transformations
			effect.Parameters["xView"].SetValue(camera.ViewMatrix);
			effect.Parameters["xProjection"].SetValue(camera.ProjectionMatrix);
			effect.Parameters["xWorld"].SetValue(camera.WorldMatrix);

			// Lighting
			Vector3 light = new Vector3(1.0f, -0.1f, -0.1f);
			light.Normalize();

			effect.Parameters["xLightDirection"].SetValue(light);
			effect.Parameters["xAmbient"].SetValue(0.5f);
			effect.Parameters["xEnableLighting"].SetValue(true);

			// Render vertices
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				device.Indices = indexBuffer;
				device.SetVertexBuffer(vertexBuffer);

				device.DrawIndexedPrimitives(
					PrimitiveType.TriangleList,
					0,
					0,
					vertexBuffer.VertexCount);

			}
		}
	}
}
