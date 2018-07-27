#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

sampler s0;

texture2D palette;
sampler s_palette = sampler_state { 
	Texture = <palette>;
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

float ind = 32;
float tex_width;
float tex_height;

float map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
{
	return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
}

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR
{
	float4 in_color = tex2D(s0, coords);

	// create index based on input
    //float r = in_color.r * (ind - 1);
    //float g = in_color.g * (ind - 1);
    //float b = in_color.b * ind;
    float r = map(in_color.r, 0, 1, 0, 31);
    float g = map(in_color.g, 0, 1, 0, 31);
    float b = map(in_color.b, 0, 1, 0, 31);

    r = floor(r);
    g = floor(g);
    b = floor(b);

    /*
	float r = map(in_color.r, 0, 1, 0, 31) / 32;
	float g = map(in_color.g, 0, 1, 0, 31) / 32;
	float b = map(in_color.r, 0, 1, 0, 31) / 32;
*/

    float x = ((r * ind) + g) / tex_width;
    float y = b / tex_height;

	// get colour on palette from position
	float4 out_color = tex2D(s_palette, float2(x, y));

	return out_color;
}

technique BasicColorDrawing
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};