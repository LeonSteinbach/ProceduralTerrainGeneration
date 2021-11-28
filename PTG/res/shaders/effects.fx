//------------------------------------------------------
//--                                                  --
//--		   www.riemers.net                    --
//--   		    Basic shaders                     --
//--		Use/modify as you like                --
//--                                                  --
//------------------------------------------------------

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VertexToPixel
{
	float4 Position   		: POSITION;
	float4 Color			: COLOR0;
	float LightingFactor	: TEXCOORD0;
	float2 TextureCoords	: TEXCOORD1;
	float Height			: TEXCOORD2;
	float Normal			: TEXCOORD3;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

//------- Constants --------

float4x4 View;
float4x4 Projection;
float4x4 World;

float3 LightDirection;
float Ambient;

float MaxHeight;

//------- Texture Samplers --------

Texture Texture0;
sampler TextureSampler0 = sampler_state {
	texture = <Texture0>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture Texture1;
sampler TextureSampler1 = sampler_state {
	texture = <Texture1>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = wrap;
	AddressV = wrap;
};

Texture Texture2;
sampler TextureSampler2 = sampler_state {
	texture = <Texture2>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

Texture Texture3;
sampler TextureSampler3 = sampler_state {
	texture = <Texture3>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = wrap;
	AddressV = wrap;
};

//------- Function --------

float constrain(float value, float min, float max) {
	if (value < min) value = min;
	if (value > max) value = max;
	return value;
}


//------- SeasonColored --------

VertexToPixel ColoredVS(float4 Pos : POSITION, float3 N : NORMAL, float4 C : COLOR)
{
	float4 Red = float4(0.7f, 0.7f, 0.9f, 1.0f);
	float4 Green = float4(0.6f, 0.4f, 0.3f, 1.0f);
	float4 Blue = float4(0.4f, 0.4f, 1.0f, 1.0f);

	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(Pos, preWorldViewProjection);
	Output.Height = Pos[1];

	if (Pos[1] > 0.0f)
	{
		Output.Color = Red;
	}
	else if (Pos[1] <= 130.0f && Pos[1] > 80.0f)
	{
		Output.Color = Green;
	}
	else
	{
		Output.Color = Blue;
	}

	float3 Normal = normalize(mul(normalize(N), World));
	Output.Normal = Normal;
	Output.LightingFactor = dot(Normal, -LightDirection);

	return Output;
}

PixelToFrame ColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + Ambient;

	return Output;
}

technique Colored
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL ColoredVS();
		PixelShader = compile PS_SHADERMODEL ColoredPS();
	}
}


//------- Textured --------

VertexToPixel TexturedVS(float4 Pos : POSITION, float3 N : NORMAL, float3 T : TANGENT, float3 B : BINORMAL, float2 TexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(Pos, preWorldViewProjection);
	Output.TextureCoords = TexCoords;
	Output.Height = Pos[1];

	float3 Normal = normalize(mul(normalize(N), World));
	Output.Normal = Normal;
	Output.LightingFactor = dot(Normal, -LightDirection);

	return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	float hw0 = 0.001f;
	float hw1 = 0.001f;
	float hw2 = 1.001f;
	float hw3 = 0.001f;

	if (abs(PSIn.Normal) > 0.4f) {
		hw0 = 0.001f;
		hw1 = 1.001f;
		hw2 = 0.001f;
		hw3 = 0.001f;
	}

	Output.Color = tex2D(TextureSampler0, PSIn.TextureCoords) * hw0;
	Output.Color += tex2D(TextureSampler1, PSIn.TextureCoords) * hw1;
	Output.Color += tex2D(TextureSampler2, PSIn.TextureCoords) * hw2;
	Output.Color += tex2D(TextureSampler3, PSIn.TextureCoords) * hw3;

	Output.Color.a = 1.0f;

	Output.Color.rgb *= saturate(PSIn.LightingFactor) + Ambient;

	return Output;
}

technique Textured
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL TexturedVS();
		PixelShader = compile PS_SHADERMODEL TexturedPS();
	}
}
