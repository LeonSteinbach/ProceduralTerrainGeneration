using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PTG.world
{
	public struct VertexPositionNormalTextureTangentBinormal
	{
		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }
		public Vector2 TextureCoordinate { get; set; }
		public Vector3 Tangent { get; set; }
		public Vector3 Binormal { get; set; }

		public static VertexDeclaration VertexDeclaration { get; } = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
				new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
			);
	}
}
