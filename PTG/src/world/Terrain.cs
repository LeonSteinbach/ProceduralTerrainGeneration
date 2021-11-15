using System;
using System.Collections.Generic;
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
		private List<int>[,] erosionBrushIndices;
		private List<float>[,] erosionBrushWeights;

		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;

		private readonly GraphicsDevice device;

		private readonly Effect effect;
		private readonly Texture2D texture0;
		private readonly Texture2D texture1;
		private readonly Texture2D texture2;
		private readonly Texture2D texture3;

		public Terrain(int width, int height, Effect effect, Texture2D texture0, Texture2D texture1, Texture2D texture2, Texture2D texture3, GraphicsDevice device)
		{
			this.width = width;
			this.height = height;
			this.effect = effect;
			this.texture0 = texture0;
			this.texture1 = texture1;
			this.texture2 = texture2;
			this.texture3 = texture3;
			this.device = device;
		}

		public void Generate()
		{
			SetHeights();
			//InitializeBrush();

			SetVertices();
			SetIndices();
			CalculateNormals();

			CopyToBuffers();
		}

		public void SetHeights()
		{
			heightMap = Noise.PerlinNoise(width, height, 12, maximum: width / 2f);

			//SetWaterLevel(100);
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

		public void Erode()
		{
			int numIterations = 100000;
			int maxLifetime = 100;
			float inertia = 0.05f;
			float sedimentCapacityFactor = 4f;
			float minSedimentCapacity = 0.01f;
			float gravity = 0.1f;
			float evaporateSpeed = 0.01f;
			float depositSpeed = 0.1f;
			float erodeSpeed = 1.3f;

			for (int iteration = 0; iteration < numIterations; iteration++)
			{
				Vector2 pos = new Vector2(
					RandomHelper.RandFloat() * (width - 1), 
					RandomHelper.RandFloat() * (height - 1));
				Vector2 dir = Vector2.Zero;
				
				float speed = 1f;
				float water = 1f;
				float sediment = 0f;

				for (int lifetime = 0; lifetime < maxLifetime; lifetime++)
				{
					Point node = new Point((int) pos.X, (int) pos.Y);
					Vector2 offset = new Vector2(pos.X - node.X, pos.Y - node.Y);

					if (node.X < 0 || node.X >= width - 1 || node.Y < 0 || node.Y >= height - 1)
						break;

					// Calculate height and gradient using linear interpolation
					Vector2 gradient = CalculateGradient(pos);
					float posHeight = CalculateHeight(pos);

					// Update the droplet's direction and position
					dir.X = gradient.X == 0 && gradient.Y == 0 ? 0 : dir.X * inertia - gradient.X * (1 - inertia);
					dir.Y = gradient.X == 0 && gradient.Y == 0 ? 0 : dir.Y * inertia - gradient.Y * (1 - inertia);
					dir.Normalize();

					pos += dir;

					// Break if position is invalid or the droplet is not moving
					if (dir.X == 0 && dir.Y == 0 || 
					    (int) pos.X < 0 || (int) pos.X >= width - 1 || 
					    (int) pos.Y < 0 || (int) pos.Y >= height - 1)
						break;

					// Calculate the droplet's new height and the difference in height
					float newPosHeight = CalculateHeight(pos);
					float deltaHeight = newPosHeight - posHeight;

					// Calculate the droplet's sediment capacity
					float sedimentCapacity = Math.Max(
						-deltaHeight * speed * water * sedimentCapacityFactor,
						minSedimentCapacity);

					// Deposit
					if (sediment > sedimentCapacity || deltaHeight > 0)
					{
						float amountToDeposit = deltaHeight > 0
							? Math.Min(deltaHeight, sediment)
							: (sediment - sedimentCapacity) * depositSpeed;

						if (float.IsNaN(amountToDeposit)) break;
						sediment -= amountToDeposit;
						
						// Add the sediment to the four neighbor nodes using linear interpolation
						heightMap[node.X, node.Y] += amountToDeposit * (1 - offset.X) * (1 - offset.Y);
						heightMap[node.X + 1, node.Y] += amountToDeposit * offset.X * (1 - offset.Y);
						heightMap[node.X, node.Y + 1] += amountToDeposit * (1 - offset.X) * offset.Y;
						heightMap[node.X + 1, node.Y + 1] += amountToDeposit * offset.X * offset.Y;
					}

					// Erode
					else
					{
						float amountToErode = Math.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);
						if (float.IsNaN(amountToErode)) break;
						sediment += amountToErode;

						/*
						// Use erosion brush to erode from all nodes inside the droplet's erosion radius
						for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[node.X, node.Y].Count; brushPointIndex++)
						{
							int nodeIndexX = erosionBrushIndices[node.X, node.Y][brushPointIndex] / width / 2;
							int nodeIndexY = erosionBrushIndices[node.X, node.Y][brushPointIndex] % height;

							if (nodeIndexX >= width - 1 || nodeIndexY >= height - 1)
							{
								break;
							}

							float weighedErodeAmount = amountToErode * erosionBrushWeights[node.X, node.Y][brushPointIndex];
							float deltaSediment = (heightMap[nodeIndexX, nodeIndexY] < weighedErodeAmount) ? heightMap[nodeIndexX, nodeIndexY] : weighedErodeAmount;
							heightMap[nodeIndexX, nodeIndexY] -= deltaSediment;
							sediment += deltaSediment;
						}*/

						
						// Use erosion brush to erode from all nodes inside the droplet's brush radius
						heightMap[node.X, node.Y] -= 0.25f * amountToErode * (1 - offset.X) * (1 - offset.Y);
						heightMap[node.X + 1, node.Y] -= 0.25f * amountToErode * offset.X * (1 - offset.Y);
						heightMap[node.X, node.Y + 1] -= 0.25f * amountToErode * (1 - offset.X) * offset.Y;
						heightMap[node.X + 1, node.Y + 1] -= 0.25f * amountToErode * offset.X * offset.Y;
					}

					// Update speed and water content
					speed = (float) Math.Sqrt(speed * speed + deltaHeight * gravity);
					water *= 1 - evaporateSpeed;
				}
			}
		}

		private void InitializeBrush()
		{
			int radius = 8;

			erosionBrushIndices = new List<int>[width, height];
			erosionBrushWeights = new List<float>[width, height];

			int[] xOffsets = new int[radius * radius * 4];
			int[] yOffsets = new int[radius * radius * 4];
			float[] weights = new float[radius * radius * 4];
			float weightSum = 0;
			int addIndex = 0;

			for (int j = 0; j < height; j++)
			{
				for (int i = 0; i < width; i++)
				{
					radius = RandomHelper.RandInt(8, 8);

					int centreX = i;
					int centreY = j;

					if (centreY <= radius || centreY >= height - radius || centreX <= radius + 1 ||
					    centreX >= width - radius)
					{
						weightSum = 0;
						addIndex = 0;
						for (int y = -radius; y <= radius; y++)
						{
							for (int x = -radius; x <= radius; x++)
							{
								float sqrDst = x * x + y * y;
								if (sqrDst < radius * radius)
								{
									int coordX = centreX + x;
									int coordY = centreY + y;

									if (coordX >= 0 && coordX < width && coordY >= 0 && coordY < height)
									{
										float weight = 1 - (float)Math.Sqrt(sqrDst) / radius;
										weightSum += weight;
										weights[addIndex] = weight;
										xOffsets[addIndex] = x;
										yOffsets[addIndex] = y;
										addIndex++;
									}
								}
							}
						}
					}

					int numEntries = addIndex;
					erosionBrushIndices[i, j] = new List<int>(numEntries);
					erosionBrushWeights[i, j] = new List<float>(numEntries);

					for (int n = 0; n < numEntries; n++)
					{
						erosionBrushIndices[i, j].Add((yOffsets[n] + centreY) * width + xOffsets[n] + centreX);
						erosionBrushWeights[i, j].Add(weights[n] / weightSum);
					}
				}
			}
		}

		private Vector2 CalculateGradient(Vector2 pos)
		{
			Point node = new Point((int) pos.X, (int) pos.Y);
			Vector2 offset = new Vector2(pos.X - node.X, pos.Y - node.Y);

			float heightNw = heightMap[node.X, node.Y];
			float heightNe = heightMap[node.X + 1, node.Y];
			float heightSw = heightMap[node.X, node.Y + 1];
			float heightSe = heightMap[node.X + 1, node.Y + 1];

			Vector2 gradient = new Vector2(
				(heightNe - heightNw) * (1 - offset.Y) + (heightSe - heightSw) * offset.Y,
				(heightSw - heightNw) * (1 - offset.X) + (heightSe - heightNe) * offset.X);

			return gradient;
		}

		private float CalculateHeight(Vector2 pos)
		{
			Point node = new Point((int) pos.X, (int) pos.Y);
			Vector2 offset = new Vector2(pos.X - node.X, pos.Y - node.Y);

			float heightNw = heightMap[node.X, node.Y];
			float heightNe = heightMap[node.X + 1, node.Y];
			float heightSw = heightMap[node.X, node.Y + 1];
			float heightSe = heightMap[node.X + 1, node.Y + 1];

			float posHeight = heightNw * (1 - offset.X) * (1 - offset.Y) + 
			                  heightNe * offset.X * (1 - offset.Y) +
			                  heightSw * (1 - offset.X) * offset.Y + 
			                  heightSe * offset.X * offset.Y;

			return posHeight;
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
			//effect.CurrentTechnique = effect.Techniques["SeasonColored"];
			effect.CurrentTechnique = effect.Techniques["Textured"];

			// Transformations
			effect.Parameters["xView"].SetValue(camera.View);
			effect.Parameters["xProjection"].SetValue(camera.Projection);
			effect.Parameters["xWorld"].SetValue(Matrix.Identity);

			// Lighting
			Vector3 light = new Vector3(1.0f, -0.1f, -0.1f);
			light.Normalize();

			effect.Parameters["xLightDirection"].SetValue(light);
			effect.Parameters["xAmbient"].SetValue(0.5f);
			effect.Parameters["xEnableLighting"].SetValue(true);

			// Textures
			effect.Parameters["xTexture0"].SetValue(texture0);
			//effect.Parameters["Texture1"].SetValue(texture1);
			//effect.Parameters["Texture2"].SetValue(texture2);
			//effect.Parameters["Texture3"].SetValue(texture3);

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
