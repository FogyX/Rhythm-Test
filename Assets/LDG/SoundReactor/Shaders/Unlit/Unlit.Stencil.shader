Shader "SoundReactor/Unlit/Stencil"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 8)) = 0.0
        
        _Stencil("Stencil ID [0,255]", Int) = 0
        _ReadMask("ReadMask [0,255]", Int) = 255
        _WriteMask("WriteMask [0,255]", Int) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Int) = 6
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Operation", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail("Stencil ZFail", Int) = 0
    }

    SubShader
    {
        Tags { "Queue"="Geometry-1" "RenderType"="Opaque" }
        ColorMask 0
        ZWrite Off
        Cull Off

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
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _EmissionStrength;

            struct appdata
            {
                float4 vertex : POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0,0,0,1);
            }

            ENDCG
        }
    }
}
