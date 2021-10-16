using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PTG.src.utility;

namespace PTG.src.world
{
	public class Terrain
	{
		private int width, height;

		private VertexPosition[] vertices;
		private int[] indices;

		private float[,] heights;

		private BasicEffect effect;

		public Terrain(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public void SetEffect(BasicEffect effect)
		{
			this.effect = effect;
		}

		public void Generate()
		{
			SetHeights();
			SetIndices();
			SetVertices();
		}

		public void SetHeights()
		{
			heights = new float[height, width];

			/*
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					heights[y, x] = RandomHelper.RandFloat();
				}
			}
			*/

			heights = Noise.PerlinNoise(width, height, 7, maximum: 70f);
		}

		public void SetIndices()
		{
			indices = new int[6 * (width - 1) * (height - 1)];

			int i = 0;
			for (int y = 0; y < height - 1; y++)
			{
				for (int x = 0; x < width - 1; x++)
				{
					// Create double triangles
					indices[i] = x + (y + 1) * width;			// up left
					indices[i + 1] = x + y * width + 1;			// down right
					indices[i + 2] = x + y * width;				// down left
					indices[i + 3] = x + (y + 1) * width;		// up left
					indices[i + 4] = x + (y + 1) * width + 1;	// up right
					indices[i + 5] = x + y * width + 1;			// down right
					i += 6;
				}
			}
		}

		public void SetVertices()
		{
			vertices = new VertexPosition[width * height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					vertices[x + y * width] = 
						new VertexPosition(new Vector3(x, heights[x, y], -y));
				}
			}
		}

		public void Render(Camera camera, GraphicsDevice graphicsDevice)
		{
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
			effect.World = camera.WorldMatrix;

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				graphicsDevice.DrawUserIndexedPrimitives(
					PrimitiveType.LineList, 
					vertices, 
					0, 
					vertices.Length, 
					indices, 
					0, 
					indices.Length / 3);
			}
		}
	}
}
