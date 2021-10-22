Shader "Light2D/Sprites/Bumped"
{
	Properties
	{
		[HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NormTex ("Normal Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Lit ("Lit", Range(0,1)) = 1

        _NormalSize ("Normal Range", Range(0.1, 2)) = 1
        _NormalDepth("Normal Depth", Range(1, 10)) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {

		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../../LitShaders/LitCore.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color    : COLOR;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD1;
				fixed4 color    : COLOR;
                float2 worldPos : TEXCOORD0;
			};
			
			fixed4 _Color;
	
			sampler2D _MainTex;
            sampler2D _NormTex;

			float4 _Cam1_Texture_TexelSize;

            float _NormalSize;
            float _NormalDepth;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
                
                OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);

				return OUT;
			}

			fixed4 Bumped(fixed4 spritePixel, v2f IN) {

                float delta = _NormalSize;

				// light pixel
				fixed4 lightPixel = LightColor(IN.worldPos);

                fixed lightPixelLeft = LightColor(IN.worldPos + float2(-delta, 0));
                fixed lightPixelRight = LightColor(IN.worldPos + float2(delta, 0));

                fixed lightPixelUp = LightColor(IN.worldPos + float2(0, delta));
                fixed lightPixelDown = LightColor(IN.worldPos + float2(0, -delta));
                
                fixed lightHorizontal = (lightPixelRight - lightPixelLeft) * _NormalDepth;
                fixed lightVertical = (lightPixelUp - lightPixelDown) * _NormalDepth;

				float3 lightDirection = normalize(float3(lightHorizontal, lightVertical, -1));

                float3 normalDirection = (tex2D(_NormTex, IN.texcoord).xyz - 0.5f) * 2.0f;
                normalDirection = float3(mul(float4(normalDirection.xyz, 1.0f), unity_WorldToObject).xyz);
                normalDirection.z *= -1;
                normalDirection = normalize(normalDirection);

                float normalDotLight = dot(normalDirection, lightDirection);

              	lightPixel.r *= normalDotLight;
              	lightPixel.g *= normalDotLight;
               	lightPixel.b *= normalDotLight;

				lightPixel = lerp(lightPixel, fixed4(1, 1, 1, 1), 1 - _Lit);

				return spritePixel * lightPixel;
			}
        
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 spritePixel = tex2D (_MainTex, IN.texcoord) * IN.color;

				spritePixel = Bumped(spritePixel, IN);
				spritePixel.rgb *= spritePixel.a; 

				return spritePixel;
			}

		    ENDCG
		}
	}
}