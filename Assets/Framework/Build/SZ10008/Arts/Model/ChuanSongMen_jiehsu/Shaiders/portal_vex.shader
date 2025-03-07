Shader "SimuranStudio/Portal_vex"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Main Texture", 2D) = "white" {}
		[HDR]_Color1("Color 1", Color) = (1,0,0,1)
		_isk_Texture("Additional_Texture", 2D) = "white" {}
		[HDR]_Color0("Color 2", Color) = (1,0.6154708,0,1)
		_Speed_isk("Speed Additional", Vector) = (0,5,0,0)
		_Isk_Tille("Additional Tiling", Vector) = (1,1,0,0)
		_Float0("Power extra texture", Float) = 1
		_Float1("Intensity extra texture", Float) = 0.08
		_Mask_Power("Mask Power", Float) = 2
		_Mask_intens("Mask intens", Float) = 3.89

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
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
				uniform sampler2D _isk_Texture;
				uniform float2 _Isk_Tille;
				uniform float2 _Speed_isk;
				uniform float _Float0;
				uniform float _Float1;
				uniform float4 _Color1;
				uniform float4 _Color0;
				uniform float _Mask_Power;
				uniform float _Mask_intens;


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

					float2 texCoord8 = i.texcoord.xy * _Isk_Tille + ( _Time.x * _Speed_isk );
					float4 temp_cast_0 = (_Float0).xxxx;
					float2 texCoord27 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 _Color2x2 = float2(1,1);
					float4 lerpResult34 = lerp( _Color1 , _Color0 , ( 1.0 - saturate( ( pow( texCoord27.y , _Color2x2.x ) * _Color2x2.y ) ) ));
					float2 texCoord43 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float clampResult50 = clamp( ( pow( texCoord43.y , _Mask_Power ) * _Mask_intens ) , 0.0 , 2.0 );
					

					fixed4 col = ( tex2D( _TextureSample0, ( pow( tex2D( _isk_Texture, texCoord8 ) , temp_cast_0 ) * _Float1 ).rg ) * i.color * lerpResult34 * clampResult50 );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}