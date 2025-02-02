Shader "SoundReactor/Unlit/Micro Pattern"
{
    Properties
    {
        _Color("Tint", Color) = (1,1,1,1)
        _MainTex("Pattern Texture", 2D) = "white" {}
        _LedTex("Micro Texture", 2D) = "white" {}
        _DetailTex("Detail", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            float4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            sampler2D _LedTex;
            sampler2D _DetailTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 micro_uv : TEXCOORD1;

                UNITY_FOG_COORDS(2)
                
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;// TRANSFORM_TEX(v.uv, _MainTex);
                o.micro_uv = v.uv;
                o.micro_uv.x *= _MainTex_TexelSize.z;
                o.micro_uv.y *= _MainTex_TexelSize.w;

                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed3 detail = tex2D(_DetailTex, i.uv);
                fixed3 micro = tex2D(_LedTex, i.micro_uv) * detail;
                fixed3 col = tex2D(_MainTex, i.uv) * micro * _Color.rgb;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}
