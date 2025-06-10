float4x4 lightViewProjection;
float maxDepth;
float minDepth;

struct VS_OUTPUT 
{
   float4 Position   : POSITION;    
   float4 Position2D : TEXCOORD0;
};

VS_OUTPUT ShadowMapVS(float4 inPos : POSITION)
{
	VS_OUTPUT Output;
 	Output.Position = mul(inPos, lightViewProjection);
    Output.Position2D = Output.Position;
	return Output;
}

float4 ShadowMapPS_Objects(VS_OUTPUT In) : COLOR 
{	
	float value=(In.Position2D.z);
	return float4(value,0,0,1);
}

float4 ShadowMapPS_Track(VS_OUTPUT In) : COLOR 
{	
	float value=(In.Position2D.z);
	
	return float4(value,0,0,1);
}

//--------------------------------------------------------------//
// Technique Section for track Effects
//--------------------------------------------------------------//

technique ShadowMap
{
   pass Track
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = CCW;
        
		VertexShader = compile vs_2_0 ShadowMapVS();
		PixelShader  = compile ps_2_0 ShadowMapPS_Track();
   }
   pass Objects
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		=TRUE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = CCW;
        
		VertexShader = compile vs_2_0 ShadowMapVS();
		PixelShader  = compile ps_2_0 ShadowMapPS_Track();
   }
}
