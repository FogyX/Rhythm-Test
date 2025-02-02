Shader "FX/Reveal"
{
	Properties
	{
        _Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
        _BackgroundTex ("Background", 2D) = "white" {}
        _AspectRatio ("Background Aspect Ratio", float) = 1.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            float4 _Color;

            sampler2D _MainTex;
			float4 _MainTex_ST;

            sampler2D _BackgroundTex;

            float _AspectRatio;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
                float4 proj : TEXCOORD1;
			};

            inline float4 ClipPosToUV (float4 pos)
            {
	            float4 o = pos * 0.5f;
                float aspect = _ScreenParams.x / _ScreenParams.y / _AspectRatio;

	            o.xy = float2(o.x * aspect, o.y) + o.w;
	            o.zw = pos.zw;

	            return o;
            }
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.proj = ClipPosToUV(o.vertex);

                if (_ProjectionParams.x < 0)
                {
                    o.proj.y = 1.0f - o.proj.y;
                }

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                fixed4 background = tex2Dproj(_BackgroundTex, i.proj);
				fixed4 finalColor = tex2D(_MainTex, i.uv);

				return finalColor * background * _Color;
			}
			ENDCG
		}
	}
}
