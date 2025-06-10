float4x4 matWorldViewProjection;
texture skyCubeTex;

struct VS_INPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   
};

struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   
};

VS_OUTPUT SkyCube_VS ( VS_INPUT Input )
{
   VS_OUTPUT Output;

   Output.Position = mul( Input.Position, matWorldViewProjection );
   Output.Texcoord = Input.Texcoord;

   return( Output );
   
}

sampler2D textureMap = sampler_state
{
   Texture = (skyCubeTex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
};

struct PS_INPUT 
{
   float2 Texcoord : TEXCOORD0;
   
};

float4 SkyCube_PS ( PS_INPUT Input ) : COLOR0
{
   return tex2D( textureMap, Input.Texcoord );
}

technique SkyBox
{
   pass Pass_0
   {
	  ZWRITEENABLE	= TRUE;
	  ZENABLE		= TRUE;
      CULLMODE		= CCW;
   
      VertexShader = compile vs_2_0 SkyCube_VS();
      PixelShader = compile ps_2_0 SkyCube_PS();
   }

}

