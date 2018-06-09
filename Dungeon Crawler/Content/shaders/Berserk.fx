#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	color.rgb = color.rbb;
	return color;
}
technique Technique1
{
    pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};