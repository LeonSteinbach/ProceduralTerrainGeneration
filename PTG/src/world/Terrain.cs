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
		public int Width { get; }
		public int Height { get; }
		public int MaxHeight { get; }

		private readonly bool waterEnabled;
		private readonly int waterLevel;

		private readonly List<Vector3> objects;

		private VertexPositionNormalTextureTangentBinormal[] vertices;
		private int[] indices;

		public float[,] HeightMap { get; set; }

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
			Width = width;
			Height = height;

			this.effect = effect;
			this.texture0 = texture0;
			this.texture1 = texture1;
			this.texture2 = texture2;
			this.texture3 = texture3;
			this.device = device;

			MaxHeight = width / 4;
			waterLevel = width / 12;
			waterEnabled = true;

			objects = new List<Vector3>();
		}

		public void Generate()
		{
			SetHeights();

			GenerateObjects();

			SetVertices();
			SetIndices();
			CalculateNormals();
			CalculateTangentsAndBinormals();

			CopyToBuffers();
		}

		public void SetHeights()
		{
			HeightMap = Noise.PerlinNoise(Width, Height, 8, device, maximum: MaxHeight);
			
			if (waterEnabled)
				SetWaterLevel(waterLevel);
		}

		private void SetWaterLevel(float level)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (HeightMap[x, y] <= level)
					{
						HeightMap[x, y] = level;
					}
				}
			}
		}

		public void GenerateObjects()
		{
			int spacing = 2;
			int biomeSize = 6;
			float biomeFactor = 0.5f;
			float heightFactor = 0.4f;
			float slopeFactor = 0.3f;
			float quantity = 0.6f;

			objects.Clear();

			float[,] noise = Noise.PerlinNoise(Width, Height, biomeSize, device);

			for (int z = RandomHelper.RandInt(0, spacing); z < Height - 1; z += spacing)
			{
				for (int x = RandomHelper.RandInt(0, spacing); x < Width - 1; x += spacing)
				{
					float y = HeightMap[x, z];
					if (y <= 0) y = 0;

					if (waterEnabled && y <= waterLevel + 0.05f) continue;

					int heightRand = RandomHelper.RandInt((int) (y * 0.5f), (int) y);
					float rand = RandomHelper.RandFloat();

					Vector2 gradient = CalculateGradient(new Vector2(x, z));
					float slope = (float) Math.Sqrt(gradient.X * gradient.X + gradient.Y * gradient.Y);

					if (noise[x, z] > biomeFactor || heightRand > MaxHeight * heightFactor || rand > quantity || slope > slopeFactor) continue;
					
					objects.Add(new Vector3(x, y, -z));
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
			float depositSpeed = 1.0f;
			float erodeSpeed = 1.0f;
			float ruggedness = 0.1f;

			for (int iteration = 0; iteration < numIterations; iteration++)
			{
				Vector2 pos = new Vector2(
					RandomHelper.RandFloat() * (Width - 1), 
					RandomHelper.RandFloat() * (Height - 1));
				Vector2 dir = Vector2.Zero;
				
				float speed = 1f;
				float water = 1f;
				float sediment = 0f;

				for (int lifetime = 0; lifetime < maxLifetime; lifetime++)
				{
					Point node = new Point((int) pos.X, (int) pos.Y);
					Vector2 offset = new Vector2(pos.X - node.X, pos.Y - node.Y);

					if (node.X < 0 || node.X >= Width - 1 || node.Y < 0 || node.Y >= Height - 1)
						break;

					// Calculate height and gradient using linear interpolation
					Vector2 gradient = CalculateGradient(pos);
					float posHeight = CalculateHeight(pos);

					// Update the droplet's direction and position
					dir.X += (dir.X * inertia - gradient.X * (1 - inertia)) * ruggedness;
					dir.Y += (dir.Y * inertia - gradient.Y * (1 - inertia)) * ruggedness;
					dir.Normalize();

					pos += dir;

					// Break if position is invalid or the droplet is not moving
					if (Math.Abs(dir.X) < 0.1f && Math.Abs(dir.Y) < 0.1f || 
					    (int) pos.X < 0 || (int) pos.X >= Width - 1 || 
					    (int) pos.Y < 0 || (int) pos.Y >= Height - 1)
						break;

					// Calculate the droplet's new height and the difference in height
					float newPosHeight = CalculateHeight(pos);
					float deltaHeight = newPosHeight - posHeight;

					// Calculate the droplet's sediment capacity
					float sedimentCapacity = Math.Max(
						-deltaHeight * speed * water * sedimentCapacityFactor,
						minSedimentCapacity);

					// Deposit
					if (sediment > sedimentCapacity)
					{
						float amountToDeposit = deltaHeight > 0
							? Math.Min(deltaHeight, sediment)
							: (sediment - sedimentCapacity) * depositSpeed;

						if (float.IsNaN(amountToDeposit)) break;
						sediment -= amountToDeposit;

						// Add the sediment to the four neighbor nodes using linear interpolation
						HeightMap[node.X, node.Y] += 0.25f * amountToDeposit * (1 - offset.X) * (1 - offset.Y);
						HeightMap[node.X + 1, node.Y] += 0.25f * amountToDeposit * offset.X * (1 - offset.Y);
						HeightMap[node.X, node.Y + 1] += 0.25f * amountToDeposit * (1 - offset.X) * offset.Y;
						HeightMap[node.X + 1, node.Y + 1] += 0.25f * amountToDeposit * offset.X * offset.Y;
					}

					// Erode
					else
					{
						float amountToErode = Math.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

						if (float.IsNaN(amountToErode)) break;
						sediment += 0.25f * amountToErode;

						// Use erosion brush to erode from all nodes inside the droplet's brush radius
						HeightMap[node.X, node.Y] -= 0.25f * amountToErode * (1 - offset.X) * (1 - offset.Y);
						HeightMap[node.X + 1, node.Y] -= 0.25f * amountToErode * offset.X * (1 - offset.Y);
						HeightMap[node.X, node.Y + 1] -= 0.25f * amountToErode * (1 - offset.X) * offset.Y;
						HeightMap[node.X + 1, node.Y + 1] -= 0.25f * amountToErode * offset.X * offset.Y;
					}

					// Update speed and water content
					speed = (float) Math.Sqrt(speed * speed + deltaHeight * gravity);
					water *= 1 - evaporateSpeed;
				}
			}
		}

		private Vector2 CalculateGradient(Vector2 pos)
		{
			Point node = new Point((int) pos.X, (int) pos.Y);
			Vector2 offset = new Vector2(pos.X - node.X, pos.Y - node.Y);

			float heightNw = HeightMap[node.X, node.Y];
			float heightNe = HeightMap[node.X + 1, node.Y];
			float heightSw = HeightMap[node.X, node.Y + 1];
			float heightSe = HeightMap[node.X + 1, node.Y + 1];

			Vector2 gradient = new Vector2(
				(heightNe - heightNw) * (1 - offset.Y) + (heightSe - heightSw) * offset.Y,
				(heightSw - heightNw) * (1 - offset.X) + (heightSe - heightNe) * offset.X);

			return gradient;
		}

		private float CalculateHeight(Vector2 pos)
		{
			Point node = new Point((int) pos.X, (int) pos.Y);
			Vector2 offset = new Vector2(pos.X - node.X, pos.Y - node.Y);

			float heightNw = HeightMap[node.X, node.Y];
			float heightNe = HeightMap[node.X + 1, node.Y];
			float heightSw = HeightMap[node.X, node.Y + 1];
			float heightSe = HeightMap[node.X + 1, node.Y + 1];

			float posHeight = heightNw * (1 - offset.X) * (1 - offset.Y) + 
			                  heightNe * offset.X * (1 - offset.Y) +
			                  heightSw * (1 - offset.X) * offset.Y + 
			                  heightSe * offset.X * offset.Y;

			return posHeight;
		}

		public void SetIndices()
		{
			indices = new int[6 * (Width - 1) * (Height - 1)];

			int i = 0;
			for (int y = 0; y < Height - 1; y++)
			{
				for (int x = 0; x < Width - 1; x++)
				{
					// Create triangles
					indices[i] = x + (y + 1) * Width;           // up left
					indices[i + 1] = x + y * Width + 1;         // down right
					indices[i + 2] = x + y * Width;             // down left
					indices[i + 3] = x + (y + 1) * Width;       // up left
					indices[i + 4] = x + (y + 1) * Width + 1;   // up right
					indices[i + 5] = x + y * Width + 1;         // down right
					i += 6;
				}
			}
		}

		public void SetVertices()
		{
			vertices = new VertexPositionNormalTextureTangentBinormal[Width * Height];

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					vertices[x + y * Width].Position = new Vector3(x, HeightMap[x, y], -y);
					vertices[x + y * Width].TextureCoordinate = 
						new Vector2(MathUtil.Constrain(x, 0, Width, 0, 16), MathUtil.Constrain(y, 0, Width, 0, 16));
				}
			}
		}

		public void CalculateNormals()
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

			foreach (var vertex in vertices)
				vertex.Normal.Normalize();
		}

		public void CalculateTangentsAndBinormals()
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 v1 = vertices[i].Position;
				int x = i % Width;

				// Calculate tangent
				if (x < Width - 1)
				{
					Vector3 v2 = vertices[i + 1].Position;
					vertices[i].Tangent = v2 - v1;
				}
				else
				{
					Vector3 v2 = vertices[^1].Position;
					vertices[i].Tangent = v1 - v2;
				}

				// Calculate binormal
				vertices[i].Tangent.Normalize();
				vertices[i].Binormal = Vector3.Cross(vertices[i].Tangent, vertices[i].Normal);
			}
		}

		public void CopyToBuffers()
		{
			vertexBuffer = new VertexBuffer(device, VertexPositionNormalTextureTangentBinormal.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vertices);
			
			indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
			indexBuffer.SetData(indices);
		}

		public void DrawModel(Model model, Vector3 position, Camera camera)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (var effect1 in mesh.Effects)
				{
					var basicEffect = (BasicEffect) effect1;
					basicEffect.AmbientLightColor = new Vector3(1f, 0, 0);
					basicEffect.EnableDefaultLighting();
					basicEffect.World = Matrix.CreateWorld(position, Vector3.UnitZ, Vector3.Up);
					basicEffect.View = camera.View;
					basicEffect.Projection = camera.Projection;
				}

				mesh.Draw();
			}
		}

		public void Render(Camera camera, Model tree)
		{
			//effect.CurrentTechnique = effect.Techniques["Colored"];
			effect.CurrentTechnique = effect.Techniques["Textured"];

			// Data
			//effect.Parameters["MaxHeight"].SetValue((float) maxHeight);
			effect.Parameters["WaterLevel"].SetValue((float) waterLevel);
			effect.Parameters["WaterEnabled"].SetValue(waterEnabled);

			// Transformations
			effect.Parameters["View"].SetValue(camera.View);
			effect.Parameters["Projection"].SetValue(camera.Projection);
			effect.Parameters["World"].SetValue(Matrix.Identity);

			// Lighting
			Vector3 light = new Vector3(1.0f, -0.5f, -0.5f);
			light.Normalize();

			effect.Parameters["LightDirection"].SetValue(light);
			effect.Parameters["Ambient"].SetValue(0.5f);

			// Textures
			effect.Parameters["Texture0"].SetValue(texture0);
			effect.Parameters["Texture1"].SetValue(texture1);
			effect.Parameters["Texture2"].SetValue(texture2);
			effect.Parameters["Texture3"].SetValue(texture3);

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
					vertexBuffer.VertexCount * 2);
			}

			foreach (Vector3 modelPosition in objects)
			{
				DrawModel(tree, modelPosition, camera);
			}
		}
	}
}
