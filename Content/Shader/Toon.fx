float4x4 world;
float4x4 worldView;
float4x4 worldViewProjection;

float fLayerOneSharp;
float fLayerOneRough;
float fLayerOneContrib;
float fLayerTwoSharp;
float fLayerTwoRough;
float fLayerTwoContrib;

float fEdgeOffset;

float3 lightPosition;
float4 lightAmbient;
float4 lightSpecular;
float4 lightDiffuse;

float3 viewPosition;

texture baseMap;
sampler baseSampler = sampler_state
{
   Texture = (baseMap);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

struct VertexPositionNormalTex
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL;
	float2 Texcoord	: TEXCOORD0;
};

struct VS_OUTPUT_CEL
{
   float4 Position         : POSITION0;
   float3 Normal         : TEXCOORD1;
   float3 ViewDirection   : TEXCOORD2;
   float3 LightDirection   : TEXCOORD3;
   float2 Texcoord         : TEXCOORD0;
};

struct VS_OUTPUT_OUTLINE
{
   float4 Position         : POSITION0;
   float3 Normal         : TEXCOORD1;
};

VS_OUTPUT_CEL ToonVSCel(VertexPositionNormalTex Input)
{
   //float4x4 WorldViewProjection = mul(mul(matWorld, matView), matProjection);
   float3 ObjectPosition = mul(Input.Position, world);
   
   VS_OUTPUT_CEL Output;
   Output.Normal         = mul(Input.Normal, world);
   Output.Position         = mul(Input.Position, worldViewProjection);
   
   Output.ViewDirection   = viewPosition - ObjectPosition;
   Output.LightDirection   = lightPosition - ObjectPosition;
   Output.Texcoord         = Input.Texcoord;
   
   return Output;
}

VS_OUTPUT_OUTLINE ToonVSOutline(VertexPositionNormalTex Input)
{   
   VS_OUTPUT_OUTLINE Output;
   Output.Normal = mul(Input.Normal, world);
   Output.Position = mul(Input.Position, worldViewProjection)+mul(fEdgeOffset, mul(Input.Normal, worldViewProjection));
   
   return Output;
}

float4 ToonPSCel(VS_OUTPUT_CEL Input) : COLOR 
{
   float3 Normal = normalize(Input.Normal);
   float3 ViewDirection = normalize(Input.ViewDirection);
   float3 NLightDirection = normalize(Input.LightDirection);
   
   float oneW = 0.18f * ( 1.0f - fLayerOneSharp );
   float twoW = 0.18f * ( 1.0f - fLayerTwoSharp );
   
   float NDotL = dot(Normal, NLightDirection);
   float3 Reflection = normalize(2.0f * NDotL * Normal - NLightDirection);
   
   float4 layerOneColor = smoothstep(0.72f-oneW, 0.72f+oneW, pow(saturate(dot(Reflection, ViewDirection)), fLayerOneRough));
   float4 layerTwoColor = smoothstep(0.72f-twoW, 0.72f+twoW, pow(saturate(dot(Reflection, ViewDirection)), fLayerTwoRough));
   
   float4 baseColor = tex2D(baseSampler, Input.Texcoord) * lightDiffuse;
   
   float4 color = (baseColor + fLayerOneContrib*layerOneColor) + fLayerTwoContrib*layerTwoColor;
   color.a = 1.0;
   
   return color;
}

float4 ToonPSOutline(VS_OUTPUT_OUTLINE In) : COLOR 
{
	return float4(0.0f, 0.0f, 0.0f, 1.0f);
}

//--------------------------------------------------------------//
// Technique Section for Toon Effects
//--------------------------------------------------------------//

technique LowQuality
{
   pass ToonCel
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_2_0 ToonVSCel();
		PixelShader  = compile ps_2_0 ToonPSCel();
   }
   
   pass ToonOutline
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CCW;
		
		VertexShader = compile vs_2_0 ToonVSOutline();
		PixelShader  = compile ps_2_0 ToonPSOutline();
   }
}

technique MediumQuality
{
   pass ToonCel
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_2_0 ToonVSCel();
		PixelShader  = compile ps_2_0 ToonPSCel();
   }
   
   pass ToonOutline
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CCW;
		
		VertexShader = compile vs_2_0 ToonVSOutline();
		PixelShader  = compile ps_2_0 ToonPSOutline();
   }
}

technique HighQuality
{
   pass ToonCel
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 ToonVSCel();
		PixelShader  = compile ps_3_0 ToonPSCel();
   }
   
   pass ToonOutline
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CCW;
		
		VertexShader = compile vs_3_0 ToonVSOutline();
		PixelShader  = compile ps_3_0 ToonPSOutline();
   }
}