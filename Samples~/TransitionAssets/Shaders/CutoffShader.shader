Shader "Unlit/CutoffShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {} //Main Texture, leave empty
		_PatternTex ("Pattern Texture", 2d) = "white" {} //The pattern transition texture
		_Cutoff("Cutoff",  Range (0, 1)) = 0 //Cut off slider
		_CutoffSteps("Cutoff Steps", Integer) = 0 //Cut off slider
		_Color("Color", Color) = (0,0,0,1) //defaults to black
		[MaterialToggle] _Flip("Flip Fade Direction", Float) = 0 //color or texture toggle
		[MaterialToggle] _UseColor("Use Color", Float) = 0 //color or texture toggle
		[MaterialToggle] _UseTransitionTexture("Use Transition Texture", Float) = 0 //color or texture toggle

	}
	SubShader
	{
        Tags {"Queue"="Transparent" "RenderType"="Cutout" }
        Cull Off ZWrite Off ZTest Always
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
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _TransTex;
			sampler2D _PatternTex;
			float4 _MainTex_ST;
			float4 _PatternTex_ST;
			float _Cutoff;
			float _CutoffSteps;
			fixed4 _Color;
			fixed4 _TintColor;
			float _UseColor;
			float _UseTransitionTexture;
			float _Flip;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _PatternTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//take the pattern ath this pixel
				fixed4 pattern = tex2D(_PatternTex, i.uv2);

				//clamp the cutoff so it can't be negative
				_Cutoff = clamp(_Cutoff, 0, 1);

				//clamp make the steps into integers for pixel patterns
				_Cutoff = round(_Cutoff * _CutoffSteps) / _CutoffSteps;

				//sample the main texture color
				fixed4 col = tex2D(_MainTex, i.uv);

				//multiply it against the tint color
				col = col * _Color;

				float cut = lerp(0, 1.1, _Cutoff);
				col.a = pattern.rgb < cut;
                clip(col.a - 0.001);
				return col;
			}
			ENDCG
		}
	}
}