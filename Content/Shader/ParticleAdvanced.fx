float4x4 worldView : WorldView;
float4x4 worldViewProjection : WorldViewProjection;

float speed;
float width;
float height;

float time
<
    string UIName = "Time";
    string UIWidget = "slider";
    half UIMin = 0.0f;
    half UIMax = 3.0f;
    half UIStep = 0.01f;   
>;

texture PositionMap;
sampler2D PositionSamp = sampler_state
{
    Texture   = <PositionMap>;
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
	float4 Position			: POSITION;
	float3 Normal  			: NORMAL;
	float2 TextureCoord0	: TEXCOORD0;
};

struct VS_OUTPUT 
{
   float4 Position			: POSITION;
   float2 TextureCoord0	    : TEXCOORD0;
};

VS_OUTPUT ParticleVSHigh(VS_INPUT In)
{
	VS_OUTPUT Out	= (VS_OUTPUT)0;
	
	// calculate texture position
	float yPosition	= float(int(In.TextureCoord0.x) / width) / float(height - 1);
	float xPosition	= fmod(In.TextureCoord0.x, float(width)) / float(width - 1);
	float4 TexCoord = float4(xPosition, yPosition, 0, 1);
    Out.Position	= tex2Dlod(PositionSamp, TexCoord);
    
    if (In.TextureCoord0.y == 0)
 	{
 		Out.Position	 += float4(In.Position.xyz * 0.5, 0);
 		//Out.Position	  = float4(0, 0, 0, 0) * 10;	// Billboarding
 		Out.TextureCoord0 = float2(0, 0);
 	}
 	if (In.TextureCoord0.y == 1)
 	{
 		Out.Position	 += float4(In.Position.xyz * 0.5, 0);
 		//Out.Position	 += float4(worldView._m00, worldView._m10, worldView._m20, 0) * 10;	// Billboarding
 		Out.TextureCoord0 = float2(0, 1);
 	}
 	if (In.TextureCoord0.y == 2)
 	{
 		Out.Position	 += float4(In.Position.xyz * 0.5, 0);
 		//Out.Position	 += float4(worldView._m01, worldView._m11, worldView._m21, 0) * 10;	// Billboarding
 		Out.TextureCoord0 = float2(1, 0);
 	}
    
 	Out.Position	= mul(float4(Out.Position.xyz, 1), worldViewProjection);
	return Out;
}

VS_OUTPUT ParticleVSMed(VS_INPUT In)
{
	return (VS_OUTPUT)0;
}

VS_OUTPUT ParticleVSLow(VS_INPUT In)
{
	return (VS_OUTPUT)0;
}

float4 ParticlePSHigh(VS_OUTPUT In) : COLOR 
{
	//return tex2D(ColorSamp, In.TextureCoord0) * float4(1, 1, 1, 1);
	return float4(1, 0, 0, 1);
	//return tex2D(ColorSamp, In.TextureCoord0) * float4(1, 1, 1, 1.0 - (time+1) * 0.5);
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
