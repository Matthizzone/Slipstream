Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Main texture
        _HitPoint ("Hit Point", Vector) = (0.5, 0.5, 0, 0)
        _ThresholdMin("ThresholdMin", Float) = 1
        _ThresholdMax("ThresholdMax", Float) = 1

        _ColoringBook("ColoringBook", Float) = 1 //  0: coloring book mode, 1: normal mode
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
            float _ThresholdMin;
            float _ThresholdMax;
            float _ColoringBook;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);


                output.uv = output.positionHCS.xy / output.positionHCS.w * 0.5 + 0.5; // Transform to normalized UV space


                // Flip y for some reason
                output.uv.y = 1.0 - output.uv.y;

                return output;
            }

            float3 HSVtoRGB(float3 hsv)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
                return hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
            }

            float4 frag(Varyings input) : SV_Target
            {
                // HERE WE GO

                // Ensure UVs are clamped
                input.uv = saturate(input.uv);

                //float4 base_color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                //return base_color;

                // Get texture dimensions for proper pixel offset calculation
                float2 texelSize = float2(1.0 / 1920.0, 1.0 / 1080.0); // Example for 1920x1080
    
                // Sample current pixel and convert to grayscale for edge detection
                float4 base_color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float center = dot(base_color.rgb, float3(0.299, 0.587, 0.114));
    
                // Sample neighboring pixels (right and down)
                float2 uv_right = input.uv + float2(texelSize.x, 0);
                float2 uv_down = input.uv + float2(0, texelSize.y);
    
                float4 color_right = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_right);
                float4 color_down = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_down);
    
                float right = dot(color_right.rgb, float3(0.299, 0.587, 0.114));
                float down = dot(color_down.rgb, float3(0.299, 0.587, 0.114));
    
                // Calculate gradient (dx, dy)
                float dx = right - center;
                float dy = down - center;
    
                // Calculate magnitude and angle
                float magnitude = sqrt(dx * dx + dy * dy);
                float angle = atan2(dy, dx);
    
                // Normalize angle to 0-1 range for hue (atan2 returns -π to π)
                float hue = (angle + 3.14159265) / (2.0 * 3.14159265);
    
                // Use magnitude for both saturation and value
                // You can adjust these multipliers to control the effect intensity
                float saturation = saturate(magnitude * 10.0); // Scale factor for saturation
                float value = saturate(magnitude * 5.0);       // Scale factor for brightness
    
                if (_ColoringBook > 0.5)
                {
                    // coloring book mode
                    
                    // Convert HSV to RGB
                    value = value > 0.03 ? 0 : 1;
                    float3 hsv = float3(0, 0, value);
                    float3 edge_color = HSVtoRGB(hsv);
    
                    float3 final_color = base_color * edge_color;
                    return float4(final_color, base_color.a);

                    return float4(base_color.rgb * edge_color, base_color.a);
                    return float4(final_color, base_color.a);

                }
                else
                {
                    // normal mode

                    // Convert HSV to RGB
                    float3 hsv = float3(hue, saturation, value);
                    float3 edge_color = HSVtoRGB(hsv);
    
                    // Option 1: Pure edge detection output
                    float scale = _ThresholdMax - _ThresholdMin;
                    float3 final_color = (edge_color - _ThresholdMin) / scale;
                    return float4(final_color, base_color.a);
                }
    
                // Blend with original
                //float3 final_color = lerp(base_color.rgb, edge_color, saturate(magnitude * 2.0));
                //return float4(final_color, base_color.a);

                    // Multiply with original
                    //float3 final_color = base_color * magnitude;
                    //return float4(final_color, base_color.a);
            }

            ENDHLSL
        }
    }
}
