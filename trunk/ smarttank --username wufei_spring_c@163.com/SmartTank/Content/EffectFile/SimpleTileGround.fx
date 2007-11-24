

float2 ViewStartPos;

float2 ViewSize;

float2 MapSize;

float2 ScrnStartPos;

float2 ScreenViewScale;

//int UsedTexSum;

float2 GridSize;

texture indexMap;

sampler indexMapSampler = sampler_state
{
	texture = <indexMap>;
	AddressU  = CLAMP;
    AddressV  = CLAMP;
    AddressW  = CLAMP;
    MIPFILTER = NONE;
    MINFILTER = POINT;
    MAGFILTER = POINT;
};

texture tex1;

sampler texSampler1 = sampler_state
{
	texture = <tex1>;
	AddressU  = WRAP;
    AddressV  = WRAP;
    AddressW  = WRAP;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture tex2;

sampler texSampler2 = sampler_state
{
	texture = <tex2>;
	AddressU = WRAP;
	AddressV = WRAP;
	AddressW = WRAP;
	MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture tex3;

sampler texSampler3 = sampler_state
{
	texture = <tex3>;
	AddressU = WRAP;
	AddressV = WRAP;
	AddressW = WRAP;
	MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

float2 TranslateToRenderCoord( float2 logicPos )
{
	float2 result = ((logicPos * MapSize - ViewStartPos) / ViewSize) * ScreenViewScale + ScrnStartPos;
	result = 2 * result - 1;
	result.y = -result.y;
	return result;
}


struct VertexInput
{
	float4 pos : POSITION;
};

struct VertexOutput
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD0;	
};



VertexOutput VS_TileBackGround(VertexInput In)
{
	VertexOutput Out;
	
	Out.pos = In.pos;
	Out.pos.xy = TranslateToRenderCoord(In.pos.xy);
	Out.tex = In.pos;
	
	return Out;
}

float4 PS_TileBackGround(VertexOutput In):Color
{
	float2 curPos = In.tex;
	int index = tex2D(indexMapSampler,curPos).a * 10.5f;
	
	
	float4 texColor;
	if (index == 0)
	{
		texColor = tex2D(texSampler1, In.tex * GridSize);
	}
	else if (index == 1)
	{
		texColor = tex2D(texSampler2, In.tex * GridSize);
	}
	else if (index == 2)
	{
		texColor = tex2D(texSampler3, In.tex * GridSize);
	}	
	
	return texColor;
}

technique TileBackGround
{
	pass TileBackGround
	{
		VertexShader = compile vs_1_1 VS_TileBackGround();
		PixelShader = compile ps_2_0 PS_TileBackGround();
	}
}