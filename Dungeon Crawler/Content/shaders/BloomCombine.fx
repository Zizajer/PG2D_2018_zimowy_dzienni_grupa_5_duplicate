#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

//EXTRACT
float BloomThreshold=0.5;
//EXTRACT

//BLUR
#define SAMPLE_COUNT 15
float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];
//BLUR

//COMBINE
float BloomIntensity=2;
float BaseIntensity=4;
float BloomSaturation;
float BaseSaturation=1;
//COMBINE

// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));
    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{

	   
	//EXTRACT
	float4 sat = tex2D(s0, coords);
	// Adjust it to keep only values brighter than the specified threshold.
    saturate((sat - BloomThreshold) / (1 - BloomThreshold)); 
	//EXTRACT
	
	//BLUR
	float4 blur = tex2D(s0, coords);  
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        blur += tex2D(s0, coords + SampleOffsets[i]) * SampleWeights[i];
    }
    // Look up the bloom and original base image colors.
	//BLUR
	
	float4 bloom = blur+sat;
	float4 base = tex2D(s0, coords);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));
    
    // Combine the two images.
	return bloom * base + base + blur;
}

technique Technique1
{
    pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};
