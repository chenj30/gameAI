Shader "Custom/MinimapShader" {
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Pass {
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct input {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 uv_MainTex : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color: COLOR;
				float2 uv_MainTex : TEXCOORD0;
			};

			v2f vert(input v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv_MainTex = v.uv_MainTex;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				if (i.color.a == 0)
					discard;
				if (length(i.uv_MainTex - float2(0.5, 0.5)) < 0.15)
					return fixed4(1, 1, 1, 1);
				return i.color;
			}

			ENDCG
		}
	} 
}
