Shader "Skybox/ImageBackground"
{
	Properties
	{
		_MainTex("Skybox Image", 2D) = "white" {}
		_Top("Top", Color) = (1,1,1,0)
		_Bottom("Bottom", Color) = (0,0,0,0)
		_Color("Color", Color) = (1,1,1,0)
		_mult("mult", Float) = 1
		_pwer("pwer", Float) = 1
		[Toggle(_SCREENSPACE_ON)] _Screenspace("Screen space", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Background" }
		LOD 100
		Cull Off
		ZWrite Off
		ZTest LEqual
		Blend Off
		ColorMask RGBA
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma shader_feature_local _SCREENSPACE_ON

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
			float4 _Bottom;
			float4 _Top;
			float _mult;
			float _pwer;

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float2 uv = i.screenPos.xy / i.screenPos.w;

				#ifdef _SCREENSPACE_ON
					// Fill full screen with texture
					return tex2D(_MainTex, uv) * _Color;
				#else
					// Gradient skybox fallback
					float gradientFactor = pow(saturate((uv.y * _mult)), _pwer);
					return lerp(_Bottom, _Top, gradientFactor);
				#endif
			}
			ENDCG
		}
	}
	
	Fallback Off
	CustomEditor "ASEMaterialInspector"
}
