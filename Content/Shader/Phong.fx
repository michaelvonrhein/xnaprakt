#include "shadow.fx"

float4x4 view;
float4x4 projection;

texture  baseMap;
texture  bumpMap;





float3   lightDirection;
float3   lightAmbient;



sampler baseSampler = sampler_state 
{
   Texture = (baseMap);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};
sampler bumpSampler = sampler_state 
{
   Texture = (bumpMap);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

struct VS_INPUT
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL;
	float2 Texcoord	: TEXCOORD0;
};

struct PS_INPUT 
{
	float4 Position         : POSITION0;    // Position in projective space
	float2 Texcoord		    : TEXCOORD0;	// Base texture coordinates
	float4 ViewPosition     : TEXCOORD1;	// Position in view space
	float3 Normal           : TEXCOORD2;	// Normal in view space
	float3 LightDirection   : TEXCOORD3;	// Light direction in view space
	float4 LightPos[3] 		: TEXCOORD4;    // Position in light space
};

PS_INPUT PhongVS(VS_INPUT In)
{
	PS_INPUT Out;
	
	Out.Texcoord		 = In.Texcoord;
	Out.ViewPosition     = mul(In.Position, worldView);
	Out.Position         = mul(Out.ViewPosition, projection);
	Out.Normal           = mul(In.Normal, worldView);
	Out.LightDirection   = mul(lightDirection, view);
	Out.LightPos[0]   = mul(In.Position, lightProjection[0]);
	Out.LightPos[1]   = mul(In.Position, lightProjection[1]);
	Out.LightPos[2]   = mul(In.Position, lightProjection[2]);
	
	return Out;
}


inline float3 phong(float3 normal, float3 lightdirection, float3 viewdirection, float3 color)
{
	normal = normalize(normal);
	viewdirection = normalize(viewdirection);
	lightdirection = normalize(lightdirection);
	float NDotL = dot(normal, lightdirection);
	
	float3 reflection = normalize(((2.0f * normal) * NDotL) - lightdirection);
	float RDotV = max(0.0f, dot(reflection, viewdirection));
	
	return (lightAmbient * color) + (NDotL * color) + pow(RDotV, 12);
}

float4 PhongPSHigh(PS_INPUT In) : COLOR 
{
	float lightAmount=getShadowVal_HIGH(In.LightPos,In.ViewPosition.z);
	float4 zomg;

	float4 base = tex2D(baseSampler, In.Texcoord);

    return float4(phong(In.Normal, In.LightDirection, -In.ViewPosition, base.rgb) * (lightAmount * 0.6 + 0.4), base.a);
}


//--------------------------------------------------------------//
// Technique Section for Phong Effects
//--------------------------------------------------------------//

technique LowQuality
{
   pass ShadowedPhong
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
		ZWRITEENABLE		= TRUE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 PhongVS();
		PixelShader  = compile ps_3_0 PhongPSHigh();
   }
}

technique MediumQuality
{
   pass ShadowedPhong
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
		ZWRITEENABLE		= TRUE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 PhongVS();
		PixelShader  = compile ps_3_0 PhongPSHigh();
   }
}

technique HighQuality
{
   pass ShadowedPhong
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
		ZWRITEENABLE		= TRUE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 PhongVS();
		PixelShader  = compile ps_3_0 PhongPSHigh();
   }
}