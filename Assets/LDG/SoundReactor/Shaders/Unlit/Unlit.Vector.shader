Shader "SoundReactor/Unlit/Vector"
{ 
    Properties
    {
        _EmissionStrength ("Emission Strength", Range(0, 8)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            float _EmissionStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;

                UNITY_FOG_COORDS(0)

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = fixed4(v.color.rgb + (v.color.rgb * _EmissionStrength), v.color.a);

                UNITY_TRANSFER_FOG(o, o.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color;

                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);

                return col;
            }

            ENDCG  
        }  
    }
}