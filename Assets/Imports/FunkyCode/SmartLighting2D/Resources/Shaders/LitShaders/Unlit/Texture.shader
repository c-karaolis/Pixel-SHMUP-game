Shader "Light2D/Unlit/Texture" {

Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Lit ("Lit", Range(0,1)) = 1
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "../../LitShaders/LitCore.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 OutputColor(fixed4 spritePixel, v2f IN) {
				fixed4 lightPixel = LightColor(IN.worldPos);

				lightPixel = lerp(lightPixel, fixed4(1, 1, 1, 1), 1 - _Lit);

				return spritePixel * lightPixel;
			}

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 col = tex2D (_MainTex, IN.texcoord);

                col = OutputColor(col, IN);
     
                UNITY_APPLY_FOG(IN.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);
                return col;
            }
        ENDCG
    }
}

}