Shader "<effect>_ML_uv/ParticleFx_Pa_Blend_ND_UV2" {
	Properties{
		[Toggle(_CUSTOM_ON)]_CUSTOM_ON("开启Custom",Int) = 1
		_Diffuse("Diffuse", 2D) = "white" {}
		_DiffuseScale("Diffuse缩放", Range(0,10)) = 1
		_DiffuseColor("DiffuseColor", Color) = (1,1,1,1)
		_DiffusePower("DiffusePower", Float) = 1
		_FrontIntensity("FrontIntensity",float) = 1
		_DiffAngle("Diff旋转",float) = 0
		[Toggle(_DISS2U_ON)] _DISS2U_ON ("Use UV2 for Dissolve Tex", Float) = 0
		_DissolveTex("DissolveTex", 2D) = "white" {}
		_SoftSize("SoftSize",Range(0,2)) = 0
		_DissolveStep("DissolveStep",Range(0,1)) = 0
		// [Toggle(_IS_CUSTOM)]_IS_CUSTOM("自定义颜色(禁动画中K开关)",float) = 0
		_DissolveColor("DissolveColor",Color) = (1,1,1,1)
		_DissolveColorPW("DissolveColorPW",float) = 1
		_NoiseXStreng("扰动X强度",Range(-10,10)) = 0
		_NoiseYStreng("扰动Y强度",Range(-10,10)) = 0
		_GChannel("G通xy控Tiling zw控速度",Vector) = (1,1,0,0)
		// [Toggle(OpenB)]_OpenB("开启B通道(禁动画中K开关)",float)=0
		// _BChannel("B通xy控Tiling zw控Offset",Vector) = (1,1,0,0)
		[Toggle(_MASK2U_ON)] _MASK2U_ON ("Use UV2 for Mask Tex", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_MaskScale("Mask缩放", float) = 1
		_MaskAngle("Mask旋转",float) = 0
		_BackIntensity("BackIntensity",float) = 1
		_BackColor("BackColor",Color) = (1,1,1,1)
		[Toggle(_COLOUR_ON)]_COLOUR_ON("色彩开关(禁动画中K开关)",float) = 0
		_Hue("色相", Range(-0.5,0.5)) = 0
		_Saturation("饱和度", Range(0,2)) = 1
		_Contrast("对比度", Range(0,2)) = 1
		_SaturRightColor("灰度渐变亮色",Color)=(1,1,1,1)
		_SaturLeftColor("灰度渐变暗色",Color)=(1,1,1,1)
		_SaturRightColorWeights("灰度渐变亮色权重",Range(0.5,1))=1
		_SaturLeftColorWeights("灰度渐变暗色权重",Range(0,0.5))=0
		// [Toggle(_GRADIENT_ON)]_GRADIENT_ON("左右渐变颜色开关(禁动画中K开关)",float) = 0
		// [Toggle(_GRADIENT_SAME_DIFF_ON)]_GRADIENT_SAME_DIFF_ON("左右渐变开启Diff相同UV(禁动画中K开关)",float) = 0
		// _LeftColor("左侧渐变色",Color) = (1,1,1,1)
		// _RightColor("右侧渐变色",Color) = (1,1,1,1)
		// _LeftWeights("左侧渐变色权重",Range(0,1)) = 0
		// _RightWeights("右侧渐变色权重",Range(0,1)) = 1
		// _Gradient("渐变色权重偏移",Range(-1,1)) = 0
		
		[Toggle(_HEIGHTGRADIENT_ON)]_HEIGHTGRADIENT_ON("高度渐变开关(禁动画中K开关)",float) = 0
		_Height("平面高度", Float) = 0
		_HeightGradient("高度渐变值", Float) = 0.5
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",float) = 0
		[Enum(Off, 0, On, 1)]_MLZWrite("ZWrite", float) = 0
		[Enum(On, 0,Off, 4)]_MLZTest("总是最前", float) = 4
		_StencilRef("StencilRef",float)=0
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("StencilComp",float)=8
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilPass("StencilPass",float)=0
		_StencilReadMask("StencilReadMask",float)=255
		_StencilWriteMask("StencilWriteMask",float)=255
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilFail("StencilFail",float)=0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilZFail("StencilZFail",float)=0
		[HideInInspector][Toggle] _IsGray("IsGray",float) = 0
		[HideInInspector]_TransparentStrong("TransparentStrong",float) = 1
		[HideInInspector]_IsInvertGray("IsInvertGray",float) = 0
	}
	SubShader{
		Tags {
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull [_Cull]
			ZWrite [_MLZWrite]
			ZTest[_MLZTest]
			Stencil {
				Ref [_StencilRef]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
				ZFail [_StencilZFail]
			}
			CGPROGRAM
			// #pragma multi_compile __ _GRADIENT_ON
			// #pragma multi_compile __ _GRADIENT_SAME_DIFF_ON
			#pragma multi_compile __ _HEIGHTGRADIENT_ON
			#pragma multi_compile __ _COLOUR_ON
			// #pragma multi_compile __ _IS_CUSTOM
			#pragma multi_compile _CUSTOM_OFF _CUSTOM_ON
			#pragma multi_compile _DISS2U_OFF _DISS2U_ON
			#pragma multi_compile _MASK2U_OFF _MASK2U_ON

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "./Common/Colour.cginc"
			#pragma target 2.0

			float _IsGray;
			float _TransparentStrong;
			float _IsInvertGray;
			uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
			uniform float4 _DiffuseColor;
			uniform sampler2D _Mask; uniform float4 _Mask_ST;
			uniform sampler2D _DissolveTex; uniform float4 _DissolveTex_ST;
			uniform float _DiffusePower;
			float _Dissolve2U;
			float _DissolveStep;
			float _SoftSize;
			float4 _DissolveColor;
			float _DissolveColorPW;
			float _IsCustom;
			float4 _BackColor;
			float4 _GChannel;
			// float4 _BChannel;
			// float _OpenB;
			float _DiffAngle;
			float _MaskAngle;
			float _NoiseXStreng;
			float _NoiseYStreng;
			float _MaskScale;
			float _DiffuseScale;
			// float _OpenCustom;
			float _BackIntensity;
			float _FrontIntensity;
			// fixed4 _LeftColor;
			// fixed4 _RightColor;
			// fixed _LeftWeights;
			// fixed _RightWeights;
			// fixed _Gradient;
			float _Height;
			float _HeightGradient;
			half _Hue;
			half _Saturation;
			half _Contrast;
			fixed4 _SaturLeftColor;
			fixed4 _SaturRightColor;
			half _SaturLeftColorWeights;
			half _SaturRightColorWeights;
			struct VertexInput {
				float4 vertex : POSITION;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 vertexColor : COLOR;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float3 posWS:TEXCOORD3;    
				float4 vertexColor : COLOR;
			};
			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.uv0;
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWS = mul(unity_ObjectToWorld,v.vertex);
				return o;
			}
			float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
				// Deciding which uv to use
				float2 dissolveCoord = i.uv0.xy;
				float2 maskCoord = i.uv0.xy;
				#if _DISS2U_ON
					#if _CUSTOM_ON
						dissolveCoord = i.uv0.zw;
					#else
						dissolveCoord = i.uv1.xy;
					#endif
				#endif

				#if _MASK2U_ON
					#if _CUSTOM_ON
						maskCoord = i.uv0.zw;
					#else
						maskCoord = i.uv1.xy;
					#endif
				#endif
				float4 faceColor = (facing >= 0 ? fixed4(1,1,1,1)*_FrontIntensity : _BackColor*_BackIntensity);
				float4 noiseVar = tex2D(_DissolveTex,fixed2((dissolveCoord.x *_GChannel.x)+ _Time.y*_GChannel.z,(dissolveCoord.y *_GChannel.y) + _Time.y*_GChannel.w ));
				float2 noiseUV = fixed2(noiseVar.g*_NoiseXStreng,noiseVar.g*_NoiseYStreng);
				float2 diffUV = float2(i.uv0 + noiseUV);
				#if _CUSTOM_ON
					diffUV = fixed2(i.uv0 + i.uv1.xy + noiseUV);
				#endif
				float difAngle = _DiffAngle*0.017453292519943295;
				half2x2 Rot = half2x2(cos(difAngle),-sin(difAngle) ,sin(difAngle),cos(difAngle));
				float2 rotaeDiffUV=diffUV-float2(0.5,0.5);   
				rotaeDiffUV=mul(Rot,rotaeDiffUV);
				rotaeDiffUV *= _DiffuseScale;
				rotaeDiffUV += float2(0.5,0.5);
				float2 finalDiff=TRANSFORM_TEX(rotaeDiffUV,_Diffuse);
				float4 _Diffuse_var = tex2D(_Diffuse,finalDiff);
				float2 maskUV = maskCoord;
				#if _CUSTOM_ON
					maskUV = fixed2(maskCoord + i.uv1.zw);
				#endif
				float2 rotaeMaskUV=maskUV-float2(0.5,0.5);
				float maskAngle = _MaskAngle*0.017453292519943295;
				half2x2 maskRot = half2x2(cos(maskAngle),-sin(maskAngle) ,sin(maskAngle),cos(maskAngle));
				rotaeMaskUV=mul(maskRot,rotaeMaskUV);
				rotaeMaskUV *=_MaskScale;
				rotaeMaskUV += float2(0.5,0.5);
				float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(rotaeMaskUV, _Mask));
				float4 dissolveVar = tex2D(_DissolveTex,TRANSFORM_TEX(fixed2(dissolveCoord + noiseUV), _DissolveTex));
				#if _CUSTOM_ON
					dissolveVar = tex2D(_DissolveTex,TRANSFORM_TEX(fixed2(dissolveCoord + i.uv2.zw + noiseUV), _DissolveTex));
				#endif
				// float4 maskBVar = float4(1,1,1,1);
				// if(_OpenB)maskBVar=tex2D(_DissolveTex,fixed2(dissolveCoord.x *_BChannel.x+_BChannel.z,dissolveCoord.y *_BChannel.y+_BChannel.w)+noiseUV);
				// _Mask_var.r *= maskBVar.b;
				float3 diffColor = fixed3(_Diffuse_var.rgb*_DiffusePower);
				float softTmp = _SoftSize;
				#if _CUSTOM_ON
					softTmp = _SoftSize + max(0,i.uv2.x);
				#endif
				float DissolveStepTmp = _DissolveStep;
				#if _CUSTOM_ON
					DissolveStepTmp = _DissolveStep + max(0,i.uv2.y);
				#endif
				float percent = saturate((max(0.001,dissolveVar.r) - DissolveStepTmp) / (softTmp*DissolveStepTmp));
				float percentFactor = step(percent,0.8);
				float3 emissive =diffColor* faceColor;
				// float3 finalColor = float3(1,1,1);
				// #if _IS_CUSTOM
					float3 finalColor = lerp(emissive, _DissolveColor*_DissolveColorPW,percentFactor);
				// #else
				// 	finalColor = emissive;
				// #endif
				float4 finalColor2 = float4(finalColor,((_Diffuse_var.a*_Mask_var.r*percent))*_DiffuseColor.a);
				float brightness = Luminance(finalColor2.rgb);
				finalColor2.rgb = (_IsGray > 0) ? float3(brightness, brightness, brightness) : finalColor2.rgb;
				finalColor2.rgb = (_IsInvertGray > 0) ? (float3(1.0, 1.0, 1.0) - finalColor2.rgb)*finalColor2.a : finalColor2.rgb;
				finalColor2.a *= _TransparentStrong;

				//return finalColor2;

				#if _HEIGHTGRADIENT_ON
					finalColor2.a *= saturate((i.posWS.y - _Height )* ( 1.0 - step( i.posWS.y , _Height ) ) /_HeightGradient);
				#endif
				#if _COLOUR_ON
					// #ifndef _IS_CUSTOM
					// 	finalColor2.rgb = RGBToHSV(finalColor2.rgb);
					// 	finalColor2.rgb = lerp(fixed3(0,0,0),HSVToRGB(fixed3((finalColor2.r + _Hue)%360,finalColor2.g * _Saturation,finalColor2.b)),_Contrast);
					// 	finalColor2.rgb *= lerp(_SaturLeftColor.rgb,_SaturRightColor.rgb,smoothstep(_SaturLeftColorWeights,_SaturRightColorWeights,_Diffuse_var.r*_Diffuse_var.a));
					// 	finalColor2.a *= lerp(_SaturLeftColor.a,_SaturRightColor.a,smoothstep(_SaturLeftColorWeights,_SaturRightColorWeights,_Diffuse_var.r*_Diffuse_var.a));
					// #else
						if(percentFactor==0)
						{
							finalColor2.rgb = RGBToHSV(finalColor2.rgb);
							finalColor2.rgb = lerp(fixed3(0,0,0),HSVToRGB(fixed3((finalColor2.r + _Hue)%360,finalColor2.g * _Saturation,finalColor2.b)),_Contrast);
							finalColor2.rgb *= lerp(_SaturLeftColor.rgb,_SaturRightColor.rgb,smoothstep(_SaturLeftColorWeights,_SaturRightColorWeights,_Diffuse_var.r*_Diffuse_var.a));
							finalColor2.a *= lerp(_SaturLeftColor.a,_SaturRightColor.a,smoothstep(_SaturLeftColorWeights,_SaturRightColorWeights,_Diffuse_var.r*_Diffuse_var.a));
						}
					// #endif
				#endif


				// #if _GRADIENT_ON
				// 	#if _GRADIENT_SAME_DIFF_ON
				// 		half tmpX = finalDiff.x;
				// 	#else
				// 		half tmpX = i.uv0.x;
				// 	#endif
				// 	fixed4 gradientCol = lerp(_LeftColor,_RightColor,smoothstep(_LeftWeights+_Gradient,_RightWeights+_Gradient,tmpX));
				// 	finalColor2.rgb *= gradientCol.rgb;
				// 	finalColor2.a *= gradientCol.a;
				// #endif

				//return finalColor2;
				if(_IsGray < 1) {
					finalColor2.rgb *= i.vertexColor.rgb * _DiffuseColor.rgb;
					finalColor2.a *= i.vertexColor.a;
				}
				
				return finalColor2;
			}
			ENDCG
		}
	}
}
