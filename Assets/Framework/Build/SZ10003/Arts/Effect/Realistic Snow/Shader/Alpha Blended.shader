Shader "XYani/YT4021/Alpha Blended" {
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_POW("Texture POW scale", Float) = 1.0
		_InvFade("Soft Particles Factor", Float) = 1.0
		[Toggle(_USESOFTPARTICLES_ON)] _UseSoftParticles("Use Soft Particles", Float) = 1
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back
			ColorMask RGB
			Cull Off Lighting Off ZWrite Off

			SubShader {
				Pass {

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma multi_compile_particles
					#pragma multi_compile_fog
					#pragma shader_feature_local _USESOFTPARTICLES_ON

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					fixed4 _TintColor;
					float _POW;

					struct appdata_t {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						UNITY_FOG_COORDS(1)
					};

					float4 _MainTex_ST;

					v2f vert(appdata_t v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);

						o.color = v.color;
						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
						UNITY_TRANSFER_FOG(o,o.vertex);
						return o;
					}

					float _InvFade;

					float4 frag(v2f i) : SV_Target
					{
						float4 tex = tex2D(_MainTex, i.texcoord);
						tex = pow(tex,_POW);
						float4 col = i.color * _TintColor * tex;
						UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
						col.a = saturate(col.a);
						return col;
					}
					ENDCG
				}
			}
		}
}
