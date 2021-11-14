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
//float4x4 xWorldViewProjection;
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float4 xSeasonColor;

float3 xLightDirection;
float xAmbient;
bool xEnableLighting;
bool xShowNormals;
float3 xCamPos;
float3 xCamUp;
float xPointSpriteSize;




//------- Texture Samplers --------

Texture xTexture;
Texture xWater;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };


//------- Technique: Pretransformed --------

VertexToPixel PretransformedVS(float4 inPos : POSITION, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;

	Output.Position = inPos;
	Output.Color = inColor;

	return Output;
}

PixelToFrame PretransformedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  PretransformedVS();
		PixelShader = compile PS_SHADERMODEL  PretransformedPS();
	}
}
//------- My Technique: SeasonColored --------

VertexToPixel SeasonColoredVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float4 inColor : COLOR)
{
	//float4 Color = xSeasonColor;
	float4 Red = float4(0.7f, 0.7f, 0.9f, 1.0f);
	float4 Green = float4(0.6f, 0.4f, 0.3f, 1.0f);
	float4 Blue = float4(0.4f, 0.4f, 1.0f, 1.0f);

	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

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

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;

	if (xEnableLighting)
		Output.LightingFactor = dot(Normal, -xLightDirection);

	return Output;
}

PixelToFrame SeasonColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

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
//------- My Technique: SeasonColoredNoShading --------

VertexToPixel SeasonColoredNoShadingVS(float4 inPos : POSITION)
{
	float4 inColor = xSeasonColor;
	float4 Sand = float4(0.96f, 0.91f, 0.7f, 0.0f);
	float4 Blue = float4(0.0f, 0.0f, 1.0f, 0.0f);

	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);


	if (inPos[1] < 0.51f && inPos[1] != 0.000f)
	{
		Output.Color = Sand;
	}
	else if (inPos[1] == 0.000f)
	{
		Output.Color = Blue;
	}
	else
	{
		Output.Color = inColor;
	}

	return Output;
}

PixelToFrame SeasonColoredNoShadingPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	//Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

	return Output;
}

technique SeasonColoredNoShading
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  SeasonColoredNoShadingVS();
		PixelShader = compile PS_SHADERMODEL  SeasonColoredNoShadingPS();
	}
}
//------- Technique: Colored --------

VertexToPixel ColoredVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float4 inColor : COLOR)
{

	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;

	if (xEnableLighting)
		Output.LightingFactor = dot(Normal, -xLightDirection);

	return Output;
}

PixelToFrame ColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

	return Output;
}

technique Colored
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  ColoredVS();
		PixelShader = compile PS_SHADERMODEL  ColoredPS();
	}
}

//------- Technique: ColoredNoShading --------

VertexToPixel ColoredNoShadingVS(float4 inPos : POSITION, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;

	return Output;
}

PixelToFrame ColoredNoShadingPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;

	return Output;
}

technique ColoredNoShading
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  ColoredNoShadingVS();
		PixelShader = compile PS_SHADERMODEL  ColoredNoShadingPS();
	}
}


//------- Technique: Textured --------

VertexToPixel TexturedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = dot(Normal, -xLightDirection);

	return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

	return Output;
}

technique Textured
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  TexturedVS();
		PixelShader = compile PS_SHADERMODEL  TexturedPS();
	}
}

//------- Technique: TexturedNoShading --------

VertexToPixel TexturedNoShadingVS(float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	return Output;
}

PixelToFrame TexturedNoShadingPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);

	return Output;
}

technique TexturedNoShading
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  TexturedNoShadingVS();
		PixelShader = compile PS_SHADERMODEL  TexturedNoShadingPS();
	}
}

//------- Technique: PointSprites --------

VertexToPixel PointSpriteVS(float3 inPos: POSITION0, float2 inTexCoord : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;

	float3 center = mul(inPos, xWorld);
	float3 eyeVector = center - xCamPos;

	float3 sideVector = cross(eyeVector, xCamUp);
	sideVector = normalize(sideVector);
	float3 upVector = cross(sideVector, eyeVector);
	upVector = normalize(upVector);

	float3 finalPosition = center;
	finalPosition += (inTexCoord.x - 0.5f) * sideVector * 0.5f * xPointSpriteSize;
	finalPosition += (0.5f - inTexCoord.y) * upVector * 0.5f * xPointSpriteSize;

	float4 finalPosition4 = float4(finalPosition, 1);

	float4x4 preViewProjection = mul(xView, xProjection);
	Output.Position = mul(finalPosition4, preViewProjection);

	Output.TextureCoords = inTexCoord;

	return Output;
}

PixelToFrame PointSpritePS(VertexToPixel PSIn) : COLOR0
{
	PixelToFrame Output = (PixelToFrame)0;
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	return Output;
}

technique PointSprites
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL  PointSpriteVS();
		PixelShader = compile PS_SHADERMODEL  PointSpritePS();
	}
}