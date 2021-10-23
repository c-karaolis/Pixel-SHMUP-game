Shader "Light2D/Internal/MeshModeAdditiveBlur" {
    Properties {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

        _radius ("Radius", float) = 10
    }

    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha One
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        SubShader {

            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;

                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v)
                {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
            
                    return o;
                }

                float _radius;

                fixed4 Blur (v2f IN) {
                    float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                    float blur = _radius / 4000;     
                    float constant = 1.0 / 55.0;
                    float raySize = 3;

                    // screen ratio
                    float ratio = 9.0 / 16.0;

                    float blurRatioX = blur * ratio;
                    float blurRatioY = blur;

                    [unroll]
                    for (int x = -raySize; x < raySize; x++) {

                        float ix = raySize - abs(x);

                        [unroll]
                        for (int y = -raySize + 1; y < raySize; y++) {

                            float iy = raySize - abs(y);
                    
                            float value = sqrt(ix + iy);

                            sum += tex2D(_MainTex, float2(IN.texcoord.x + x * blurRatioX, IN.texcoord.y + y * blurRatioY)) * value;
                        }
                    }			
                    return sum * constant;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    tex = Blur(i);
                   

                    fixed4 col;
                    col.rgb = _TintColor.rgb * tex.rgb * i.color.rgb * 2.0f;
                    
                    col.a = tex.a;
                
                    return col;
                }
                ENDCG
            }
        }
    }
}