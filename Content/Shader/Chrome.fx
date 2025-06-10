#define EPSILON 0.0000001

float4x4 world;
float4x4 worldView;
float4x4 worldViewProjection;
float4x4 projection;


float3   lightAmbient;			    // Global ambient light color (rgb)

float3   lightPosition[4];			// Light Positions in view space
float3   lightDirection[4];			// Light Directions in view space
float    lightAngle[4];				// Cosine of the light angles
float3   lightSpecular[4];			// Specular colors (rgb)
float3   lightDiffuse[4];			// Diffuse colors (rgb)
float    lightSpecularPower[4];		// Specular highlight sizes
float4x4 lightProjection[4];		// Transformations from view space to light projection
float    lightShadowDepth[4];		// Maximum real depths of the shadowmaps
float    lightShadow[4];			// Indicates whether the light has a shadowmap

float    shadowMapSize;             // Shadow map resolution (texels)

texture  shadowMap0;				// Shadowmaps
texture  shadowMap1;
texture  shadowMap2;
texture  shadowMap3;

texture environmentMap;
float reflectivity;

float4 silver = float4(0.3, 0.3, 0.3, 1.0);

samplerCUBE environmentSampler = sampler_state
{
   Texture = (environmentMap);
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

sampler shadowSampler[4] = {		// Sampler array for shadowmaps
 sampler_state
 {
   Texture = (shadowMap0);
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
 },
 sampler_state
 {
   Texture = (shadowMap1);
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
 },
 sampler_state
 {
   Texture = (shadowMap2);
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
 },
 sampler_state
 {
   Texture = (shadowMap3);
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
 }
};

struct VertexPositionNormalTex
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL;
	float2 Texcoord	: TEXCOORD0;
};

struct VS_OUTPUT 
{
	float4 ScreenPos		: POSITION;
	float3 ReflectionDir	: TEXCOORD1;
   	float4 ViewPosition     : TEXCOORD2;	// Position in view space
	float3 Normal           : TEXCOORD3;	// Normal in view space
	float4 LightPosition[4] : TEXCOORD4;	// Position in light spaces
};

VS_OUTPUT ChromeVS(VertexPositionNormalTex In)
{
	VS_OUTPUT Out;
 	float4 ScreenPos			= mul(float4(In.Position.xyz, 1), worldViewProjection);

	float3 ViewSpaceNormal		= normalize(mul(In.Normal, worldView));
	float3 ViewDirection		= -normalize(mul(In.Position, worldView));

	Out.ReflectionDir	= 2 * dot (ViewDirection, ViewSpaceNormal) * ViewSpaceNormal - ViewDirection;

	Out.ViewPosition	= mul(In.Position, worldView);
	Out.ScreenPos		= ScreenPos;
 	Out.Normal			= ViewSpaceNormal;
 	
 	for(int i=0; i<4; i++)
	{
		Out.LightPosition[i] = mul(In.Position, lightProjection[i]);
	}
 	
	return Out;
}

float4 ChromePS(VS_OUTPUT In) : COLOR 
{
	In.Normal = normalize(In.Normal);

	float3 diffuse = lightAmbient; // Start with ambient (shadow) color
	float3 specular = 0;
	
	// Add color from each light
    for(int i=0; i<4; i++)
    {
		// lightRay is the unit vector from the light to this pixel
		float3 lightRay = normalize(float3(In.ViewPosition - lightPosition[i]));
    
		// Light must face the pixel (within Theta)
		if( dot( lightRay, lightDirection[i] ) > lightAngle[i] ) 
		{
			// Pixel is in lit area. 
			float lightAmount = 1.0f;
			
			// Find out if it's in shadow using 2x2 percentage
			// closest filtering if it has shadow map
			if(lightShadow[i] == 1.0f)
			{
				// transform from RT space to texture space.
				float2 ShadowTexC = 0.5 * In.LightPosition[i].xy / In.LightPosition[i].w + float2( 0.5, 0.5 );
				ShadowTexC.y = 1.0f - ShadowTexC.y;

				// transform to texel space
				float2 texelpos = shadowMapSize * ShadowTexC;
		        
				// Determine the lerp amounts           
				float2 lerps = frac( texelpos );

				//read in bilerp stamp, doing the shadow checks
				float sourcevals[4];
				sourcevals[0] = (tex2D(shadowSampler[i], ShadowTexC).r + EPSILON < (In.LightPosition[i].z / In.LightPosition[i].w) / lightShadowDepth[i])? 0.0f : 1.0f;  
				sourcevals[1] = (tex2D(shadowSampler[i], ShadowTexC + float2(1.0/shadowMapSize, 0)).r + EPSILON < (In.LightPosition[i].z / In.LightPosition[i].w) / lightShadowDepth[i])? 0.0f: 1.0f;  
				sourcevals[2] = (tex2D(shadowSampler[i], ShadowTexC + float2(0, 1.0/shadowMapSize)).r + EPSILON < (In.LightPosition[i].z / In.LightPosition[i].w) / lightShadowDepth[i])? 0.0f: 1.0f;  
				sourcevals[3] = (tex2D(shadowSampler[i], ShadowTexC + float2(1.0/shadowMapSize, 1.0/shadowMapSize)).r + EPSILON < (In.LightPosition[i].z / In.LightPosition[i].w) / lightShadowDepth[i])? 0.0f: 1.0f;
		        
				// lerp between the shadow values to calculate our light amount
				lightAmount = lerp( lerp( sourcevals[0], sourcevals[1], lerps.x ),
										  lerp( sourcevals[2], sourcevals[3], lerps.x ),
										  lerps.y );
			}
			
			diffuse += saturate(dot(-lightRay, normalize(In.Normal))) * lightAmount * lightDiffuse[i];
			specular += pow(max(0.0f, dot(reflect(lightRay, In.Normal), -normalize(In.ViewPosition))), lightSpecularPower[i]) * lightAmount * lightSpecular[i];
		}
	}
	
	float4 base = saturate(float4(((silver.rgb * diffuse) + specular), silver.a));

	In.ReflectionDir.x = -In.ReflectionDir.x;

	float4 Reflection = reflectivity * texCUBE(environmentSampler, In.ReflectionDir);
	Reflection.a = 1;
	
	return saturate(Reflection + base);

}


//--------------------------------------------------------------//
// Technique Section for Chrome Effects
//--------------------------------------------------------------//

technique LowQuality
{
   pass ShadowedChrome
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE	=	TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 ChromeVS();
		PixelShader  = compile ps_3_0 ChromePS();
   }
}

technique MediumQuality
{
   pass ShadowedChrome
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE	=	TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 ChromeVS();
		PixelShader  = compile ps_3_0 ChromePS();
   }
}

technique HighQuality
{
   pass ShadowedChrome
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE	=	TRUE;
		ALPHABLENDENABLE	= FALSE;
	
		CULLMODE = CW;
		
		VertexShader = compile vs_3_0 ChromeVS();
		PixelShader  = compile ps_3_0 ChromePS();
   }
}