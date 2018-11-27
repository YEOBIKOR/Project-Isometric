Shader "Unlit/WorldObject"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 scrPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			uniform float3 _CameraPosition;
			uniform float3 _SkyColor;

			sampler2D _NoiseTex;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float2 cloudPos = (i.scrPos - _Time.r) * 0.1;
				cloudPos.y -= i.color.y * 0.01;
				float cloudValue = floor(tex2D(_NoiseTex, cloudPos).r * 4.0) * 0.25;

				col.rgb *= 1.0 - cloudValue * cloudValue;

				float3 delta = i.color - _CameraPosition;
				float distance = (delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z);
				float factor = clamp(distance * 0.005, 0.0, 1.0);

				col.rgb = lerp(col.rgb, _SkyColor, factor);

				return col;
			}
			ENDCG
		}
	}
}
