float4x4 worldView : WorldView;
float4x4 worldViewProjection : WorldViewProjection;

float4 center;
float amplitude;
float speed;
float gas;
bool turbo;
float4 scale;

float time
<
    string UIName = "Time";
    string UIWidget = "slider";
    half UIMin = 0.0f;
    half UIMax = 3.0f;
    half UIStep = 0.01f;   
>;

texture baseMap;
sampler2D baseSamp = sampler_state
{
    Texture   = <baseMap>;
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
    
    if (gas >= 0)
    {
		if (In.TextureCoord0.y == 0)
 		{
 			Out.Position += float4(In.Position.xyz, 0) * scale.w;
 			if (turbo == true)
 			{
 				Out.TextureCoord0 = float2(1, 1);
 			}
 			else
 			{
 				Out.TextureCoord0 = float2(0, 0);
 			}
 		}
 		if (In.TextureCoord0.y == 1)
 		{
 			Out.Position += float4(In.Position.xyz, 0) * scale.w;
 			Out.TextureCoord0 = float2(0, 1);
 		}
 		if (In.TextureCoord0.y == 2)
 		{
 			Out.Position += float4(In.Position.xyz, 0) * scale.w;
 			Out.TextureCoord0 = float2(1, 0);
 		}
	    
		Out.Position.x      += sin(-In.TextureCoord0.x * time / 10) * scale.x;
		Out.Position.y		+= fmod(-In.TextureCoord0.x * time * speed, fmod(In.TextureCoord0.x, amplitude)) * gas * scale.y;
		Out.Position.z		+= cos(-In.TextureCoord0.x * time / 6) * scale.z;
    }
    
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
	float alpha = 1.0f;
	// We want to see the fire!
	//if (gas == 0.0)
	//{
	//	alpha = 0.0;
	//}
	float4 texColor = tex2D(baseSamp, In.TextureCoord0);
	
	return (texColor * float4(1, 1, 1, alpha) * (1.0 - gas * 0.5)
		 + (float4(1, 1, 1, texColor.w * alpha) * gas * 0.5));
}

float4 ParticlePSMed(VS_OUTPUT In) : COLOR 
{
	return ParticlePSHigh(In);
}

float4 ParticlePSLow(VS_OUTPUT In) : COLOR 
{
	float4 texColor = tex2D(baseSamp, In.TextureCoord0);
	texColor.w *= 0.5;
	
	float halfGas = gas * 0.5;
	
	return (texColor * (1.0 - halfGas)
		 + (float4(1, 1, 1, texColor.w * 0.5) * halfGas));
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
		ZWRITEENABLE	 = TRUE;
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
		ZWRITEENABLE	 = TRUE;
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
		ALPHABLENDENABLE = TRUE;
		ZWRITEENABLE	 = TRUE;
		CULLMODE         = NONE;
		
		VertexShader = compile vs_3_0 ParticleVSHigh();
		PixelShader  = compile ps_3_0 ParticlePSHigh();
   }
}
