texture scene;
texture noise;

float2 noisePos;

float scratchPos[4];
float scratchWidth[4];

sampler sceneSampler = sampler_state
{
	Texture = (scene);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

sampler noiseSampler = sampler_state
{
	Texture = (noise);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float4 OldCameraPS(float2 texCoord : TEXCOORD0) : COLOR0
{
	// get color from backbuffer
	float4 original = tex2D(sceneSampler, texCoord.xy);
	
	float noisePower = lerp(0.75f, 1.0f, tex2D(noiseSampler, texCoord.xy / 2 + noisePos).r);

	//convert to grayscale
	float gray = 0.299 * original.r + 0.584 * original.g + 0.114 * original.b;
	
	//add sepia effect;
	float4 color = float4(0.9 * gray, 0.8 * gray, 0.4 * gray, original.a);
	
	for (int i = 0; i < 4; i++)
	{
		if (texCoord.x >= scratchPos[i] && texCoord.x <= scratchPos[i] + scratchWidth[i])
			color = float4(0,0,0,0);
	}

    return noisePower * color;
}

//--------------------------------------------------------------//
// Technique Section for ShieldDistortion
//--------------------------------------------------------------//

technique OldCamera
{
	pass OldCamera
	{
		PixelShader = compile ps_2_0 OldCameraPS();
	}
}