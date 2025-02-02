Shader "SoundReactor/Piano/Note Stencil"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 8)) = 0.0
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 1
        _Length ("Length", Float) = 1
        _Radius ("Radius", Float) = 0.5
        _Stencil ("Stencil ID [0,255]", Int) = 0
        _ReadMask ("ReadMask [0,255]", Int) = 255
        _WriteMask ("WriteMask [0,255]", Int) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Int) = 0
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Stencil
            {
                Ref[_Stencil]
                ReadMask[_ReadMask]
                WriteMask[_WriteMask]
                Comp[_StencilComp]
                Pass[_StencilOp]
                Fail[_StencilFail]
                ZFail[_StencilZFail]
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            fixed4 _Color;
            float _EmissionStrength;
            float _Radius;
            float _Width;
            float _Length;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord: TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 dimensions : TEXCOORD1;

                UNITY_FOG_COORDS(2)

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                o.dimensions = float2(_Width / _Radius, _Length / _Radius);

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = float2
                (
                    max((abs(i.uv.x - 0.5) * i.dimensions.x + 0.5) + ((1.0 - i.dimensions.x) * 0.5), 0.0f),
                    max((abs(i.uv.y - 0.5) * i.dimensions.y + 0.5) + ((1.0 - i.dimensions.y) * 0.5), 0.0f)
                );

                // sample the texture
                fixed4 col = tex2D(_MainTex, uv) * _Color * i.color;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return fixed4(col.rgb + (col.rgb * _EmissionStrength), col.a);
            }

            ENDCG
        }
    }
}