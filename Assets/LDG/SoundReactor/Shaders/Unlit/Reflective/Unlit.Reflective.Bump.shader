Shader "SoundReactor/Unlit/Reflective/Bump"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection ("Reflection", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _BumpMap("Bump", 2D) = "normal" {}
        _Cube ("Reflection", Cube) = "black" {}
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
            float _Reflection;
            float _Metallic;
            float _EmissionStrength;

            sampler2D _BumpMap;
            samplerCUBE _Cube;

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 uv       : TEXCOORD;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;

                float3 TtoW0    : TEXCOORD1;
                float3 TtoW1    : TEXCOORD2;
                float3 TtoW2    : TEXCOORD3;

                float3 viewDir  : TEXCOORD4;

                UNITY_FOG_COORDS(5)

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // object space to tangent space rotation
                TANGENT_SPACE_ROTATION;

                // tangent space to world space rotation
                rotation = mul((float3x3)unity_ObjectToWorld, transpose(rotation));

                o.TtoW0 = normalize(rotation[0]);
                o.TtoW1 = normalize(rotation[1]);
                o.TtoW2 = normalize(rotation[2]);

                // view direction in world space derrived from subtracting camera pos from world space vertex.
                // this fails with Scriptable Render Pipline since the camera pos variable doesn't exist in custom pipelines
                o.viewDir = UnityWorldSpaceViewDir(v.vertex.xyz);

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                // tangent space normal
                float3 tNormal = UnpackNormal(tex2D(_BumpMap, i.uv.xy));

                // world space normal
                float3 wNormal = float3(dot(i.TtoW0, tNormal), dot(i.TtoW1, tNormal), dot(i.TtoW2, tNormal));

                // reflected view direction
                float3 refl = reflect(-i.viewDir, wNormal);

                float3 reflCol = texCUBE(_Cube, refl) * _Reflection;

                float3 diff = float3(1,1,1) * _Color.rgb;

                diff += (diff * _EmissionStrength);

                fixed3 col = lerp(diff + reflCol, diff * reflCol, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);

                return fixed4(col, _Color.a);
            }
            ENDCG
        }
    }
}
