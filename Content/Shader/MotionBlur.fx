
texture scene;
float blurIntensity;
float2 blurCenter;

sampler sceneSampler = sampler_state
{
	Texture = (scene);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};




float4 BlurPS(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(sceneSampler,texCoord);
	float2 coord = texCoord;
	
	for( int i=0; i<6; i++)
	{
		coord.x -= pow(blurIntensity * (coord.x - blurCenter.x), 4);
		coord.y -= pow(blurIntensity * (coord.y - blurCenter.y), 4);
		color += tex2D(sceneSampler,coord);
	}
	color = color / 7;
	return color;
}

//--------------------------------------------------------------//
// Technique Section for Phong Effects
//--------------------------------------------------------------//

technique MotionBlur
{
	pass MotionBlur
	{
		PixelShader = compile ps_2_0 BlurPS();
	}
}