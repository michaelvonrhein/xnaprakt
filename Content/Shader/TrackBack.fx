#define EPSILON 0.0

float4x4 worldView;
float4x4 projection;

float3   lightAmbient;			    // Global ambient light color (rgb)



texture  baseMap;					// Base (diffuse & ambient) texture


float    tilesPerSegment;
float    segments;

sampler baseSampler = sampler_state // Sampler for base texture
{
   Texture = (baseMap);
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
	float3 Tangent  : TANGENT;
};

struct PS_INPUT 
{
	float4 Position         : POSITION0;    // Position in projective space
	float2 Texcoord		    : TEXCOORD0;	// Base texture coordinates
	float4 ViewPosition     : TEXCOORD1;	// Position in view space
	float3 Normal           : TEXCOORD2;	// Normal in view space
};

PS_INPUT TrackVS(VS_INPUT In)
{
	PS_INPUT Out;
	
	Out.ViewPosition     = mul(In.Position, worldView);
	Out.Texcoord         = float2(In.Texcoord.x * segments, In.Texcoord.y * tilesPerSegment);
	Out.Normal           = mul(In.Normal, (float3x3)worldView);
	Out.Position         = mul(Out.ViewPosition, projection);
	
	return Out;
}



float4 TrackPSHigh(PS_INPUT In) : COLOR 
{
	In.Normal = normalize(In.Normal);

	float3 diffuse = lightAmbient; // Start with ambient (shadow) color
	float3 specular = 0;
	
	
	
	float4 base = tex2D(baseSampler, In.Texcoord);

	return saturate(float4(((base.rgb * diffuse) + specular), 0.5));
}



//--------------------------------------------------------------//
// Technique Section for Track Effects
//--------------------------------------------------------------//

technique LowQuality
{
   pass TransparentShadowedTrackBack
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;
		

        CULLMODE = CW;
        
		VertexShader = compile vs_3_0 TrackVS();
		PixelShader  = compile ps_3_0 TrackPSHigh();
   }
}

technique MediumQuality
{
   pass TransparentShadowedTrackBack
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;
		

        CULLMODE = CW;
        
		VertexShader = compile vs_3_0 TrackVS();
		PixelShader  = compile ps_3_0 TrackPSHigh();
   }
}

technique HighQuality
{
   pass TransparentShadowedTrackBack
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;
		
        CULLMODE = CW;
        
		VertexShader = compile vs_3_0 TrackVS();
		PixelShader  = compile ps_3_0 TrackPSHigh();
   }
}

