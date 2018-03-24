#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Source: https://channel9.msdn.com/coding4fun/articles/SilverShader--Introduction-to-Silverlight-and-WPF-Pixel-Shaders
float BlockCount : register(C0);

sampler2D input : register(S0);

// Static computed vars for optimization
static float BlockSize = 1.0f / BlockCount;

float4 MainPS(float4 pos : SV_POSITION, float4 color0 : COLOR0, float2 uv : TEXCOORD0) : COLOR
{
    // Calculate block center
    float2 blockPos = floor(uv * BlockCount);
    float2 blockCenter = blockPos * BlockSize + BlockSize * 0.5;
            
    // Sample color at the calculated coordinate
    return tex2D(input, blockCenter);

    //Explaination: For instance, BlockCount is 10, BlockSize is 0.1 and input x coordinate is 0.27.
    //Then calculated blockPos is rounded to 2 (it's second block in a first row). Center of this block is 0.25 and pixel color of it is returned by shader.
    //Same value will be retuned when input coords will be in a range of <0.20; 0.30).
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};