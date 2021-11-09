using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PTG.utility;
using PTG.graphics;

namespace PTG.world
{
	public class Terrain
	{
		private readonly int width, height;

		private VertexPositionNormalTexture[] vertices;
		private uint[] indices;

		private float[,] heightMap;
		private float[,] particleMap;

		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;

		private readonly GraphicsDevice device;

		private readonly Effect effect;
		private readonly Texture2D texture;

		public Terrain(int width, int height, Effect effect, Texture2D texture, GraphicsDevice device)
		{
			this.width = width;
			this.height = height;
			this.effect = effect;
			this.texture = texture;
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
			heightMap = Noise.PerlinNoise(width, height, 10, maximum: width / 2f);

			particleMap = new float[width, height];

			CreateParticles();

			SetWaterLevel(200);
		}

		private void SetWaterLevel(float level)
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (heightMap[x, y] <= level)
					{
						heightMap[x, y] = level;
					}
				}
			}
		}

		public void CreateParticles()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					// Initialize particle map
					particleMap[x, y] = heightMap[x, y] / width * 2f;
				}
			}
		}

		public void Erode()
		{
			// Erosion iteration
			for (int n = 0; n < 1; n++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						int dx = x;
						int dy = y;
						float diff = 0;

						// Get steepest neighbor
						for (int j = y - 1; j <= y + 1; j++)
						{
							if (j < 0 || j >= height) continue;
							for (int i = x - 1; i <= x + 1; i++)
							{
								if (i < 0 || i >= width) continue;
								if (y == j && x == i) continue;

								float newDiff = heightMap[x, y] - heightMap[i, j];
								if (newDiff >= diff)
								{
									diff = newDiff;
									dx = i;
									dy = j;
								}
							}
						}
						
						if (diff > 1f)
						{
							// Erode
							heightMap[x, y] -= Math.Min(diff, particleMap[x, y]);

							// Deposit
							heightMap[dx, dy] += Math.Min(diff, particleMap[x, y]) * 1f;

							// Update particle map
							particleMap[dx, dy] = Math.Min(diff, particleMap[x, y]);
							particleMap[x, y] = 0;
						}
					}
				}
			}
		}

		public void SetIndices()
		{
			indices = new uint[6 * (width - 1) * (height - 1)];

			int i = 0;
			for (int y = 0; y < height - 1; y++)
			{
				for (int x = 0; x < width - 1; x++)
				{
					// Create triangles
					indices[i] = (uint)(x + (y + 1) * width);           // up left
					indices[i + 1] = (uint)(x + y * width + 1);         // down right
					indices[i + 2] = (uint)(x + y * width);             // down left
					indices[i + 3] = (uint)(x + (y + 1) * width);       // up left
					indices[i + 4] = (uint)(x + (y + 1) * width + 1);   // up right
					indices[i + 5] = (uint)(x + y * width + 1);         // down right
					i += 6;
				}
			}
		}

		public void SetVertices()
		{
			vertices = new VertexPositionNormalTexture[width * height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					vertices[x + y * width].Position = new Vector3(x, heightMap[x, y], -y);
					vertices[x + y * width].TextureCoordinate = 
						new Vector2(MathUtil.Constrain(x, 0, width, 0, 256), MathUtil.Constrain(y, 0, width, 0, 256));
				}
			}
		}

		public void CalculateNormals()
		{
			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Normal = Vector3.Zero;

			for (int i = 0; i < indices.Length / 3; i++)
			{
				uint index1 = indices[i * 3];
				uint index2 = indices[i * 3 + 1];
				uint index3 = indices[i * 3 + 2];

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

		public void CopyToBuffers()
		{
			vertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vertices);
			
			indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
			indexBuffer.SetData(indices);
		}

		public void Render(Camera camera)
		{
			effect.CurrentTechnique = effect.Techniques["SeasonColored"];
			//effect.CurrentTechnique = effect.Techniques["Textured"];

			// Transformations
			effect.Parameters["xView"].SetValue(camera.View);
			effect.Parameters["xProjection"].SetValue(camera.Projection);
			effect.Parameters["xWorld"].SetValue(Matrix.Identity);
			effect.Parameters["xTexture"].SetValue(texture);

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
