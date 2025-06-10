float4x4 world;
float4x4 viewProjection;
texture shieldTex;
float distortionFactor;

sampler2D shieldMap = sampler_state
{
   Texture = (shieldTex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
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
	float4 Normal		    : TEXCOORD0;	// Base texture coordinates
	float2 Texcoord			: TEXCOORD1;
};

PS_INPUT DistortionGenVS(VS_INPUT In)
{
	PS_INPUT Out;

	float4x4 worldViewProjection = mul(world, viewProjection);

	Out.Position	= mul(In.Position, worldViewProjection);
	Out.Normal		= mul(In.Normal, worldViewProjection);
	Out.Texcoord	= In.Texcoord;
	
	return Out;
}

float4 NoDistortionPS(PS_INPUT In) : COLOR 
{
	return float4 (0.5f, 0.5f, 0.5f, 1.0f); 
}

float4 DistortionPS(PS_INPUT In) : COLOR 
{
	float2 Normal = (tex2D( shieldMap, In.Texcoord ).rg - float2(0.5, 0.5)) * distortionFactor;
	
	return float4 (Normal.x, Normal.y, 0.5f, 1.0f); 
}

//--------------------------------------------------------------//
// Technique Section for Distortion Generation Effects
//--------------------------------------------------------------//

technique DistortionGen
{
   pass NoDistortion
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
		ZWRITEENABLE		= TRUE;
		BLENDOP             = ADD;
		DESTBLEND			= ZERO;
		SRCBLEND			= ONE;
	
		CULLMODE = CCW;
		
		VertexShader = compile vs_2_0 DistortionGenVS();
		PixelShader  = compile ps_2_0 NoDistortionPS();
   }
   
   pass NoDistortionNoCulling
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= FALSE;
		ZWRITEENABLE		= TRUE;
		BLENDOP             = ADD;
		DESTBLEND			= ZERO;
		SRCBLEND			= ONE;
	
		CULLMODE = NONE;
		
		VertexShader = compile vs_2_0 DistortionGenVS();
		PixelShader  = compile ps_2_0 NoDistortionPS();
   }
   
   pass Distortion
   {
		ZENABLE				= TRUE;
		ALPHABLENDENABLE	= TRUE;
		ZWRITEENABLE		= FALSE;
		BLENDOP             = ADD;
		DESTBLEND			= ONE;
		SRCBLEND			= ONE;
	
		CULLMODE = CCW;
		
		VertexShader = compile vs_2_0 DistortionGenVS();
		PixelShader  = compile ps_2_0 DistortionPS();
   }
}