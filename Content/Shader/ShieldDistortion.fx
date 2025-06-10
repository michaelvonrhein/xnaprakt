#define epsilon 0.01

texture scene;
texture distortionMap;

sampler sceneSampler = sampler_state
{
	Texture = (scene);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

sampler distortionSampler = sampler_state
{
	Texture = (distortionMap);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

// The Distortion map represents zero displacement as 0.5, but in an 16 bit color
// channel there is no exact value for 0.5. ZeroOffset adjusts for this error.
const float ZeroOffset = 0.5f / 256.0f; //65535.0f;

float4 DistortionPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 displacement = tex2D(distortionSampler, texCoord).rg;
    // Convert from [0,1] to [-.5, .5) 
    // .5 is excluded by adjustment for zero
    displacement -= 0.5f + ZeroOffset;

	if (length(displacement.xy) < epsilon) displacement = float2(0, 0);
	
	float4 color = tex2D(sceneSampler, texCoord.xy + displacement);
	
	// Tint result slightly blue and brighten based on displacement
	float len = length(displacement.xy);
	color *= 1.0f + (10.0f * len);
	color.b += len * 2.0f;

    return saturate(color);
}

//--------------------------------------------------------------//
// Technique Section for ShieldDistortion
//--------------------------------------------------------------//

technique ShieldDistortion
{
	pass ShieldDistortion
	{
		PixelShader = compile ps_2_0 DistortionPS();
	}
}