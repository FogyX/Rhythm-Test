Shader "SoundReactor/Unlit/Texture Cutout"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 8)) = 0.0
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _EmissionStrength;
            float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;

                UNITY_FOG_COORDS(1)

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;

                clip(col.a - _Cutoff);

                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return fixed4(col.rgb + (col.rgb * _EmissionStrength), col.a);
            }

            ENDCG
        }
    }
}
