using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PTG.utility
{
	public class Noise
	{
		private static void SaveArrayToPng(string filename, GraphicsDevice device, float[,] array, int width, int height, float maxHeight)
		{
			// Convert array to texture
			Color[] colorArray = new Color[width * height];
			for (int y = 0; y < width; y++)
			{
				for (int x = 0; x < height; x++)
				{
					colorArray[x + width * y] = new Color(new Vector3(array[x, y] / maxHeight));
				}
			}

			Texture2D texture = new Texture2D(device, width, height);
            texture.SetData(colorArray);

			// Save texture to png file
			Directory.CreateDirectory("gen");
			Stream stream = File.Create(filename);
			texture.SaveAsPng(stream, width, height);
			stream.Dispose();
			texture.Dispose();
        }

        private static float[,] GenerateSmoothNoise(float[,] baseNoise, int width, int height, int octave)
        {
            float[,] smoothNoise = new float[width, height];

            int samplePeriod = 1 << octave;
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                int sampleI0 = (i / samplePeriod) * samplePeriod;
                int sampleI1 = (sampleI0 + samplePeriod) % width;
                float horizontalBlend = (i - sampleI0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    int sampleJ0 = (j / samplePeriod) * samplePeriod;
                    int sampleJ1 = (sampleJ0 + samplePeriod) % height;
                    float verticalBlend = (j - sampleJ0) * sampleFrequency;

                    float top = MathUtil.Interpolate(baseNoise[sampleI0, sampleJ0], baseNoise[sampleI1, sampleJ0], horizontalBlend);
                    float bottom = MathUtil.Interpolate(baseNoise[sampleI0, sampleJ1], baseNoise[sampleI1, sampleJ1], horizontalBlend);

                    smoothNoise[i, j] = MathUtil.Interpolate(top, bottom, verticalBlend);
                }
            }

            return smoothNoise;
        }

        public static float[,] PerlinNoise(int width, int height, int octaveCount, GraphicsDevice device, float maximum = 1f)
        {
	        float[][,] smoothNoise = new float[octaveCount][,];
            float persistence = 0.5f;

            for (int i = 0; i < octaveCount; i++)
            {
	            float[,] baseNoise = new float[width, height];
	            for (int y = 0; y < width; y++)
	            {
		            for (int x = 0; x < height; x++)
		            {
			            baseNoise[y, x] = RandomHelper.RandFloat();
		            }
	            }

	            // Generate smooth noise
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, width, height, i);

                SaveArrayToPng("gen/smooth_noise_" + i + ".png", device, smoothNoise[i], width, height, 1);
            }

            float[,] perlinNoise = new float[width, height];
            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistence;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
				{
                    for (int j = 0; j < height; j++)
					{
                        perlinNoise[i, j] += smoothNoise[octave][i, j] * amplitude;
					}
				}
            }

            // Normalize
            float min = maximum;
            float max = 0;

            for (int i = 0; i < width; i++)
			{
                for (int j = 0; j < height; j++)
				{
                    perlinNoise[i, j] /= totalAmplitude;

                    if (perlinNoise[i, j] < min) min = perlinNoise[i, j];
                    if (perlinNoise[i, j] > max) max = perlinNoise[i, j];
				}
			}
            
            // Amplify from 0 to maximum
            for (int i = 0; i < width; i++)
            {
	            for (int j = 0; j < height; j++)
	            {
		            perlinNoise[i, j] = MathUtil.Constrain(perlinNoise[i, j], min, max, 0f, maximum);
                }
            }

            SaveArrayToPng("gen/perlin_noise.png", device, perlinNoise, width, height, maximum);

            return perlinNoise;
        }
    }
}
