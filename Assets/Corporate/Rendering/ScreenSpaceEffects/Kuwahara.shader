Shader "Custom/Kuwahara"
{
    Properties
    {
        _Radius("Radius", Float) = 2
        _DepthThreshold("Depth Threshold", Float) = 0
        _MainTex ("Base Texture", 2D) = "white" {} // Main texture
        _CameraDepthTexture ("Depth Texture", 2D) = "white" {} // Depth texture
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
        
        Stencil
        {
            Ref 1
            Comp NotEqual   // Only draw where stencil != 1
        }

        Pass
        {
            Name "SceneDepthPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // Homogeneous clip space
                float2 uv : TEXCOORD0;           // Normalized UV coordinates
            };
            
            float _Radius;          // Radius of kernel, e.g. 2 = 5x5, 4 = 9x9
            float _DepthThreshold;  // How much depth difference is tolerated

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);


                output.uv = output.positionHCS.xy / output.positionHCS.w * 0.5 + 0.5; // Transform to normalized UV space


                // Flip y for some reason
                output.uv.y = 1.0 - output.uv.y;

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = saturate(input.uv);
                float2 texelSize = 1.0 / _ScreenParams.xy;

                float centerDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;

                float3 m[4] = {0,0,0, 0,0,0, 0,0,0, 0,0,0};
                float3 s[4] = {0,0,0, 0,0,0, 0,0,0, 0,0,0};
                int w[4] = {0, 0, 0, 0};

                int radius = (int)_Radius;

                [loop]
                for (int i = -radius; i <= radius; i++)
                {
                    [loop]
                    for (int j = -radius; j <= radius; j++)
                    {
                        float2 offset = float2(i, j) * texelSize;
                        float2 sampleUV = uv + offset;
                        float3 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV).rgb;
                        float sampleDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, sampleUV).r;

                        // Edge preserve using depth difference
                        if (abs(sampleDepth - centerDepth) > _DepthThreshold)
                            continue;

                        int index = (i <= 0 ? 0 : 1) + (j <= 0 ? 0 : 2); // TL, TR, BL, BR
                        m[index] += c;
                        s[index] += c * c;
                        w[index]++;
                    }
                }

                float minVariance = 1e20;
                float3 result = float3(0, 0, 0);

                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    if (w[i] == 0) continue;
                    float n = w[i];
                    float3 mean = m[i] / n;
                    float3 variance = s[i] / n - mean * mean;

                    float v = variance.r + variance.g + variance.b;
                    if (v < minVariance)
                    {
                        minVariance = v;
                        result = mean;
                    }
                }

                return float4(result, 1.0);
            }


            ENDHLSL
        }
    }
}
