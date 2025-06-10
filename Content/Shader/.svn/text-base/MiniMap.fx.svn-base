float4x4 worldViewProjection;
float4x4 world;
float4   color;

float minZ;
float maxZ;
float outline;

struct VS_INPUT
{
	float4 Position	: POSITION0;
	float4 Normal	: NORMAL0;
};

struct VS_OUTPUT 
{
   float4 Position	: POSITION;
   float4 Normal	: TEXCOORD0;
   float4 Pos		: TEXCOORD1;
};

VS_OUTPUT MiniMapVS(VS_INPUT In)
{
	VS_OUTPUT Output;
	
	In.Normal = normalize(In.Normal);
	
 	Output.Position = mul(In.Position, worldViewProjection);
 	Output.Pos = mul(In.Position, world);
 	Output.Normal = In.Normal;
	return Output;
}

float4 MiniMapPS(VS_OUTPUT In) : COLOR 
{
	return color;
}

float4 MiniMapHeightPS(VS_OUTPUT In) : COLOR 
{
	float height = maxZ - minZ;

	float4 result = color * (In.Pos.z - minZ + height) / (3 * height);
	
	result.a = 1;
	
	return result;
}

VS_OUTPUT MiniMapOutlineVS(VS_INPUT In)
{
	VS_OUTPUT Output;
	
	In.Normal = normalize(In.Normal);
	
	Output.Normal = In.Normal;
	
 	Output.Position = mul(In.Position - mul(outline, In.Normal), worldViewProjection);;
 	Output.Pos = mul(In.Position, world);
	return Output;
}

float4 MiniMapOutlinePS(VS_OUTPUT In) : COLOR 
{
	return float4(1, 1, 1, 1);
}

//--------------------------------------------------------------//
// Technique Section for track Effects
//--------------------------------------------------------------//

technique MiniMap
{
   pass MiniMapPlayer
   {
		ZENABLE				= FALSE;
		ZWRITEENABLE		= FALSE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = CCW;
        
		VertexShader = compile vs_2_0 MiniMapVS();
		PixelShader  = compile ps_2_0 MiniMapPS();
   }
   
   pass MiniMapEntity
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = CCW;
        
		VertexShader = compile vs_2_0 MiniMapVS();
		PixelShader  = compile ps_2_0 MiniMapPS();
   }

   pass MiniMapTrack
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= FALSE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = CW;
        
		VertexShader = compile vs_2_0 MiniMapVS();
		PixelShader  = compile ps_2_0 MiniMapHeightPS();
   }
   
   pass MiniMapTrackOutline
   {
		ZENABLE				= TRUE;
		ZWRITEENABLE		= TRUE;
		ALPHABLENDENABLE	= FALSE;

        CULLMODE = CCW;
        
		VertexShader = compile vs_2_0 MiniMapOutlineVS();
		PixelShader  = compile ps_2_0 MiniMapOutlinePS();
   }
}
