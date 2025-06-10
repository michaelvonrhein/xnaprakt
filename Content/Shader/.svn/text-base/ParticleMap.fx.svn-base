float4x4 worldView : WorldView;
float4x4 worldViewProjection : WorldViewProjection;

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
   float4 ScreenPosition	: POSITION;
   float4 Position			: BINORMAL;
};

inline float2 GetTexPoint(int particleID)
{
	float tex1Width		= 1.0 / float(width);
	float tex1Height	= 1.0 / float(height);
	float2 tex0Point	= float2((particleID % width) * tex1Width, float(int(float(particleID) / float(width))) * tex1Height);
	return tex0Point + 0.5 * float2(tex1Width, tex1Height);
}

VS_OUTPUT ParticleMapVS(VS_INPUT In)
{
	VS_OUTPUT Out    = (VS_OUTPUT)0;
	
	// load current position from texture
	float yTexPos	= float(int(In.TextureCoord0.x) / width) / float(height - 1);
	float xTexPos	= fmod(In.TextureCoord0.x, float(width)) / float(width - 1);
	float4 TexCoord = float4(xTexPos, yTexPos, 0, 1);
    Out.Position	= tex2Dlod(PositionSamp, TexCoord);
	
	// calculate particle position
	Out.Position.x	+= -In.TextureCoord0.x * time * 6;
	Out.Position.y	+= -In.TextureCoord0.x * time * 6;
	Out.Position.z	+= -In.TextureCoord0.x * time * 6;
	
	// calculate screen position
	float yPosition		= float(int(In.TextureCoord0.x) / width) / float(height - 1) * 2.0 - 1.0;
	float xPosition		= fmod(In.TextureCoord0.x, float(width)) / float(width - 1) * 2.0 - 1.0;
	Out.ScreenPosition	= float4(xPosition, yPosition, 0, 1);
	
	return Out;
}

float4 ParticleMapPS(VS_OUTPUT In) : COLOR 
{
	return float4(1, 1, 1, 1);
	//return In.Position;
}


//--------------------------------------------------------------//
// Technique Section for Particle Effects
//--------------------------------------------------------------//

technique ParticleMap
{
   pass Particles
   {
		ZENABLE          = FALSE;
		ZWRITEENABLE	 = FALSE;
		ALPHABLENDENABLE = FALSE;
		CULLMODE         = NONE;
		
		VertexShader = compile vs_3_0 ParticleMapVS();
		PixelShader  = compile ps_3_0 ParticleMapPS();
   }
}