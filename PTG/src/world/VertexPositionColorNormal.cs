using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PTG.world
{
	public struct VertexPositionColorNormal
	{
		public Vector3 Position { get; set; }
		public Color Color { get; set; }
		public Vector3 Normal { get; set; }

		public static VertexDeclaration VertexDeclaration { get; } = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), //Position
				new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0), //Color
				new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0) //Normal
			);
	}
}
