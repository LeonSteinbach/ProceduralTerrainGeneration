using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PTG.src.world
{
	public struct VertexPositionColorNormal
	{
		public Vector3 Position;
		public Color Color;
		public Vector3 Normal;

		public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), //Position
				new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0), //Color
				new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0) //Normal
			);
	}
}
