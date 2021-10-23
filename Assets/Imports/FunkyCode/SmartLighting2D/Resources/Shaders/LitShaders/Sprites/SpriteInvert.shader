Shader "Light2D/Sprites/Inverted"
{
	Properties
	{
		[HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
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

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
                
                OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);

				return OUT;
			}

			fixed4 OutputColor(fixed4 spritePixel, v2f IN) {
				fixed4 lightPixel = LightColor(IN.worldPos);

				fixed4 inverted = fixed4(1, 1, 1, 1) - spritePixel;

				fixed4 spriteOutput = lerp(inverted, spritePixel, lightPixel.r);

				spriteOutput.a = spritePixel.a;

				return spriteOutput;
			}
        
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 spritePixel = tex2D (_MainTex, IN.texcoord) * IN.color;

				spritePixel = OutputColor(spritePixel, IN);
				spritePixel.rgb *= spritePixel.a; 
				
				return spritePixel;
			}

		    ENDCG
		}
	}
}