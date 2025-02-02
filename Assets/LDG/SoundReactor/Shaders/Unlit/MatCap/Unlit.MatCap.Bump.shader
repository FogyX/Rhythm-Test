Shader "SoundReactor/Unlit/MatCap/Bumped"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection("Reflection", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Bump", 2D) = "bump" {}
        _MatCapDiff("Diffuse", 2D) = "black" {}
        _MatCapRefl("Reflection", 2D) = "black" {}
        [Toggle(PERSPECTIVE_ON)] _Perspective("Perspective", Float) = 0
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
            sampler2D _BumpMap;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _MatCapDiff;
            sampler2D _MatCapRefl;

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex       : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float2 dir_uv       : TEXCOORD1;
                float3 tangent      : TEXCOORD2;
                float3 binormal     : TEXCOORD3;
                float3 normal       : TEXCOORD4;
                float3 viewDir      : TEXCOORD5;

                UNITY_FOG_COORDS(6)

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

                // object space to tangent space rotation
                TANGENT_SPACE_ROTATION;

                // tangent space to view space rotation
                rotation = mul((float3x3)UNITY_MATRIX_IT_MV, transpose(rotation));

                o.tangent = rotation[0];
                o.binormal = rotation[1];
                o.normal = rotation[2];

                o.viewDir = normalize(UnityObjectToViewPos(v.vertex));

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                // get tangent space normal
                float3 tNormal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 vNormal = normalize(float3(dot(i.tangent, tNormal), dot(i.binormal, tNormal), dot(i.normal, tNormal)));

#if PERSPECTIVE_ON
                float3 viewCross = cross(i.viewDir, vNormal);
                vNormal = float3(-viewCross.y, viewCross.x, 0.0);
#endif

                vNormal = vNormal * 0.5 + 0.5;

                // sample the texture
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed4 col;

                fixed4 diff = tex2D(_MatCapDiff, vNormal.xy) * _Color * mainTex;
                fixed4 refl = tex2D(_MatCapRefl, vNormal.xy) * _Reflection;

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
