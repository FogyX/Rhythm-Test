Shader "SoundReactor/Unlit/MatCap/Texture"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection ("Reflection", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MatCapDiff ("Diffuse", 2D) = "black" {}
        _MatCapRefl ("Reflection", 2D) = "black" {}
        [Toggle(PERSPECTIVE_ON)] _Perspective("Perspective", Float) = 1
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
            #pragma multi_compile __ PERSPECTIVE_ON

            #pragma target 3.0

            #include "UnityCG.cginc"

            float4 _Color;
            float _Reflection;
            float _Metallic;
            float _EmissionStrength;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _MatCapDiff;
            sampler2D _MatCapRefl;

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex       : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float2 dir_uv       : TEXCOORD1;
                float3 normal       : TEXCOORD2;

                UNITY_FOG_COORDS(3)

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.normal = COMPUTE_VIEW_NORMAL;

#if PERSPECTIVE_ON
                // get view space position of vertex
                float3 viewPos = UnityObjectToViewPos(v.vertex);
                float3 viewDir = normalize(viewPos);

                // get vector perpendicular to both view direction and view normal
                float3 viewCross = cross(viewDir, o.normal);

                // swizzle perpendicular vector components to create a new perspective corrected view normal
                o.normal = float3(-viewCross.y, viewCross.x, 0.0);
#endif
                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col;

                i.normal.xy = i.normal.xy * 0.5 + 0.5;

                fixed4 diff = tex2D(_MatCapDiff, i.normal.xy) * _Color * tex;
                fixed4 refl = tex2D(_MatCapRefl, i.normal.xy) * _Reflection;

                diff.rgb = diff.rgb + (diff.rgb * _EmissionStrength);

                col = lerp(diff + refl, diff * refl, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);

                // draw view space normal
                return col;
            }
            ENDCG
        }
    }
}
