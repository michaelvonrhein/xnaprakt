float4x4 worldView : WorldView;
float4x4 worldViewProjection : WorldViewProjection;

float4 center;
float amplitude;
float speed;

float time
<
    string UIName = "Time";
    string UIWidget = "slider";
    half UIMin = 0.0f;
    half UIMax = 3.0f;
    half UIStep = 0.01f;   
>;

texture NoiseMap;
sampler2D NoiseSamp = sampler_state
{
    Texture   = <NoiseMap>;
    MipFilter = NONE;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU  = WRAP;
    AddressV  = WRAP;
};

texture ColorMap;
sampler2D ColorSamp = sampler_state
{
    Texture   = <ColorMap>;
    MipFilter = NONE;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU  = WRAP;
    AddressV  = WRAP;
};

struct VS_INPUT
{
	float4 Position				: POSITION;
	float3 Normal  				: NORMAL;
	float2 TextureCoord0	    : TEXCOORD0;
};

struct VS_OUTPUT 
{
   float4 Position			    : POSITION;
   float2 TextureCoord0	    	: TEXCOORD0;
};

VS_OUTPUT ParticleVSHigh(VS_INPUT In)
{
	VS_OUTPUT Out       = (VS_OUTPUT)0;
	
    Out.Position		= float4(0, 0, 0, 0);
    if (In.TextureCoord0.y == 0)
 	{
 		Out.Position += float4(In.Position.xyz, 0);
 		Out.TextureCoord0 = float2(0, 0);
 	}
 	if (In.TextureCoord0.y == 1)
 	{
 		Out.Position += float4(In.Position.xyz, 0);
 		Out.TextureCoord0 = float2(0, 1);
 	}
 	if (In.TextureCoord0.y == 2)
 	{
 		Out.Position += float4(In.Position.xyz, 0);
 		Out.TextureCoord0 = float2(1, 0);
 	}
 	
 	// Explosion
 	float4 vNormal = (normalize(In.Normal), 0);
 	Out.Position		+= float4(time * normalize(In.Normal) * 20, 1);
    
 	Out.Position        = mul(float4(Out.Position.xyz, 1), worldViewProjection);
	return Out;
}

VS_OUTPUT ParticleVSMed(VS_INPUT In)
{
	return ParticleVSHigh(In);
}

VS_OUTPUT ParticleVSLow(VS_INPUT In)
{
	return ParticleVSHigh(In);
}

float4 ParticlePSHigh(VS_OUTPUT In) : COLOR 
{
	return tex2D(ColorSamp, In.TextureCoord0) * float4(1, 1, 1, 1.0 - (time+1)/2);
}

float4 ParticlePSMed(VS_OUTPUT In) : COLOR 
{
	return ParticlePSHigh(In);
}

float4 ParticlePSLow(VS_OUTPUT In) : COLOR 
{
	return ParticlePSHigh(In);
}


//--------------------------------------------------------------//
// Technique Section for Particle Effects
//--------------------------------------------------------------//

technique LowQuality
{
   pass Particles
   {
		ZENABLE          = TRUE;
		ALPHABLENDENABLE = TRUE;
		ZWRITEENABLE	 = FALSE;
		CULLMODE         = NONE;
		
		VertexShader = compile vs_1_1 ParticleVSLow();
		PixelShader  = compile ps_1_1 ParticlePSLow();
   }
}

technique MediumQuality
{
   pass Particles
   {
		ZENABLE			 = TRUE;
		ALPHABLENDENABLE = TRUE;
		ZWRITEENABLE	 = FALSE;
		CULLMODE         = NONE;
		
		VertexShader = compile vs_2_0 ParticleVSMed();
		PixelShader  = compile ps_2_0 ParticlePSMed();
   }
}

technique HighQuality
{
   pass Particles
   {
		ZENABLE          = TRUE;
		ZWRITEENABLE	 = FALSE;
		ALPHABLENDENABLE = TRUE;
		CULLMODE         = NONE;
		
		VertexShader = compile vs_3_0 ParticleVSHigh();
		PixelShader  = compile ps_3_0 ParticlePSHigh();
   }
}
