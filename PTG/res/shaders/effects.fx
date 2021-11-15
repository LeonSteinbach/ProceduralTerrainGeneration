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
	float4 Position   	: POSITION;
	float4 Color			: COLOR0;
	float LightingFactor : TEXCOORD0;
	float2 TextureCoords: TEXCOORD1;
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

//------- Texture Samplers --------

/*
texture xTexture0;
texture xTexture1;
texture xTexture2;
texture xTexture3;
*/

/*
sampler2D TextureSampler0 = sampler_state { Texture = <xTexture0>; MagFilter = Linear; MinFilter = Linear; MipFilter = Linear; AddressU = Wrap; AddressV = Wrap; };
sampler2D TextureSampler1 = sampler_state { Texture = <xTexture1>; MagFilter = Linear; MinFilter = Linear; MipFilter = Linear; AddressU = Wrap; AddressV = Wrap; };
sampler2D TextureSampler2 = sampler_state { Texture = <xTexture2>; MagFilter = Linear; MinFilter = Linear; MipFilter = Linear; AddressU = Wrap; AddressV = Wrap; };
sampler2D TextureSampler3 = sampler_state { Texture = <xTexture3>; MagFilter = Linear; MinFilter = Linear; MipFilter = Linear; AddressU = Wrap; AddressV = Wrap; };
*/

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


//------- SeasonColored --------

VertexToPixel SeasonColoredVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float4 inColor : COLOR)
{
	float4 Red = float4(0.7f, 0.7f, 0.9f, 1.0f);
	float4 Green = float4(0.6f, 0.4f, 0.3f, 1.0f);
	float4 Blue = float4(0.4f, 0.4f, 1.0f, 1.0f);

	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);

	if (inPos[1] > 0.0f)
	{
		Output.Color = Red;
	}
	else if (inPos[1] <= 130.0f && inPos[1] > 80.0f)
	{
		Output.Color = Green;
	}
	else
	{
		Output.Color = Blue;
	}

	float3 Normal = normalize(mul(normalize(inNormal), World));
	Output.LightingFactor = dot(Normal, -LightDirection);

	return Output;
}

PixelToFrame SeasonColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + Ambient;

	return Output;
}

technique SeasonColored
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  SeasonColoredVS();
		PixelShader = compile PS_SHADERMODEL  SeasonColoredPS();
	}
}


//------- Textured --------

VertexToPixel TexturedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(View, Projection);
	float4x4 preWorldViewProjection = mul(World, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	float3 Normal = normalize(mul(normalize(inNormal), World));
	Output.LightingFactor = dot(Normal, -LightDirection);

	return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	// Calculate blend textures
	float x = PSIn.Position[1];
	float n = 1.7f;
	float a = 5.2f;

	float hw0 = pow(-(a * x - n * 0), 4) + 1;
	float hw1 = pow(-(a * x - n * 1), 4) + 1;
	float hw2 = pow(-(a * x - n * 2), 4) + 1;
	float hw3 = pow(-(a * x - n * 3), 4) + 1;

	if (hw0 < 0) hw0 = 0;
	if (hw1 < 0) hw1 = 0;
	if (hw2 < 0) hw2 = 0;
	if (hw3 < 0) hw3 = 0;

	Output.Color = tex2D(TextureSampler0, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + Ambient;

	return Output;
}

technique Textured
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader = compile ps_2_0 TexturedPS();
	}
}
