Shader "Custom/Warbly"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Main texture
        _Scale("Scale", Float) = 100
        _Intensity("Intensity", Float) = 0.002
        _Speed("Intensity", Float) = 20
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

            float _Scale;
            float _Intensity;
            float _Speed;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);


                output.uv = output.positionHCS.xy / output.positionHCS.w * 0.5 + 0.5; // Transform to normalized UV space


                // Flip y for some reason
                output.uv.y = 1.0 - output.uv.y;

                return output;
            }


            
            // Random function for noise generation
            float random2D(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            // Perlin noise function
            float perlin2D(float2 p)
            {
                // Simple 2D Perlin noise implementation
                float2 i = floor(p);
                float2 f = frac(p);
    
                // Four corners of the cell
                float a = random2D(i);
                float b = random2D(i + float2(1.0, 0.0));
                float c = random2D(i + float2(0.0, 1.0));
                float d = random2D(i + float2(1.0, 1.0));
    
                // Smooth interpolation
                float2 u = f * f * (3.0 - 2.0 * f);
    
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }





            float4 frag(Varyings input) : SV_Target
            {
                // HERE WE GO

                // Ensure UVs are clamped
                input.uv = saturate(input.uv);
                
                // Parameters for controlling the effect
                float2 noiseScale = float2(_Scale, _Scale);  // Scale of the noise pattern
                float scrollSpeedX = _Speed;                // Speed of X scrolling
                float scrollSpeedY = _Speed;                // Speed of Y scrolling
                float distortionStrength = _Intensity / 1000;         // Strength of the distortion
    
                // Calculate scrolling offsets
                float2 scrollOffsetX = float2(_Time.y * scrollSpeedX, 0.0);
                float2 scrollOffsetY = float2(0.0, _Time.y * scrollSpeedY);
    
                // Generate first Perlin noise (scrolling in X+)
                float2 noiseUV_X = (input.uv * noiseScale) + scrollOffsetX;
                float noise1 = perlin2D(noiseUV_X);
    
                // Generate second Perlin noise (scrolling in Y+)
                float2 noiseUV_Y = (input.uv * noiseScale) + scrollOffsetY;
                float noise2 = perlin2D(noiseUV_Y);
    
                // Combine the noises
                float combinedNoise = noise1 * noise2;
    
                // Calculate X displacement
                float2 displacementX = float2(combinedNoise * distortionStrength, 0.0);
    
                // Generate different noise for Y displacement (using different seeds)
                float2 noiseUV_X_Y = (input.uv * noiseScale) + scrollOffsetX + float2(100.0, 200.0); // Offset for different seed
                float2 noiseUV_Y_Y = (input.uv * noiseScale) + scrollOffsetY + float2(300.0, 400.0); // Offset for different seed
    
                float noise3 = perlin2D(noiseUV_X_Y);
                float noise4 = perlin2D(noiseUV_Y_Y);
                float combinedNoiseY = noise3 * noise4;
    
                // Calculate Y displacement
                float2 displacementY = float2(0.0, combinedNoiseY * distortionStrength);
    
                // Apply both displacements
                float2 finalUV = input.uv + displacementX + displacementY;
    
                // Sample the texture with displaced UV coordinates
                float4 base_color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, finalUV);
    
                return base_color;
            }

            
            ENDHLSL
        }
    }
}
