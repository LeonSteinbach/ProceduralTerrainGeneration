using Microsoft.Xna.Framework;
using System;

namespace PTG.utility
{
	public static class MathUtil
	{
        public static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        public static float Sigmoid(float x, float t, float u)
        {
            return 1 / (1 + (float)Math.Pow(Math.E, -t * x + u));
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        public static float Distance(Point a, Point b)
        {
            return Distance(a.ToVector2(), b.ToVector2());
        }

        public static float AngleRadians(Vector2 a, Vector2 b)
        {
            return (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        public static float AngleRadians(Point a, Point b)
        {
            return AngleRadians(a.ToVector2(), b.ToVector2());
        }

        public static float RadiansToDegrees(float angle)
        {
            return angle * 180f / (float)Math.PI;
        }

        public static float DegreesToRadians(float angle)
        {
            return angle * (float)Math.PI / 180f;
        }

        public static float Constrain(float x, float inMin, float inMax, float outMin, float outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public static Vector2 IsometricToOrthogonal(Vector2 position)
        {
            return new Vector2((2 * position.Y + position.X) / 2, (2 * position.Y - position.X) / 2);
        }

        public static Vector2 OrthogonalToIsometric(Vector2 position)
        {
            return new Vector2(position.X - position.Y, (position.X + position.Y) / 2);
        }
    }
}
