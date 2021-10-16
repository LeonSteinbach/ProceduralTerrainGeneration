using System.Diagnostics;

namespace PTG.src.utility
{
	public class Noise
	{
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

        public static float[,] PerlinNoise(int width, int height, int octaveCount, float maximum = 1f)
        {
            float[,] baseNoise = new float[width, height];
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    baseNoise[x, y] = RandomHelper.RandFloat();
                }
            }

            float[][,] smoothNoise = new float[octaveCount][,];

            float persistence = 0.5f;

            for (int i = 0; i < octaveCount; i++)
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, width, height, i);

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
            for (int i = 0; i < width; i++)
			{
                for (int j = 0; j < height; j++)
				{
                    perlinNoise[i, j] /= totalAmplitude;
                    perlinNoise[i, j] = MathUtil.Constrain(perlinNoise[i, j], 0f, 1f, 0f, maximum);
				}
			}

            return perlinNoise;
        }
    }
}
