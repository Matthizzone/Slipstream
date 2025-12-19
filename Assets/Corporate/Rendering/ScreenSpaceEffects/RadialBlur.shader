Shader "Custom/Ripple"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Main texture
        _HitPoint ("Hit Point", Vector) = (0.5, 0.5, 0, 0)
        _Distortion("Distortion", Float) = 32
        _Iterations("Iterations", Float) = 0
        _Intensity("Intensity", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Distortion;
            float4 _HitPoint;
            float _Iterations;
            float _Intensity;

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
                // Ensure UVs are clamped
                input.uv = saturate(input.uv);




                // HERE WE GO

                
                float4 result;
                float weights;
                for (int i = 0; i < _Iterations; i++)
                {
                    float t = (float)i / _Iterations; // [0, 1]
                    float t2 = t * 2 - 1;  // [-1, 1]

                    float offscreen_amt = abs(_HitPoint.x) + abs(_HitPoint.y) - 2; // [-2 to INF]
                    offscreen_amt = 1 - saturate(offscreen_amt * 3); // [1, 0]

                    float dist = length(input.uv - _HitPoint);
                    dist = saturate(dist - 0.2);

                    float2 offset_vec = dist * _Distortion * offscreen_amt * _Intensity;
                    float z = _HitPoint.z;
                    if (z < 0) offset_vec = 0;

                    float2 p = input.uv + offset_vec * t2;

                    float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, p);

                    float weight = pow(100, -pow(t2, 2));

                    result += baseColor * weight;
                    weights += weight;
                }
                result /= weights;

                return result;

                //return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            }

            ENDHLSL
        }
    }
}
