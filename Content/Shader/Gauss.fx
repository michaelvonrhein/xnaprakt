texture  tex;
sampler TextureSampler : register(s0) = sampler_state
{
   Texture = (tex);
   ADDRESSU = CLAMP;
   ADDRESSV = CLAMP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

float4 GaussPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, texCoord + SampleOffsets[i]) * SampleWeights[i];
    }
    
    return c * 2.0f;
}

//--------------------------------------------------------------//
// Technique Section for bloom effect
//--------------------------------------------------------------//

technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 GaussPixelShader();
    }
}
