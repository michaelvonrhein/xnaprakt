float4x4 matWorldViewProjection;
float4x4 matWorld;
texture planetTex;
texture planetNightTex;

float3 lightDirection;
float3 lightDiffuse;

struct VS_INPUT 
{
   float4 Position	: POSITION0;
   float2 Texcoord	: TEXCOORD0;
   float3 Normal	: NORMAL0;
   
};

struct VS_OUTPUT 
{
   float4 Position			: POSITION0;
   float2 Texcoord			: TEXCOORD0;
   float3 Normal			: TEXCOORD1;
   
   
};

VS_OUTPUT Planet_VS ( VS_INPUT Input )
{
   VS_OUTPUT Output;

   Output.Position = mul( Input.Position, matWorldViewProjection );
   Output.Texcoord = Input.Texcoord;
   Output.Normal = mul(Input.Normal, matWorld);
   //Output.LightDirection = lightPosition - mul(Input.Position, matWorld);

   return( Output );   
}

sampler2D textureMap = sampler_state
{
   Texture = (planetTex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
};

sampler2D nightTextureMap = sampler_state
{
   Texture = (planetNightTex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
};

float4 Planet_PS ( VS_OUTPUT Input ) : COLOR0
{
	float3 Normal = normalize(Input.Normal);
	float NDotL = max(0, dot(Normal, lightDirection));
	float NDotLnight = (1.0 - NDotL) * (1.0 - NDotL);
	
	float4 baseColor = tex2D( textureMap, Input.Texcoord );
	float4 nightColor = tex2D( nightTextureMap, Input.Texcoord );
	float3 Color = lightDiffuse * (NDotL * baseColor + NDotLnight * nightColor);

	return float4(Color, baseColor.a);
}

technique Planet
{
   pass Pass_0
   {
	  ZENABLE			= TRUE;
	  ZWRITEENABLE		= TRUE;
	  
	  ALPHABLENDENABLE	= TRUE;
	  BLENDOP			= ADD;		
	  DESTBLEND			= INVSRCALPHA;
	  SRCBLEND			= SRCALPHA;
	  
      CULLMODE = CCW;
   
      VertexShader = compile vs_2_0 Planet_VS();
      PixelShader = compile ps_2_0 Planet_PS();
   }
}

technique PlanetZTestDisabled
{
   pass Pass_0
   {
	  ZENABLE			= FALSE;
	  ZWRITEENABLE		= TRUE;
	  
	  ALPHABLENDENABLE	= TRUE;
	  BLENDOP			= ADD;		
	  DESTBLEND			= INVSRCALPHA;
	  SRCBLEND			= SRCALPHA;
	  
      CULLMODE = CCW;
   
      VertexShader = compile vs_2_0 Planet_VS();
      PixelShader = compile ps_2_0 Planet_PS();
   }
}

