Shader "SoundReactor/Unlit/Reflective/Color"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection ("Reflection", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
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

            samplerCUBE _Cube;

            struct appdata
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float3 refl     : TEXCOORD0;

                UNITY_FOG_COORDS(1)

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);

                // view direction in world space derrived from subtracting camera pos from world space vertex.
                // this fails with Scriptable Render Pipline since the camera pos variable doesn't exist in custom pipelines
                float3 view = UnityWorldSpaceViewDir(v.vertex.xyz);

                // reflect([world space view direction], [world space normal])
                o.refl = reflect(-view, normalize(mul((float3x3)unity_ObjectToWorld, v.normal)));

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                float4 diff = _Color;
                float4 refl = texCUBE(_Cube, i.refl) * _Reflection;

                diff.rgb = diff.rgb + (diff.rgb * _EmissionStrength);

                fixed4 col = lerp(diff + refl, diff * refl, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);

                return col;
            }
            ENDCG
        }
    }
}
