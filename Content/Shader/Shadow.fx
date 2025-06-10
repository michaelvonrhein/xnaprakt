#define EPSILON 0.0015
#define MINVAR 0.0005
#define BLEEDREDUCTION 0.5
float4x4 lightProjection[3];
float4x4 worldView;
float splitPlanes[4];
float    shadowMapSize;             // Shadow map resolution (texels)
texture  shadowMap0;				// Shadowmaps
texture  shadowMap1;
texture  shadowMap2;	

sampler shadowSampler[3] = {		// Sampler array for shadowmaps
 sampler_state
 {
   Texture = (shadowMap0);
     MAGFILTER = POINT;
   MINFILTER = POINT;
   AddressU = CLAMP;
   AddressV = CLAMP;
 },
 sampler_state
 {
   Texture = (shadowMap1);
   MAGFILTER = POINT;
   MINFILTER = POINT;
   AddressU = CLAMP;
   AddressV = CLAMP;
 },
 sampler_state
 {
   Texture = (shadowMap2);
     MAGFILTER = POINT;
   MINFILTER = POINT;
   AddressU = CLAMP;
   AddressV = CLAMP;
 }
};



inline float2 transformTexcoords(float4 pos)
{
    float2 ret =0.5 * pos.xy / pos.w + float2( 0.5, 0.5 );
    ret.y = 1.0f - ret.y;
	return ret;
}

inline float4 transformTexcoordsBias(float4 pos)
{
    float4 ret=0;
    ret.xy =0.5 * pos.xy / pos.w + float2( 0.5, 0.5 );
    ret.y = 1.0f - ret.y;
    ret.z=-2.0f;
    ret.w=-2.0f;
	return ret;
}

inline float linstep(float min, float max,float v)
{
	return clamp((v-min)/(max-min),0,1);
}

inline float bendaid(float p_max,float value)
{
	return linstep(value,1,p_max);
}

float ChebyshevUB(float2 Moments,float t)
{
	float p=(t<=Moments.x);
	
	float Variance=Moments.y-(Moments.x*Moments.x);
	Variance=max(Variance,MINVAR);
	
	float d=t-Moments.x;
	float p_max=bendaid(Variance/(Variance+d*d),BLEEDREDUCTION);
	
	return max(p,p_max);
}

float simpleVSMFilter(float2 texcoord, float depth,sampler shadowtex)
{
	float step=1/shadowMapSize;
	float2 x=float2(2,0);
	float2 y=float2(0,2);
	float2 xy=float2(2,2);
	float2 nxy=float2(-2,2);
	float2 moments=tex2D(shadowtex,texcoord).xy+
	tex2D(shadowtex,texcoord+x*step).xy+
	tex2D(shadowtex,texcoord-x*step).xy+
	tex2D(shadowtex,texcoord+y*step).xy+
	tex2D(shadowtex,texcoord-y*step).xy+
	tex2D(shadowtex,texcoord+xy*step).xy+
	tex2D(shadowtex,texcoord-xy*step).xy+
	tex2D(shadowtex,texcoord+nxy*step).xy+
	tex2D(shadowtex,texcoord-nxy*step).xy;
	return ChebyshevUB(moments/9.0f,depth);
	
}

float simpleVSMFilter2(float2 texcoord, float depth,sampler shadowtex)
{
	float step=1/shadowMapSize;
	float2 x=float2(2,0);
	float2 y=float2(0,2);
	float2 xy=float2(2,2);
	float2 nxy=float2(-2,2);
	float ret=ChebyshevUB(tex2D(shadowtex,texcoord).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord+x*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord-x*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord+y*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord-y*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord+xy*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord-xy*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord+nxy*step).xy,depth)+
	ChebyshevUB(tex2D(shadowtex,texcoord-nxy*step).xy,depth);
	return ret/9;
	
}
float pcf13(float2 texcoord, float depth,sampler shadowtex)
{
	float step=1/shadowMapSize;
	float2 x=float2(step,0);
	float2 y=float2(0,step);
	float2 xy=float2(step,step);
	float2 nxy=float2(-step,step);
	float ret;
	ret=(depth<=tex2D(shadowtex,texcoord+xy*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-xy*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+nxy*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-nxy*2).x);
	
	if(ret==0||ret>=4) return ret/4;
	else
	{
	
	ret+=(depth<=tex2D(shadowtex,texcoord+x*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-x*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+y*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-y*2).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+nxy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-nxy).x);
	ret+= (depth<=tex2D(shadowtex,texcoord).x);
	return ret/13;
	}
}


