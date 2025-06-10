float4x4 worldViewProjection;

texture flareTexture;
sampler flareSampler : register(s0) = sampler_state
{
   Texture = (flareTexture);
   MinFilter = LINEAR;
   MagFilter = LINEAR;
   AddressU = CLAMP;
   AddressV = CLAMP;
};

texture occlusionTexture;
sampler occlusionSampler : register(s1) = sampler_state
{
   Texture = (occlusionTexture);
   MinFilter = POINT;
   MagFilter = POINT;
   AddressU = WRAP;
   AddressV = WRAP;
};

float3 flareColor;

float4 OcclusionVS(float4 Position : POSITION0) : POSITION
{
	return mul(Position, worldViewProjection);
}

float4 OcclusionPS(float4 Position : POSITION) : COLOR 
{
	return float4(0,0,0,1);
}

float4 ReductionVS(float4 Position : POSITION0) : POSITION
{
	return mul(Position, worldViewProjection);
}

float4 ReductionPS() : COLOR 
{
	float sum = 0;
	for(float i = 0; i < 16; i++)
	{
		for(float j = 0; j < 16; j++)
		{
			sum += tex2D(occlusionSampler, float2(i * 0.0625, j * 0.0625)).x;
		}
	}
	return float4(sum / 256.0, 0, 0, 1);
}

float4 LensFlarePS(float2 TexCoord : TEXCOORD0) : COLOR 
{
	float3 color     = tex2D(flareSampler, TexCoord).rgb;
	float  occlusion = tex2D(occlusionSampler, float2(0,0)).x;
	
	return float4(color * flareColor * occlusion, 1);
}

//--------------------------------------------------------------//
// Technique Section for lens flare Effect
//--------------------------------------------------------------//

technique Occlusion
{
   pass OcclusionSampling
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = NONE;
        
		VertexShader = compile vs_2_0 OcclusionVS();
		PixelShader  = compile ps_2_0 OcclusionPS();
   }
   
   pass ReductionSampling
   {
		ZENABLE				= FALSE;
		ZWRITEENABLE		= FALSE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = NONE;
        
        VertexShader = compile vs_3_0 ReductionVS();
		PixelShader  = compile ps_3_0 ReductionPS();
   }
}

technique LensFlare
{
   pass Flares
   {
		ZENABLE				= FALSE;
		ZWRITEENABLE		= FALSE;
		ALPHABLENDENABLE	= TRUE;
		BLENDOP             = ADD;
        
		PixelShader  = compile ps_2_0 LensFlarePS();
   }
}
