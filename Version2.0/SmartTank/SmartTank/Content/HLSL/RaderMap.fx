string description = "input Rader Sector Buffer and Depth Map, returns a rader texture";

float RaderAng;

//float RaderR;

float4x4 RaderRotaMatrix;

float4 RaderColor;

//float TargetSize;

texture DepthMap;

sampler DepthMapSampler = sampler_state 
{
	texture = <DepthMap>;
};


struct VertexInput
{
	float4 pos : POSITION;
};

struct VertexOutput
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD0;	
};

VertexOutput VS_RenderRader(VertexInput In)
{
	VertexOutput Out;
	
	Out.pos = In.pos;
	
	Out.pos.x = In.pos.y * sin(RaderAng*In.pos.x);
	Out.pos.y = In.pos.y * cos(RaderAng*In.pos.x);

	Out.pos = mul(Out.pos, RaderRotaMatrix);
	
	//float scale = 2 * RaderR / TargetSize;
	//float2 deltaPos = float2(-1+2*RaderR/TargetSize,1-2*RaderR/TargetSize);
	//Out.pos.x = Out.pos.x*scale + deltaPos.x;
	//Out.pos.y = Out.pos.y*scale + deltaPos.y;

	Out.tex = In.pos;

	return Out;
}

float4 PS_RenderRader(VertexOutput In) : COLOR
{
	float2 tex;
	tex.x = 0.5f * (In.tex.x/In.tex.y + 1.0f);
	tex.y = 1.0f;
	
	float depth = tex2D(DepthMapSampler, tex).w;

	if (In.tex.y > depth)
	{
		return float4(0.0f,1.0f,0.0f,0.0f);
	}
	else
	{
		float4 result = RaderColor;
		result.a = 0.65f-0.25f*In.tex.y;
		return result;
	}
}

technique RenderRader
{
	pass RenderRader
	{
		VertexShader = compile vs_1_1 VS_RenderRader();
		PixelShader = compile ps_2_0 PS_RenderRader();
	}

}