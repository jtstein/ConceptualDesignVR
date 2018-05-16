Shader "Unlit/Diaglow"
{
	Properties
	{
		_Color("Color", Color) = (0, 0, 255, 63)
		_Dist("Dist", Float) = .02
		_Bias("Bias", Float) = .5
		_Offset("Offset", Float) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask On
		LOD 100
		//ZWrite Off
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			//sampler2D _MainTex;
			//float4 _MainTex_ST;
			fixed4 _Color;
			float _Dist;
			float _Bias;
			float _Offset;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				
				float phase = frac((i.worldPos.x + i.worldPos.y + i.worldPos.z + _Offset) / _Dist);
				if (phase >= _Bias)
					col = 0;

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
