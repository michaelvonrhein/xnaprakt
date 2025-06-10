#include "Shadow.fx"



float4x4 worldViewIT;
float4x4 projection;

texture  diffuse1;	
texture  diffuse2;
texture  diffuse3;

texture  normal1;	
texture  normal2;
texture  normal3;

texture diffuseDefinition;
texture normalDefinition;
texture specular;	

texture lightmap;
texture lightdirmap;	

	


float    tilesPerSegment;
float    segments;



float3 lightdir;



sampler lightmapTexture = sampler_state 
{
   Texture = (lightmap);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

sampler lightdirTexture = sampler_state 
{
   Texture = (lightdirmap);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

sampler diffuse1Texture = sampler_state 
{
   Texture = (diffuse1);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
     MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};
sampler diffuse2Texture = sampler_state 
{
   Texture = (diffuse2);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
     MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};
sampler diffuse3Texture = sampler_state 
{
   Texture = (diffuse3);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
     MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

sampler normal1Texture = sampler_state 
{
   Texture = (normal1);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};
sampler normal2Texture = sampler_state 
{
   Texture = (normal1);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};
sampler normal3Texture = sampler_state 
{
   Texture = (normal1);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};
sampler diffdescTexture=sampler_state 
{
   Texture = (diffuseDefinition);
   ADDRESSU = CLAMP;
   ADDRESSV = CLAMP;
   MAGFILTER = POINT;
   MINFILTER = POINT;
   MIPFILTER = POINT;
};
sampler normdescTexture=sampler_state 
{
   Texture = (normalDefinition);
   ADDRESSU = CLAMP;
   ADDRESSV = CLAMP;
    MAGFILTER = POINT;
   MINFILTER = POINT;
   MIPFILTER = POINT;
};

sampler specularTexture=sampler_state 
{
   Texture = (specular);
   ADDRESSU = CLAMP;
   ADDRESSV = CLAMP;
    MAGFILTER = POINT;
   MINFILTER = POINT;
   MIPFILTER = POINT;
};

struct VS_INPUT
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL;
	float2 Texcoord	: TEXCOORD0;
	float3 Tangent  : TANGENT;
};




struct PS_INPUT 
{
	float4 Position         : POSITION0;    // Position in clip space
	float2 Texcoord0        : TEXCOORD0;
    float2 Texcoord1        : TEXCOORD1;	
	float3 TexSpaceLightDir	: TEXCOORD2;
	float ViewDepth 		:TEXCOORD3;
	float4 LightPos[3]      :TEXCOORD4;
	
	};

PS_INPUT TrackVS(VS_INPUT In)
{
	PS_INPUT Out;
	float3x3 texSpaceMatrix;

	Out.Texcoord0	  = In.Texcoord;
	Out.Texcoord1     = float2(In.Texcoord.x * segments, In.Texcoord.y * tilesPerSegment);
	Out.Position      = mul(mul(In.Position, worldView), projection);
	Out.TexSpaceLightDir = mul(lightdir, worldView);
	Out.ViewDepth  = mul(In.Position, worldView).z;
	Out.LightPos[0]   = mul(In.Position, lightProjection[0]);
	Out.LightPos[1]   = mul(In.Position, lightProjection[1]);
	Out.LightPos[2]   = mul(In.Position, lightProjection[2]);
	texSpaceMatrix[0]=mul(float4(In.Tangent.xyz,0),worldViewIT).xyz;
	texSpaceMatrix[2]=mul(float4(In.Normal.xyz,0),worldViewIT).xyz;
	texSpaceMatrix[1]=float4(cross(texSpaceMatrix[0],texSpaceMatrix[2]),0).xyz;
	Out.TexSpaceLightDir=mul(texSpaceMatrix,Out.TexSpaceLightDir.xyz);

	return Out;
}





void applyLightingHigh(PS_INPUT In,float3 TexSpaceNormal,inout float3 diffuse,inout float3 specular)
{ 
	float ndotl = dot(TexSpaceNormal,In.TexSpaceLightDir);
	if(ndotl<0.15) diffuse=0;
	else
	{
		diffuse = saturate(ndotl)*diffuse;
		diffuse*=getShadowVal_HIGH(In.LightPos,In.ViewDepth);
	}
}

void applyLightingMedium(PS_INPUT In,float3 TexSpaceNormal,inout float3 diffuse,inout float3 specular)
{ 
	float ndotl = dot(TexSpaceNormal,In.TexSpaceLightDir);
	if(ndotl<0.15) diffuse=0;
	else
	{
		diffuse = saturate(ndotl)*diffuse;
		diffuse*=getShadowVal_MEDIUM(In.LightPos,In.ViewDepth);
	}
}

inline float4 blend(float4 color1, float4 color2,float alpha)
{
   return color1*(1.0f-color2.w*alpha)+color2*color2.w*alpha;
}

//compute ligthing with the blinn_phong model for specular lighting
inline float4 diffuse_specular_blinn(float3 lightdir, float3 normal, float3 viewdir,
                             float4 diffuseColor,
                             float lightcolorSpecular,
                             float lightcolorDiffuse,
                             float materialGlossiness,
                             float4 materialSpecularColor)
{
   float4 specpart=0,diffpart=0;
   float3 h,vl;
   vl=viewdir+lightdir;
   h=vl/length(vl);
   specpart=(pow(max(dot(normal,h),0),materialGlossiness)*lightcolorSpecular) * materialSpecularColor;
   diffpart=(max(dot(normal,lightdir),0)*lightcolorDiffuse)*diffuseColor; 
    return specpart+diffpart;
}

float4 TrackPSHigh(PS_INPUT In) : COLOR 
{

	float4 fragmentColor=float4(0,0,0,0);
	float4 tdv;  
	float4 specParam;
	float3 light=1;
	tdv=tex2D(normdescTexture,In.Texcoord0);
	float3 TexSpaceNormal=tex2D(normal1Texture,In.Texcoord1)*tdv.x+tex2D(normal2Texture,In.Texcoord1)*tdv.y+tex2D(normal3Texture,In.Texcoord1)*tdv.z;
	TexSpaceNormal=2.0f*(TexSpaceNormal-0.5f);
	float3 TexSpaceLightDir=2.0f*(tex2D(lightdirTexture,In.Texcoord0)-0.5f);
	float ndotl=max(0,dot(TexSpaceNormal,TexSpaceLightDir));
	float3 specular = 0;
	applyLightingHigh(In,TexSpaceNormal,light,specular);
	float3 lightmap=tex2D(lightmapTexture,In.Texcoord0).xyz;
	
	light=saturate(light+ndotl*0.8*lightmap+0.2*lightmap);
		
	tdv=tex2D(diffdescTexture,In.Texcoord0);
   
	fragmentColor=blend(fragmentColor,tex2D(diffuse1Texture,In.Texcoord1),tdv.x);
	fragmentColor=blend(fragmentColor,tex2D(diffuse2Texture,In.Texcoord1),tdv.y);
	fragmentColor=blend(fragmentColor,tex2D(diffuse3Texture,In.Texcoord1),tdv.z);
	tdv=tex2D(normdescTexture,In.Texcoord0);
	
	fragmentColor.xyz=fragmentColor.xyz*light.xyz;
  
	return fragmentColor;
}

float4 TrackPSMedium(PS_INPUT In) : COLOR 
{

	
	float4 fragmentColor = float4(0,0,0,0);
	float4 tdv;  
	float4 specParam;
	float3 light=1;
	
	float3 specular = 0;
	float3 TexSpaceNormal=float3(0,0,1);
	applyLightingHigh(In,TexSpaceNormal,light,specular);
	light += tex2D(lightmapTexture,In.Texcoord0).xyz;
	
	tdv = tex2D(diffdescTexture,In.Texcoord0);
   
	fragmentColor=blend(fragmentColor,tex2D(diffuse1Texture,In.Texcoord1),tdv.x);
	fragmentColor=blend(fragmentColor,tex2D(diffuse2Texture,In.Texcoord1),tdv.y);
	fragmentColor=blend(fragmentColor,tex2D(diffuse3Texture,In.Texcoord1),tdv.z);
	tdv = tex2D(normdescTexture,In.Texcoord0);
	
	fragmentColor.xyz = fragmentColor.xyz*light.xyz;
  
	return fragmentColor;
}

float4 TrackPSLow(PS_INPUT In) : COLOR 
{

	float4 fragmentColor=float4(0,0,0,0);
	float4 tdv;  
	float4 specParam;
	float3 light=1;
	
	float3 specular = 0;
	light=tex2D(lightmapTexture,In.Texcoord0).xyz;
	
	tdv=tex2D(diffdescTexture,In.Texcoord0);
   
	fragmentColor=blend(fragmentColor,tex2D(diffuse1Texture,In.Texcoord1),tdv.x);
	fragmentColor=blend(fragmentColor,tex2D(diffuse2Texture,In.Texcoord1),tdv.y);
	fragmentColor=blend(fragmentColor,tex2D(diffuse3Texture,In.Texcoord1),tdv.z);
	tdv=tex2D(normdescTexture,In.Texcoord0);
	
	fragmentColor.xyz=fragmentColor.xyz*light.xyz;
  
	return fragmentColor;
}



//--------------------------------------------------------------//
// Technique Section for Track Effects
//--------------------------------------------------------------//

technique LowQuality
{
   pass ShadowedTrack
   {
			ZENABLE				= TRUE;
			ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;
		

        CULLMODE = CCW;
        
		VertexShader = compile vs_3_0 TrackVS();
		PixelShader  = compile ps_3_0 TrackPSHigh();
   }
   
  
}

technique MediumQuality
{
   pass ShadowedTrack
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;
		

        CULLMODE = CCW;
        
		VertexShader = compile vs_3_0 TrackVS();
		PixelShader  = compile ps_3_0 TrackPSHigh();
   }
   
   
}

technique HighQuality
{
   pass ShadowedTrack
   {
			ZENABLE				= TRUE;
			ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;
		

        CULLMODE = CCW;
        
		VertexShader = compile vs_3_0 TrackVS();
		PixelShader  = compile ps_3_0 TrackPSHigh();
   }

}

