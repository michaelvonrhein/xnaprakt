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
	
    float4 tex          = tex2Dlod(NoiseSamp, float4(In.TextureCoord0.xy, 0, 1));
    //Out.Position      = In.Position + amplitude * fmod(tex.x * time * speed, 1.0) * (In.Position - center);
    //Out.Position		= float4(0, In.TextureCoord0.y * In.TextureCoord0.y, In.TextureCoord0.y, 0);
    Out.Position		= float4(0, 0, 0, 0);
    if (In.TextureCoord0.y == 0)
 	{
 		//Out.Position += float4(In.Position.xyz * 0.5, 0);
 		Out.Position = float4(0, 0, 0, 0) * 10;	// Billboarding
 		Out.TextureCoord0 = float2(0, 0);
 	}
 	if (In.TextureCoord0.y == 1)
 	{
 		//Out.Position += float4(In.Position.xyz * 0.5, 0);
 		Out.Position += float4(worldView._m00, worldView._m10, worldView._m20, 0) * 10;	// Billboarding
 		Out.TextureCoord0 = float2(0, 1);
 	}
 	if (In.TextureCoord0.y == 2)
 	{
 		//Out.Position += float4(In.Position.xyz * 0.5, 0);
 		Out.Position += float4(worldView._m01, worldView._m11, worldView._m21, 0) * 10;	// Billboarding
 		Out.TextureCoord0 = float2(1, 0);
 	}
 	
 	// Explosion
 	float4 vNormal = (normalize(In.Normal), 0);
 	Out.Position		+= float4(time * normalize(In.Normal) * 20, 1);
 	
    /* Heiligenschein 
    Out.Position.x      += cos(-In.TextureCoord0.x * time) * 6;
    Out.Position.y		+= sin(-In.TextureCoord0.x * time) * 6;
    Out.Position.z		+= sin(-In.TextureCoord0.x * time) * 6;*/
    
    /*//advanced Heiligenschein
    if(fmod(In.TextureCoord0.x, 0.2) == 0.0)
    {
		Out.Position.x      += cos(-In.TextureCoord0.x * time) * 6;// + In.Normal.x * time *2;
		Out.Position.y		+= sin(-In.TextureCoord0.x * time) * 6;// + In.Normal.y * time *2;
		Out.Position.z		+= sin(-In.TextureCoord0.x * time) * 6;// + In.Normal.z * time *2;
	}
	else
	{
		Out.Position.x      += cos(-In.TextureCoord0.x * time) * 6;// + In.Normal.x * time *2;
		Out.Position.y		-= sin(-In.TextureCoord0.x * time) * 6;// + In.Normal.y * time *2;
		Out.Position.z		+= sin(-In.TextureCoord0.x * time) * 6;// + In.Normal.z * time *2;
	}
	
    Out.Position.x *= time;
    Out.Position.y *= time;
    Out.Position.z *= time;*/
    
    /*Out.Position.x      += In.Normal.x * time * 100;
    Out.Position.y		+= In.Normal.y * time * 100;
    Out.Position.z		+= In.Normal.z * time * 100;*/
    
    /*// nettes Ding
    Out.Position.x      += cos(-In.TextureCoord0.x * time / 10) * 6;
    Out.Position.y		+= sin(In.TextureCoord0.x * time / 8) * 6;
    Out.Position.z		+= sin(-In.TextureCoord0.x * time / 6) * 6;*/
    
    /*Out.Position.x      += sin(-In.TextureCoord0.x * time / 10) * 6;
    Out.Position.y		+= fmod(-In.TextureCoord0.x * time, fmod(In.TextureCoord0.x, 5.0)) * 6;
    Out.Position.z		+= cos(-In.TextureCoord0.x * time / 6) * 6;*/
    
    /*Out.Position.x      += (cos(-In.TextureCoord0.x * time / 10) + sin(In.TextureCoord0.x * time / 8));
    Out.Position.y		+= (cos(-In.TextureCoord0.x * time / 3) + sin(In.TextureCoord0.x * time / 4));
    Out.Position.z		+= (cos(-In.TextureCoord0.x * time / 6) + sin(In.TextureCoord0.x * time / 5));*/
    
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
	//return tex2D(ColorSamp, In.TextureCoord0) * float4(1, 1, 1, 1);
	return tex2D(ColorSamp, In.TextureCoord0) * float4(1, 1, 1, 1.0 - (time+1) * 0.5);
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
