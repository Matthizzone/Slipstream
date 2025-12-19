Shader "Custom/ProperGrayscale"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Main texture
        _Intensity ("Intensity", Float) = 0.5 // Intensity property, default value is 0.5
        _Iterations("Iterations", Float) = 32
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

            // Declare depth texture and sampler
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Intensity;
            float _Iterations;

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

                // Sample depth from the camera's depth texture
                float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);

                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                float grayscale = 0.299 * baseColor.x + 0.587 * baseColor.y + 0.114 * baseColor.z;
                float bad_grascale = (baseColor.x + baseColor.y + baseColor.z) / 3;

                return grayscale;
            }

            ENDHLSL
        }
    }
}
