Shader "Hidden/MultiplyToColor" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Blend("Blend", Range(0, 1)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
 
			#include "UnityCG.cginc"
 
			uniform sampler2D _MainTex;
			uniform float _Blend;
			uniform fixed4 _Color;
 
			float4 frag(v2f_img i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				float4 mp = c * _Color;

				float4 result = c;
				result.rgb = lerp(c.rgb, mp.rgb, _Blend);
				return result;
			}
			ENDCG
		}
	}
}