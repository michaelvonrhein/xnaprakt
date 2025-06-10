float4x4 worldView : WorldView;
float4x4 worldViewProjection : WorldViewProjection;

int texWidth;
int texHeight;
float width;
float height;
float aspectRatio;

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
   float2 TextureCoord1			: TEXCOORD1;
};

inline float2 GetTexPoint(int texIndex)
{
	float tex1Width		= 1.0 / float(texWidth);
	float tex1Height	= 1.0 / float(texHeight);
	float2 tex0Point	= float2((texIndex % texWidth) * tex1Width, float(int(float(texIndex) / float(texWidth))) * tex1Height);
	return tex0Point;
}

VS_OUTPUT ParticleVSHigh(VS_INPUT In)
{
	VS_OUTPUT Out       = (VS_OUTPUT)0;
	
    Out.Position		= float4(0, 0, 0, 0);
    float4 viewRight	= normalize(float4(worldView._m00, worldView._m10, worldView._m20, 0)) * width * 0.5;
    float4 viewUp		= normalize(float4(worldView._m01, worldView._m11, worldView._m21, 0)) * height * 0.5 * aspectRatio;
    
    float tex1Width		= 1.0 / float(texWidth);
    float tex1Height	= 1.0 / float(texHeight);
    int texCount		= texWidth * texHeight;
    float texTime		= 1.0 / float(texCount);
    float texIndex		= time / texTime;
    
	if (In.TextureCoord0.y == 0)
 	{
 		if (In.TextureCoord0.x == 0)
 		{
 			Out.Position -= viewRight + viewUp;
 			Out.TextureCoord0 = float2(0, tex1Height) + GetTexPoint(int(texIndex));
 			Out.TextureCoord1 = float2(0, tex1Height) + GetTexPoint(int((texIndex + 1) % texCount));
 		}
 		if (In.TextureCoord0.x == 1)
 		{
 			Out.Position += viewRight + viewUp;
 			Out.TextureCoord0 = float2(tex1Width, 0) + GetTexPoint(int(texIndex));
 			Out.TextureCoord1 = float2(tex1Width, 0) + GetTexPoint(int((texIndex + 1) % texCount));
 		}
 	}
 	else if (In.TextureCoord0.y == 1)
 	{
 		Out.Position += viewUp - viewRight;
 		Out.TextureCoord0 = GetTexPoint(int(texIndex));
 		Out.TextureCoord1 = GetTexPoint(int((texIndex + 1) % texCount));
 	}
 	else if (In.TextureCoord0.y == 2)
 	{
 		Out.Position += viewRight - viewUp;
 		Out.TextureCoord0 = float2(tex1Width, tex1Height) + GetTexPoint(int(texIndex));
 		Out.TextureCoord1 = float2(tex1Width, tex1Height) + GetTexPoint(int((texIndex + 1) % texCount));
 	}
    
 	Out.Position = mul(float4(Out.Position.xyz, 1), worldViewProjection);
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
    int texCount		= texWidth * texHeight;
    float texTime		= 1.0 / float(texCount);
    float texIndex		= time / texTime;
    float texWeight		= texIndex - int(texIndex);
    
	return (tex2D(baseSamp, In.TextureCoord0) * (1.0 - texWeight)
		 + (tex2D(baseSamp, In.TextureCoord1) * texWeight));
}

float4 ParticlePSMed(VS_OUTPUT In) : COLOR 
{
	return tex2D(baseSamp, In.TextureCoord0);
}

float4 ParticlePSLow(VS_OUTPUT In) : COLOR 
{
	return tex2D(baseSamp, In.TextureCoord0);
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