float pcf9(float2 texcoord, float depth,sampler shadowtex)
{
	float step=1/shadowMapSize;
	float2 x=float2(step,0);
	float2 y=float2(0,step);
	float2 xy=float2(step,step);
	float2 nxy=float2(-step,step);
	float ret;
	ret=(depth<=tex2D(shadowtex,texcoord+xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+nxy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-nxy).x);
	ret+= (depth<=tex2D(shadowtex,texcoord).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+x).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-x).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+y).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-y).x);
	return ret/9;
}

float pcf9fast(float2 texcoord, float depth,sampler shadowtex)
{
	float step=1/shadowMapSize;
	float2 x=float2(step,0);
	float2 y=float2(0,step);
	float2 xy=float2(step,step);
	float2 nxy=float2(-step,step);
	float ret;
	ret=(depth<=tex2D(shadowtex,texcoord+xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+nxy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-nxy).x);
	
	if(ret==0||ret>=4) return ret/4;
	else
	{
	ret+= (depth<=tex2D(shadowtex,texcoord).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+x).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-x).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+y).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-y).x);
	return ret/9;
	}
}
float pcf4(float2 texcoord, float depth,sampler shadowtex)
{
	float step=0.5/shadowMapSize;
	float2 xy=float2(step,step);
	float2 nxy=float2(-step,step);
	float ret;
	ret=(depth<=tex2D(shadowtex,texcoord+xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-xy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord+nxy).x);
	ret+=(depth<=tex2D(shadowtex,texcoord-nxy).x);
	return ret/4;
}





float simple(float2 texcoord, float depth,sampler shadowtex)
{
	return (depth<=tex2D(shadowtex,texcoord).x);	
}



float getShadowVal_HIGH(float4 LightPos[3],float ViewDepth)
{ 
	float shadowval = 1;
	if(abs(ViewDepth)<=(splitPlanes[1]))
	{
		shadowval=pcf9(transformTexcoords(LightPos[0]),LightPos[0].z-EPSILON,shadowSampler[0]);
	}
	else if(abs(ViewDepth)<=splitPlanes[2])
	{
		shadowval=pcf4(transformTexcoords(LightPos[1]),LightPos[1].z-EPSILON,shadowSampler[1]);
	}
	else 
	{
		shadowval=pcf4(transformTexcoords(LightPos[2]),LightPos[2].z-EPSILON,shadowSampler[2]);
	}
	return shadowval;
}

float getShadowVal_MEDIUM(float4 LightPos[3],float ViewDepth)
{ 
	float shadowval = 1;
	if(abs(ViewDepth)<=(splitPlanes[1]))
	{
		shadowval=pcf4(transformTexcoords(LightPos[0]),LightPos[0].z-EPSILON,shadowSampler[0]);
	}
	else if(abs(ViewDepth)<=splitPlanes[2])
	{
		shadowval=pcf4(transformTexcoords(LightPos[1]),LightPos[1].z-EPSILON,shadowSampler[1]);
	}
	else 
	{
		shadowval=simple(transformTexcoords(LightPos[2]),LightPos[2].z-EPSILON,shadowSampler[2]);
	}
	return shadowval;
}
float getShadowVal_LOW(float4 LightPos[3],float ViewDepth)
{ 
	float shadowval = 1;
	if(abs(ViewDepth)<=(splitPlanes[1]))
	{
		shadowval=simple(transformTexcoords(LightPos[0]),LightPos[0].z-EPSILON,shadowSampler[0]);
	}
	else if(abs(ViewDepth)<=splitPlanes[2])
	{
		shadowval=simple(transformTexcoords(LightPos[1]),LightPos[1].z-EPSILON,shadowSampler[1]);
	}
	else 
	{
		shadowval=simple(transformTexcoords(LightPos[2]),LightPos[2].z-EPSILON,shadowSampler[2]);
	}
	return shadowval;
}