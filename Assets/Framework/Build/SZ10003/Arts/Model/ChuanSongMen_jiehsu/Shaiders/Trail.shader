Shader "SimuranStudio/Trail"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		[HDR]_Color0("Color 1", Color) = (0.1367925,0.8397518,1,1)
		[HDR]_Color1("Color 2", Color) = (0.7075472,0.09678713,0.7075338,1)
		_Noise_SpeedTille("Noise_Speed", Vector) = (-0.67,0,1,1)
		_TextureSample0("Main Texture", 2D) = "white" {}
		_Trail_("Trail", Float) = 6.08
		_Color("Color concentration", Float) = 1

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend One OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _TextureSample0;
				uniform float4 _Noise_SpeedTille;
				uniform float _Trail_;
				uniform float4 _Color0;
				uniform float4 _Color1;
				uniform float _Color;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 appendResult23 = (float2(_Noise_SpeedTille.x , _Noise_SpeedTille.y));
					float2 texCoord20 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 panner21 = ( 1.0 * _Time.y * appendResult23 + texCoord20);
					float2 texCoord38 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 texCoord56 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 texCoord14 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float clampResult35 = clamp( step( ( ( i.color.a * tex2D( _TextureSample0, panner21 ).r ) - ( 1.0 - ( pow( ( texCoord38.y * _Trail_ ) , 1.0 ) * pow( ( 1.0 - ( texCoord56.y * 1.0 ) ) , 1.0 ) * pow( ( 1.0 - ( texCoord14.x * 1.0 ) ) , 1.0 ) ) ) ) , 0.0 ) , 0.0 , 1.0 );
					float2 temp_cast_0 = (_Color).xx;
					float2 texCoord6 = i.texcoord.xy * temp_cast_0 + float2( 0,0 );
					float4 lerpResult1 = lerp( _Color0 , _Color1 , ( texCoord6.x * i.color.a ));
					

					fixed4 col = ( ( 1.0 - clampResult35 ) * lerpResult1 );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}